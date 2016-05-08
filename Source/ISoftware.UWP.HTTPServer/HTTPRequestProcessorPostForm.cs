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
using ISoftware.UWPl.HTTPServer;

namespace ISoftware.UWP.HTTPServer
{
    public abstract class HTTPRequestProcessorPostForm : IRequestProcessor
    {
        public async Task<HTTPResponse> Process(string pathPrefix, HTTPRequest request)
        {
            Dictionary<string, string> formFields = HTTPBodyUtilities.GetFormContent(request);

            Dictionary<string,string> responseHeaders = new Dictionary<string, string>();
            object responseBody = await ProcessRequest(formFields);

            if (responseBody is StorageFile)
            {
                StorageFile responseFile = responseBody as StorageFile;
                FileExtensionToMimeTypeMap extensionToContentTypeMap = new FileExtensionToMimeTypeMap();

                responseHeaders[HTTPHeaders.Content_Type] = extensionToContentTypeMap.Map(responseFile.FileType);
                return new HTTPResponse(HttpStatusCode.OK, responseHeaders, new HTTPResponseBodyTextFile(responseFile));
            }

            if (responseBody is string)
            {
                FileExtensionToMimeTypeMap extensionToContentTypeMap = new FileExtensionToMimeTypeMap();

                responseHeaders[HTTPHeaders.Content_Type] = extensionToContentTypeMap.Map(".HTML");
                return new HTTPResponse(HttpStatusCode.OK, responseHeaders, new HTTPResponseBodyString(responseBody as string));
            }

            if (responseBody == null)
                return new HTTPResponse(HttpStatusCode.OK, null, null);

            throw new Exception($"The HTTPRequestProcessorPostForm does not currently support a response type of {responseBody.GetType().FullName}.");
        }

        protected abstract Task<object> ProcessRequest(Dictionary<string, string> formFields);
    }
}
