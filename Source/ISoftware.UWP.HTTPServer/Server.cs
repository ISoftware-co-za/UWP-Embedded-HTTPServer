/*
Copyright 2016 ISoftware

This file is part of ISoftware.UWP.HTTPServer. ISoftware.UWP.HTTPServer is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as 
published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. ISoftware.UWP.HTTPServer is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Foobar. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using ISoftware.UWP.Logger;

namespace ISoftware.UWP.HTTPServer
{
    public class Server
    {
        #region PROPERTIES

        public bool Running { get; private set; }

        public PathRequestProcessorRouter Router { get; }

        public List<HTTPHeader> DefaultHeadres { get; }

        #endregion

        #region CONSTRUCTION

        public Server(int port, int readTimeoutMSecs, ILogger logger)
        {
            _port = port;
            _readTimeoutMSecs = readTimeoutMSecs;
            _logger = logger;

            Router = new PathRequestProcessorRouter();
            DefaultHeadres = new List<HTTPHeader>();
        }

        #endregion

        #region EVENTS

        public event RequestProcesedEventHandler RequestProcessed;

        #endregion

        #region METHODS

        public async Task Start()
        {
            var ipAddress = GetCurrentIPAddress();
            NetworkAdapter thisAdapter = ipAddress.IPInformation.NetworkAdapter;

            _listener = new StreamSocketListener();
            _listener.ConnectionReceived += OnConnectionReceived;

            await _listener.BindServiceNameAsync(this._port.ToString(), SocketProtectionLevel.PlainSocket, thisAdapter);

            Running = true;
        }

        public void Stop()
        {
            _listener.Dispose();
            Running = false;
        }

        #endregion

        #region PRIVATE METHODS

        private async void OnConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            HTTPRequest httpRequest = null;
            HTTPResponse httpResponse = null;
            DateTime start = DateTime.Now;
            Exception exception = null;

            LoggedOperation loggedOpetation = new LoggedOperation(LogUtilities.GetMethodName(this));

            try
            {
                string requestString = await ReadRequest(args);
                httpRequest = GenerateRequest(requestString);
                httpResponse = await Router.Process(httpRequest);

                LoggedMessage message = new LoggedMessage(LoggedItemCategory.Information, $"Processing the request {httpRequest.Path}");
                await _logger.LogAsync(message);

                await WriteResponse(args, httpResponse);

                loggedOpetation.End();
                await _logger.LogAsync(loggedOpetation);
            }
            catch (Exception exp)
            {
                loggedOpetation.End(exp);
                await _logger.LogAsync(loggedOpetation);

                exception = exp;
            }

            try
            {
                TimeSpan duration = DateTime.Now - start;

                RequestProcessed?.Invoke(this, new RequestProcessedEventArgs(httpRequest, httpResponse, duration, exception));
            }
            catch (Exception e)
            {
                string msg = e.ToString();
            }
        }

        private async Task<string> ReadRequest(StreamSocketListenerConnectionReceivedEventArgs args)
        {
            const int bufferSize = 8192;
            StringBuilder request = new StringBuilder();

            using (var input = args.Socket.InputStream)
            {
                byte[] data = new byte[bufferSize];
                IBuffer buffer = data.AsBuffer();
                uint bytesRead = bufferSize;
                CancellationTokenSource readAsyncCancellation = new CancellationTokenSource(_readTimeoutMSecs);

                while (bytesRead == bufferSize)
                {
                    var receiveBuffer = await input.ReadAsync(buffer, bufferSize, InputStreamOptions.Partial).AsTask(readAsyncCancellation.Token);

                    if (receiveBuffer.Length == 0)
                        throw new TimeoutException("InputStream Socket ReadAsync timeout.");

                    byte[] receivedData = receiveBuffer.ToArray();

                    request.Append(Encoding.UTF8.GetString(receivedData, 0, receivedData.Length));
                    bytesRead = buffer.Length;
                }
            }

            string requestString = request.ToString();

            return requestString;
        }

        private async Task WriteResponse(StreamSocketListenerConnectionReceivedEventArgs args, HTTPResponse httpResponse)
        {
            using (IOutputStream output = args.Socket.OutputStream)
            {
                byte[] body = null;
                int bodyLength = 0;

                if (httpResponse.Body != null)
                {
                    if (httpResponse.Body is HTTPResponseBodyAsync)
                        body = await ((HTTPResponseBodyAsync)(httpResponse.Body)).GenerateBodyAsync();
                    else
                        body = ((HTTPResponseBody)(httpResponse.Body)).GenerateBody();
                    bodyLength = body.Length;
                }

                string responseHeader =
                    $"HTTP/1.1 {(int)(httpResponse.StatusCode)} {GetStatusCodeDescription(httpResponse.StatusCode)}\r\n" +
                    $"Content-Length: {bodyLength}\r\n" +
                    "Connection: close\r\n";

                if (DefaultHeadres.Count > 0)
                    responseHeader = DefaultHeadres.Aggregate(responseHeader, (current, header) => current + $"{header.Name}:{header.Value}\r\n");
                if (httpResponse.Headers != null)
                    responseHeader = httpResponse.Headers.Aggregate(responseHeader, (current, header) => current + $"{header.Key}:{header.Value}\r\n");
                responseHeader += "\r\n";

                byte[] headerArray = Encoding.UTF8.GetBytes(responseHeader);

                await output.WriteAsync(headerArray.AsBuffer());
                if (body != null)
                    await output.WriteAsync(body.AsBuffer());
                await output.FlushAsync();
                output.Dispose();
            }
        }

        private HostName GetCurrentIPAddress()
        {
            IReadOnlyList<HostName> hosts = NetworkInformation.GetHostNames();
            HostName ipHostName = null;

            foreach (HostName name in hosts)
            {
                if (name.Type == HostNameType.Ipv4)
                {
                    ipHostName = name;
                }
            }
            return ipHostName;
        }

        private HTTPRequest GenerateRequest(string request)
        {
            string requestLine = GetRequestLine(request);
            string[] tokens = requestLine.Split(' ');

            string method = tokens[0];
            string uri = Uri.UnescapeDataString(tokens[1]);
            string version = tokens[2];

            int indexOfSchemeSeparator = uri.IndexOf("//");

            string scheme = null;
            string remainder = null;

            if (indexOfSchemeSeparator >= 0)
            {
                scheme = uri.Substring(0, indexOfSchemeSeparator + 2);
                remainder = uri.Substring(indexOfSchemeSeparator + 2);
            }
            else
            {
                scheme = null;
                remainder = uri;
            }

            string[] pathTokens = remainder.Split('?');

            string path = pathTokens[0];
            Dictionary<string, string> queryParameters = null;

            if (pathTokens.Length > 1)
                queryParameters = GetQueryParameters(pathTokens[1]);

            Dictionary<string, string> headers = GetRequestHeaders(request);

            return new HTTPRequest(method, path, version, headers, queryParameters, GetBody(request));
        }

        private string GetStatusCodeDescription(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.OK:
                    return "OK";

                case HttpStatusCode.Found:
                    return "Found";
            }

            return "UNDEFINED";
        }

        private string GetRequestLine(string request)
        {
            int indexOfFirstNewLine = request.IndexOf("\r\n", StringComparison.Ordinal);

            if (indexOfFirstNewLine >= 0)
                return request.Substring(0, indexOfFirstNewLine);
            return request;
        }

        private Dictionary<string, string> GetRequestHeaders(string request)
        {
            Dictionary<string,string> headers = new Dictionary<string, string>();
            int startIndex = request.IndexOf("\r\n", StringComparison.Ordinal) + 2;
            int endIndex = request.IndexOf("\r\n", startIndex, StringComparison.Ordinal);

            while (endIndex - startIndex > 0)
            {
                string line = request.Substring(startIndex, endIndex - startIndex);
                string[] headerTokens = line.Split(':');

                headers[headerTokens[0]] = headerTokens[1].Trim();

                startIndex = endIndex + 2;
                endIndex = request.IndexOf("\r\n", startIndex, StringComparison.Ordinal);
            }

            return headers;
        }

        private Dictionary<string, string>  GetQueryParameters(string queryParameterString)
        {
            Dictionary<string,string> queryParameters = new Dictionary<string, string>();
            string[] tokens = queryParameterString.Split('&');

            foreach (string token in tokens)
            {
                string[] queryParameterTokens = token.Split('=');

                queryParameters[queryParameterTokens[0]] = queryParameterTokens[1];
            }

            return queryParameters;
        }

        private string GetBody(string request)
        {
            int indexOfStartOfBody = request.IndexOf("\r\n\r\n", StringComparison.Ordinal);

            if (indexOfStartOfBody >= 0)
                return request.Substring(indexOfStartOfBody + 4);
            return null;
        }

        #endregion

        #region FIELDS

        private readonly int _port;
        private readonly int _readTimeoutMSecs;
        private readonly IRequestProcessor _reqestProcessor;
        private StreamSocketListener _listener;
        private readonly ILogger _logger;

        #endregion
    }
}
