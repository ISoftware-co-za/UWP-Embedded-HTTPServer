/*
Copyright 2016 ISoftware

This file is part of ISoftware.UWP.HTTPServer. ISoftware.UWP.HTTPServer is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as 
published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. ISoftware.UWP.HTTPServer is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Foobar. If not, see http://www.gnu.org/licenses/.
*/

using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using ISoftware.UWP.HTTPServer;

namespace ISoftware.UWP.TestContainer
{
    public class ListItemRequestResponse
    {
        public string Request { get; private set; }

        public string ResponseContent { get; private set; }

        public TimeSpan Duration { get; private set; }

        public Brush Background { get; private set; }

        public string ExceptionMessage { get; private set; }

        public Visibility VisibilityExceptionMessage { get; private set; }

        public ListItemRequestResponse(RequestProcessedEventArgs args)
        {
            Request = args.Request != null ? args.Request.Path : "Undefined";

            if (args.Response != null)
            {
                if (args.Response.Headers != null && args.Response.Headers.ContainsKey(HTTPHeaders.Content_Type))
                    ResponseContent = args.Response.Headers[HTTPHeaders.Content_Type];
            }

            Duration = args.Duration;

            if (args.Exception != null)
            {
                ExceptionMessage = args.Exception.Message;
                VisibilityExceptionMessage = Visibility.Visible;
                Background = _failureBackground;
            }
            else
            {
                VisibilityExceptionMessage = Visibility.Collapsed;
                Background = _successBackground;
            }
        }

        private static readonly SolidColorBrush _successBackground = new SolidColorBrush(Color.FromArgb(255,200,255,200));
        private static readonly SolidColorBrush _failureBackground = new SolidColorBrush(Color.FromArgb(255, 255, 200, 200));

    }
}
