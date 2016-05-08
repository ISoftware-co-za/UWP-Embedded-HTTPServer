/*
Copyright 2016 ISoftware

This file is part of ISoftware.UWP.HTTPServer. ISoftware.UWP.HTTPServer is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as 
published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. ISoftware.UWP.HTTPServer is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Foobar. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Threading.Tasks;

namespace ISoftware.UWP.Logger
{
    public class LoggedOperation
    {
        public LoggedItemCategory Category { get; set; }

        public DateTime DateTime { get; set; }

        public string Duration { get; set; }

        public string Operation { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public LoggedOperation()
        {
        }

        public LoggedOperation(string operation)
        {
            DateTime = DateTime.Now;
            Operation = operation;
        }

        public void End(Exception exception)
        {
            Category = LoggedItemCategory.Error;
            StackTrace = exception.ToString();
            Message = exception.GetUserMessage();
            SetDuration();
        }

        public void End(string message, LoggedItemCategory category = LoggedItemCategory.Warning)
        {
            Category = category;
            Message = message;
            SetDuration();
        }

        public void End()
        {
            Category = LoggedItemCategory.Information;
            SetDuration();
        }

        private void SetDuration()
        {
            Task.Delay(123);

            Duration = (DateTime.Now - DateTime).ToString();
        }
    }
}
