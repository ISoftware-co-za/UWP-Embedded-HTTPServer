/*
Copyright 2016 ISoftware

This file is part of ISoftware.UWP.HTTPServer. ISoftware.UWP.HTTPServer is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as 
published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. ISoftware.UWP.HTTPServer is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Foobar. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using ISoftware.UWP.Logger;
using Newtonsoft.Json;

namespace ISoftware.UWP.FileLogger
{
    public class FileLogger : ILogger
    {
        public const string MessageFilePrefix = "MESSAGE";

        public const string OperationFilePrefix = "OPERATION";

        public bool Verbose { get; }

        public FileLogger(bool verbose, StorageFolder folder)
        {
            Verbose = verbose;
            _folder = folder;
        }

        public async Task LogAsync(LoggedMessage loggedMessage)
        {
            if (!Verbose && loggedMessage.Category == LoggedItemCategory.Information)
                return;

            string serialisedLoggedMessage = JsonConvert.SerializeObject(loggedMessage);
            string fileName = GenerateFileName(loggedMessage.DateTime, loggedMessage.Category);

            await WriteTextFile(fileName, serialisedLoggedMessage);
        }

        public async Task LogAsync(LoggedOperation loggedOperation)
        {
            if (!Verbose && loggedOperation.Category == LoggedItemCategory.Information)
                return;

            string serialisedLoggedMessage = JsonConvert.SerializeObject(loggedOperation);
            string fileName = GenerateFileName(loggedOperation.DateTime, loggedOperation.Category, loggedOperation.Operation);

            await WriteTextFile(fileName, serialisedLoggedMessage);
        }

        private string GenerateFileName(DateTime dateTime, LoggedItemCategory loggedItemCategory)
        {
            Interlocked.Increment(ref _counter);
            return $"{MessageFilePrefix} {dateTime.Year}-{dateTime.Month:00}-{dateTime.Day:00}H{dateTime.Hour:00}M{dateTime.Minute:00}S{dateTime.Second:00}({_counter}) {loggedItemCategory}.json";
        }

        private string GenerateFileName(DateTime dateTime, LoggedItemCategory loggedItemCategory, string operation)
        {
            Interlocked.Increment(ref _counter);
            return $"{OperationFilePrefix} {dateTime.Year}-{dateTime.Month:00}-{dateTime.Day:00}H{dateTime.Hour:00}M{dateTime.Minute:00}S{dateTime.Second:00}({_counter}) {loggedItemCategory} {operation}.json";
        }

        private async Task WriteTextFile(string fileName, string serialisedLoggedMessage)
        {
            using (Stream writer = await _folder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.OpenIfExists))
            {
                byte[] content = System.Text.Encoding.UTF8.GetBytes(serialisedLoggedMessage);

                await writer.WriteAsync(content, 0, content.Length);
            }
        }

        private readonly StorageFolder _folder;
        private static int _counter;
    }
}
