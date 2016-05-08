/*
Copyright 2016 ISoftware

This file is part of ISoftware.UWP.HTTPServer. ISoftware.UWP.HTTPServer is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as 
published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. ISoftware.UWP.HTTPServer is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Foobar. If not, see http://www.gnu.org/licenses/.
*/

using System;

namespace ISoftware.UWP.HTTPServer
{
    public delegate void RequestProcesedEventHandler(object sender, RequestProcessedEventArgs args);

    public class RequestProcessedEventArgs : EventArgs
    {
        public HTTPRequest Request { get; private set; }

        public HTTPResponse Response { get; private set; }

        public TimeSpan Duration { get; private set; }

        public Exception Exception { get; private set; }

        public RequestProcessedEventArgs(HTTPRequest request, HTTPResponse response, TimeSpan duration, Exception exception)
        {
            Request = request;
            Response = response;
            Duration = duration;
            Exception = exception;
        }
    }
}
