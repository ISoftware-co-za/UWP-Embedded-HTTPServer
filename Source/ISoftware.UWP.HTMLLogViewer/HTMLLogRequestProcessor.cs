/*
Copyright 2016 ISoftware

This file is part of ISoftware.UWP.HTTPServer. ISoftware.UWP.HTTPServer is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as 
published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. ISoftware.UWP.HTTPServer is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Foobar. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Windows.Storage;
using ISoftware.UWP.HTTPServer;

namespace ISoftware.UWP.HTMLLogViewer
{
    public class HTMLLogRequestProcessor : IRequestProcessor
    {
        public HTMLLogRequestProcessor(StorageFolder logsFolder)
        {
            _logsFolder = logsFolder;
        }

        public async Task<HTTPResponse> Process(string pathPrefix, HTTPRequest request)
        {
            string cleansedPath = request.Path.ToUpperInvariant();
            ILogsRequestProcessor processor;

            if (cleansedPath == "/LOGS")
                processor = new LogsProcessor();
            else if (cleansedPath == "/LOGS/DELETE")
                processor = new LogsDeleteProcessor();
            else if (cleansedPath.Contains("/LOGS") && cleansedPath.Contains("DELETE"))
                processor = new LogsFileDeleteProcessor();
            else
                processor = new LogsFileProcessor();

            try
            {
                return await processor.GenerateResponse(request, _logsFolder);
            }
            catch (Exception exception)
            {
                return await GenerateErrorResponse(exception);
            }
        }

        private async Task<HTTPResponse> GenerateErrorResponse(Exception exception)
        {
            StorageFolder viewFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("ISoftware.UWP.HTMLLogViewer");
            StorageFile errorTemplateFile = await viewFolder.GetFileAsync("ViewError.html");
            string errorTemplate = await FileIO.ReadTextAsync(errorTemplateFile);

            errorTemplate = errorTemplate.Replace("{error}", exception.ToString());

            Dictionary<string, string> headers = new Dictionary<string, string> { ["Cache-control"] = "no-cache" };

            return new HTTPResponse(HttpStatusCode.OK, headers, new HTTPResponseBodyString(errorTemplate));
        }

        private readonly StorageFolder _logsFolder;
    }
}
