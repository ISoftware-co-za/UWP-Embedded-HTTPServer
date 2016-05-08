/*
Copyright 2016 ISoftware

This file is part of ISoftware.UWP.HTTPServer. ISoftware.UWP.HTTPServer is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as 
published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. ISoftware.UWP.HTTPServer is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Foobar. If not, see http://www.gnu.org/licenses/.
*/

using System.Collections.Generic;

namespace ISoftware.UWP.HTTPServer
{
    public class HTTPRequest
    {
        public string Method { get; private set; }

        public string Path { get; private set; }

        public string Version { get; private set; }

        public Dictionary<string, string> Headers { get; private set; }

        public Dictionary<string, string> QueryParameters { get; private set; }

        public string Body { get; private set; }

        public HTTPRequest(string method, string path, string version, Dictionary<string, string> headers, Dictionary<string, string> queryParameters, string body)
        {
            Method = method;
            Path = path;
            Version = version;
            Headers = headers;
            QueryParameters = queryParameters;
            Body = body;
        }
    }
}
