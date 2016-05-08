﻿/*
Copyright 2016 ISoftware

This file is part of ISoftware.UWP.HTTPServer. ISoftware.UWP.HTTPServer is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as 
published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. ISoftware.UWP.HTTPServer is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Foobar. If not, see http://www.gnu.org/licenses/.
*/

namespace ISoftware.UWP.HTTPServer
{
    internal class RouteProcessorLink
    {
        public string PathPrefix { get; private set; }

        public IRequestProcessor RequestProcessor { get; private set; }

        public RouteProcessorLink(string pathPrefix, IRequestProcessor requestProcessor)
        {
            PathPrefix = pathPrefix;
            RequestProcessor = requestProcessor;
        }
    }
}