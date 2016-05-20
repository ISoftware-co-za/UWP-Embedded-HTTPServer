using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Windows.Storage;
using ISoftware.UWP.HTTPServer;

namespace ISoftware.UWP.HTMLLogViewer
{
    internal class LogsStaticContentProcessor: ILogsRequestProcessor
    {
        public async Task<HTTPResponse> GenerateResponse(HTTPRequest request, StorageFolder logsFolder)
        {
            string fileName = Path.GetFileName(request.Path);
            StorageFolder logStaticFileFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Logs");
            StorageFile staticFile = await logStaticFileFolder.GetFileAsync(fileName);

            return new HTTPResponse(HttpStatusCode.OK, null, new HTTPResponseBodyTextFile(staticFile));
        }
    }
}
