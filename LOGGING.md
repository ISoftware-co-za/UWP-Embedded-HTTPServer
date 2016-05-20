#Logging
This file describes logging functionality that may be used by the `Server` and the application it is embedded in.

## Writting log entries
The Logger project defines the `ILogger` interface and its associated types. The FileLogger project is a file based implementation of the `ILogger`. 

An operation can be logged by writting a `LoggedOperation` to an `ILogger`. A message can be logged by writing a `LoggedMessage` to an `ILogger`.

Log entries can be assigned one of the following categories:
* Information
* Warning
* Error

## Viewing log entries
The HTTPLogViewer project implements an `IRequestProcessor` that can be used to view and clear the files created by the FileLogger. The HTTPFileLogger uses HTML templates to produce the HTML output . 

The HTTPLogViewer can be used as an example of an `IRequestProcessor` implementation. The TestContainer links the **/logs** path to the `HTTPLogRequestProcessor` which is an `IRequestProcessor`. It processes the requests that allow the log entries to be viewed via a browser.   
