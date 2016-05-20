/*
Copyright 2016 ISoftware

This file is part of ISoftware.UWP.HTTPServer. ISoftware.UWP.HTTPServer is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as 
published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. ISoftware.UWP.HTTPServer is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Foobar. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Core;
using ISoftware.UWP.HTMLLogViewer;
using ISoftware.UWP.HTTPServer;
using ISoftware.UWP.TestContainer.Form;

namespace ISoftware.UWP.TestContainer
{
    internal class ViewModel : INotifyPropertyChanged
    {
        #region PROPERTIES

        public Server Server { get; set; }

        public string CommandToggleRunningCaption
        {
            get { return _commandToggleRunningCaption; }
            set
            {
                if (_commandToggleRunningCaption != value)
                {
                    _commandToggleRunningCaption = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CommandToggleRunningCaption"));
                }
            }
        }
        private string _commandToggleRunningCaption;

        public ObservableCollection<ListItemRequestResponse> Log { get; set; }

        #endregion

        #region COMMANDS

        public ICommand CommandToggleRunning { get; set; }

        public ICommand CommandClearLog { get; set; }

        #endregion

        #region CONSTRUCTION

        public ViewModel(CoreDispatcher dispatcher)
        {
            _dispatcher = dispatcher;

            CommandToggleRunning = new CommandToggleRunning(this);
            CommandClearLog = new CommandClearLog(this);

            Log = new ObservableCollection<ListItemRequestResponse>();
        }

        #endregion

        #region EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region METHODS

        public async Task Initialise()
        {
            StorageFolder logFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Logs", CreationCollisionOption.OpenIfExists);

            Server = new Server(5400, 30 * 1000, new FileLogger.FileLogger(true, logFolder));

            Server.DefaultHeaders.Add(new HTTPHeader(HTTPHeaders.Cache_Control, "no-cache"));

            Server.Router.AddRoute("/about", new HTTPRequestProcessorFileRetriever("about"));
            Server.Router.AddRoute("/form/post", new HTTPRequestProcessorFormPost());
            Server.Router.AddRoute("/form", new HTTPRequestProcessorFileRetriever("form"));
            Server.Router.AddRoute("/logs", new HTMLLogRequestProcessor(logFolder));

            Server.RequestProcessed += (sender, args) =>
            {
                _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ListItemRequestResponse logEntry = new ListItemRequestResponse(args);

                    Log.Insert(0, logEntry);
                });
            };
        }

        #endregion

        #region PROTECTED METHODS

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region FIELDS

        private readonly CoreDispatcher _dispatcher;

        #endregion
    }
}
