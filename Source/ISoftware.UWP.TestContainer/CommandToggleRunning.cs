/*
Copyright 2016 ISoftware

This file is part of ISoftware.UWP.HTTPServer. ISoftware.UWP.HTTPServer is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as 
published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. ISoftware.UWP.HTTPServer is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Foobar. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ISoftware.UWP.TestContainer
{ 
    internal class CommandToggleRunning : ICommand
    {
        public CommandToggleRunning(ViewModel viewModel)
        {
            _viewModel = viewModel;
            SetCommandToggleRunningCaption();
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_running)
                StopServer();
            else
                StartServer();

            _running =! _running;
            SetCommandToggleRunningCaption();
        }

        private async Task StartServer()
        {
            await _viewModel.Server.Start();
        }

        private async Task StopServer()
        {
            await _viewModel.Server.Start();
        }

        private void SetCommandToggleRunningCaption()
        {
            _viewModel.CommandToggleRunningCaption = (_running) ? "Stop" : "Start";
        }

        private bool _running;
        private readonly ViewModel _viewModel;
    }
}
