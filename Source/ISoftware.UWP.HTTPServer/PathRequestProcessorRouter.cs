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

namespace ISoftware.UWP.HTTPServer
{
    public class PathRequestProcessorRouter
    {
        public PathRequestProcessorRouter()
        {
            _routes = new List<RouteProcessorLink>();
        }

        public void AddRoute(string pathPrefix, IRequestProcessor processor)
        {
            _routes.Add(new RouteProcessorLink(pathPrefix, processor));
        }

        public async Task<HTTPResponse> Process(HTTPRequest request)
        {
            if (request.Path.ToUpperInvariant() == "/FAVICON.ICO")
                return await ProcessFavicon(request);

            RouteProcessorLink routeProcessorLink  = FindRequestProcessorForPath(request.Path);

            if (routeProcessorLink != null)
                return await routeProcessorLink.RequestProcessor.Process(routeProcessorLink.PathPrefix, request);
            return await ProcessorNotFound(request);
        }

        private RouteProcessorLink FindRequestProcessorForPath(string path)
        {
            foreach (RouteProcessorLink route in _routes)
            {
                if (route.PathPrefix.Length <= path.Length)
                {
                    string pathPrefix = path.Substring(0, route.PathPrefix.Length).ToUpperInvariant();

                    if (route.PathPrefix.ToUpperInvariant() == pathPrefix)
                        return route;
                }
            }

            return null;
        }

        private async Task<HTTPResponse> ProcessFavicon(HTTPRequest request)
        {
            StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("ISoftware.UWP.HTTPServer");
            StorageFile favicon = await folder.GetFileAsync("favicon.ico");

            Dictionary<string,string> headers = new Dictionary<string, string>();

            headers["Content-type"] = "image/png";

            return new HTTPResponse(HttpStatusCode.OK, headers, new HTTPResponseBodyBinaryFile(favicon));
        }

        private async Task<HTTPResponse> ProcessorNotFound(HTTPRequest request)
        {
            StorageFolder htmlFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("ISoftware.CourtControl.HTTPServer");
            StorageFile htmlFile = await htmlFolder.GetFileAsync("HTTPServerRouteNotFound.html");
            string html = await FileIO.ReadTextAsync(htmlFile);

            html = html.Replace("{method}", request.Method);
            html = html.Replace("{path}", request.Path);

            return new HTTPResponse(HttpStatusCode.OK, null, new HTTPResponseBodyString(html));
        }

        private readonly List<RouteProcessorLink> _routes;
    }
}
