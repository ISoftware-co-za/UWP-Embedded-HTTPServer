/*
Copyright 2016 ISoftware

This file is part of ISoftware.UWP.HTTPServer. ISoftware.UWP.HTTPServer is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as 
published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. ISoftware.UWP.HTTPServer is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Foobar. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Threading.Tasks;
using Windows.Storage;
using ISoftware.UWP.HTTPServer;
using ISoftware.UWP.Logger;
using Newtonsoft.Json;

namespace ISoftware.UWP.HTMLLogViewer
{
    internal class LogsFileProcessor : ILogsRequestProcessor
    {
        public async Task<HTTPResponse> GenerateResponse(HTTPRequest request, StorageFolder logsFolder)
        {
            StorageFolder templateFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("ISoftware.UWP.HTMLLogViewer");
            string[] pathTokens = request.Path.Split('/');

            if (pathTokens.Length != 3)
                throw new Exception($"Cannot process the request as '{request.Path} is not a valid URL for a log file.");

            StorageFile logFile = await logsFolder.GetFileAsync(pathTokens[2]);
            string fileTemplate;

            if (logFile.Name.Contains(FileLogger.FileLogger.MessageFilePrefix))
                fileTemplate = await GenerateMessageTemplate(templateFolder, logFile);
            else
                fileTemplate = await GenerateOperationTemplate(templateFolder, logFile);

            StorageFile masterTemplateFile = await templateFolder.GetFileAsync("ViewFileMaster.html");
            string masterTemplate = await FileIO.ReadTextAsync(masterTemplateFile);

            masterTemplate = masterTemplate.Replace("{template}", fileTemplate);
            return ResponseUtilities.GenerateHTMLResponse(masterTemplate);
        }

        private async Task<string> GenerateMessageTemplate(StorageFolder templateFolder, StorageFile messageFile)
        {
            StorageFile templateFile = await templateFolder.GetFileAsync("ViewFileMessage.html");
            string template = await FileIO.ReadTextAsync(templateFile);

            string fileContent = await FileIO.ReadTextAsync(messageFile);
            LoggedMessage loggedMessage = JsonConvert.DeserializeObject<LoggedMessage>(fileContent);

            template = template.Replace("{date}", loggedMessage.DateTime.Format());
            template = template.Replace("{category}", loggedMessage.Category.ToString());
            template = template.Replace("{message}", loggedMessage.Message);

            return template;
        }

        private async Task<string> GenerateOperationTemplate(StorageFolder templateFolder, StorageFile operationFile)
        {
            StorageFile templateFile = await templateFolder.GetFileAsync("ViewFileOperation.html");
            string template = await FileIO.ReadTextAsync(templateFile);

            string fileContent = await FileIO.ReadTextAsync(operationFile);
            LoggedOperation loggedOperation = JsonConvert.DeserializeObject<LoggedOperation>(fileContent);

            template = template.Replace("{date}", loggedOperation.DateTime.Format());
            template = template.Replace("{duration}", loggedOperation.Duration);
            template = template.Replace("{category}", loggedOperation.Category.ToString());
            template = template.Replace("{operation}", loggedOperation.Operation);
            template = template.Replace("{message}", loggedOperation.Message);
            template = template.Replace("{stackTrace}", loggedOperation.StackTrace);

            return template;
        }
    }
}
