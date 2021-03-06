﻿/*
Copyright 2016 ISoftware

This file is part of ISoftware.UWP.HTTPServer. ISoftware.UWP.HTTPServer is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as 
published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. ISoftware.UWP.HTTPServer is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Foobar. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using ISoftware.UWP.HTTPServer;

namespace ISoftware.UWPl.HTTPServer
{
    public static class HTTPBodyUtilities
    {
        public static Dictionary<string, string> GetFormContent(HTTPRequest request)
        {
            Dictionary<string,string> formFields = new Dictionary<string, string>();
            string contentType = request.Headers[HTTPHeaders.Content_Type];

            if (contentType != "application/x-www-form-urlencoded")
                throw new Exception("The HTTP request body does not contain form data.");

            string[] pairs = request.Body.Split('&');

            foreach (string pair in pairs)
            {
                string[] pairTokens = pair.Split('=');

                formFields[pairTokens[0]] = pairTokens[1];
            }

            return formFields;
        }
    }
}
