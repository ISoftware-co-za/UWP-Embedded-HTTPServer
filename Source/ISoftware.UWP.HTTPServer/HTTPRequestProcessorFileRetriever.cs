/*
Copyright 2016 ISoftware

This file is part of ISoftware.UWP.HTTPServer. ISoftware.UWP.HTTPServer is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as 
published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. ISoftware.UWP.HTTPServer is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Foobar. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ISoftware.UWPl.HTTPServer;

namespace ISoftware.UWP.HTTPServer
{
    public class HTTPRequestProcessorFileRetriever : IRequestProcessor
    {
        public HTTPRequestProcessorFileRetriever(string folder)
        {
            _folder = folder;
        }

        public async Task<HTTPResponse> Process(string pathPrefix, HTTPRequest request)
        {
            string fileName = request.Path.Substring(pathPrefix.Length + 1);
            string fileExtension = GetCleansedFileExtension(fileName);
            var applicationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var fileFolder = await applicationFolder.GetFolderAsync(_folder);
            var file = await fileFolder.GetFileAsync(fileName);

            if (string.IsNullOrEmpty(fileExtension))
                throw new Exception("The Content-Type of the response cannot be determined as the file requested does not include an extension.");

            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                [HTTPHeaders.Content_Type] = FileExtensionToMimeTypeMap.Map(fileExtension)
            };

            HTTPResponseBodyAsync body;

            if (IsTextFile(fileExtension))
                body = new HTTPResponseBodyTextFile(file);
            else
                body = new HTTPResponseBodyBinaryFile(file);

            return new HTTPResponse(HttpStatusCode.OK, headers, body);
        }

        private bool IsTextFile(string fileExtension)
        {
            return fileExtension == ".HTM" || fileExtension == ".HTML" || fileExtension == ".CSS" || fileExtension == ".JS";
        }

        private string GetCleansedFileExtension(string fileName)
        {
            string extension = Path.GetExtension(fileName);

            if (!string.IsNullOrEmpty(extension))
                return extension.ToUpperInvariant();
            return null;
        }

        private readonly string _folder;
        private static readonly FileExtensionToMimeTypeMap FileExtensionToMimeTypeMap = new FileExtensionToMimeTypeMap();
    }
}
