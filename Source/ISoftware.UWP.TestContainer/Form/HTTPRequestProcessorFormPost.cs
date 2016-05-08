using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using ISoftware.UWP.HTTPServer;

namespace ISoftware.UWP.TestContainer.Form
{
    public class HTTPRequestProcessorFormPost : HTTPRequestProcessorPostForm
    {
        public HTTPRequestProcessorFormPost() 
        {
        }

        protected override async Task<object> ProcessRequest(Dictionary<string, string> formFields)
        {
            var applicationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var fileFolder = await applicationFolder.GetFolderAsync("Form");
            var file = await fileFolder.GetFileAsync("FormPost.html");
            string fileContent = await FileIO.ReadTextAsync(file);

            foreach (string field in formFields.Keys)
            {
                string fieldPlaceholder = $"{{{{{field}}}}}";

                fileContent = fileContent.Replace(fieldPlaceholder, formFields[field]);
            }

            return fileContent;
        }
    }
}
