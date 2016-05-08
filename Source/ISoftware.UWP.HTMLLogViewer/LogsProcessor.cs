/*
Copyright 2016 ISoftware

This file is part of ISoftware.UWP.HTTPServer. ISoftware.UWP.HTTPServer is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as 
published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. ISoftware.UWP.HTTPServer is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Foobar. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using ISoftware.UWP.HTTPServer;
using ISoftware.UWP.Logger;
using Newtonsoft.Json;

namespace ISoftware.UWP.HTMLLogViewer
{
    internal class LogsProcessor : ILogsRequestProcessor
    {
        public async Task<HTTPResponse> GenerateResponse(HTTPRequest request, StorageFolder logsFolder)
        {
            IReadOnlyCollection<StorageFile> logFiles = await logsFolder.GetFilesAsync();
            StorageFolder viewFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("ISoftware.UWP.HTMLLogViewer");
            StorageFile masterTemplateFile = await viewFolder.GetFileAsync("ViewLogsMaster.html");
            StorageFile rowTemplateFile = await viewFolder.GetFileAsync("ViewLogsRow.html");
            string rowTemplate = await FileIO.ReadTextAsync(rowTemplateFile);

            StringBuilder table = new StringBuilder();

            foreach (var logFile in logFiles)
            {
                if (logFile.Name.StartsWith(FileLogger.FileLogger.MessageFilePrefix))
                    table.Append(await GenerateMessageRow(rowTemplate, logFile));
                if (logFile.Name.StartsWith(FileLogger.FileLogger.MessageFilePrefix))
                    table.Append(await GenerateOperationRow(rowTemplate, logFile));
            }

            string htmlPage = await FileIO.ReadTextAsync(masterTemplateFile);

            htmlPage = htmlPage.Replace("{table}", table.ToString());
            return ResponseUtilities.GenerateHTMLResponse(htmlPage);
        }

        private async Task<string> GenerateMessageRow(string rowTemplate, StorageFile messageFile)
        {
            string fileContent = await FileIO.ReadTextAsync(messageFile);
            LoggedMessage loggedMessage = JsonConvert.DeserializeObject<LoggedMessage>(fileContent);

            string fileAnchor = $"<a href=\"/logs/{messageFile.Name}\">{loggedMessage.DateTime.ToString("yyyy-MM-dd hh:mm:ss")}</a>";

            rowTemplate = rowTemplate.Replace("{date}", fileAnchor);
            rowTemplate = rowTemplate.Replace("{type}", "MESSAGE");
            rowTemplate =rowTemplate.Replace("{category}", loggedMessage.Category.ToString());
            rowTemplate =rowTemplate.Replace("{description}", loggedMessage.Message);

            return rowTemplate;
        }

        private async Task<string> GenerateOperationRow(string rowTemplate, StorageFile operationFile)
        {
            string fileContent = await FileIO.ReadTextAsync(operationFile);
            LoggedOperation loggedMessage = JsonConvert.DeserializeObject<LoggedOperation>(fileContent);

            string fileAnchor = $"<a href=\"/logs/{operationFile.Name}\">{loggedMessage.DateTime.ToString("yyyy-MM-dd hh:mm:ss")}</a>";

            rowTemplate = rowTemplate.Replace("{date}", fileAnchor);
            rowTemplate = rowTemplate.Replace("{type}", "OPERATION");
            rowTemplate = rowTemplate.Replace("{category}", loggedMessage.Category.ToString());
            rowTemplate = rowTemplate.Replace("{description}", loggedMessage.Operation);

            return rowTemplate;
        }
    }
}
