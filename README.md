# UWP-Embedded-HTTPServer
![Logo](/Images/splashLogo.png?raw=true) 

A simple HTTP server that can be embedded in an UWP application, in particular a Windows 10 IoT Core application. The HTTP Server can be used to configure, control and get status information on the IoT device.

# Quick start
To use the HTTP server do the following.

## Implement the HTTP request processing
A HTTP request is processed by class that implements the `IRequestProcesor` interface. This interface is listed below.

```c#
  public interface IRequestProcessor
  { 
      Task<HTTPResponse> Process(string routePrefix, HTTPRequest request);
  }
```

## Create an instance of the server
Create an instance of the Server object as shown below.

```c#
Server = new Server(5400, 30 * 1000, new FileLogger.FileLogger(true, logFolder));
```
The parameters specified are:
* The listening port
* The request receive timeout in milliseconds
* An instance of the `FileLogger` used to log the requests.

## Define the routing
Define the routes by linking a Path prefix to an `IRequestProcessor`.

```c#
Server.Router.AddRoute("/about", new HTTPRequestProcessorFileRetriever("about"));
Server.Router.AddRoute("/form/post", new HTTPRequestProcessorFormPost());
Server.Router.AddRoute("/form", new HTTPRequestProcessorFileRetriever("form"));
Server.Router.AddRoute("/logs", new HTMLLogRequestProcessor(logFolder));
```
## Start the Server
Call the Start method on the Server.

```c#
Server.Start();
```
The Server can be stopped by calling the Stop method.

```c#
Server.Stop();
```
# Additional information
This section contains information of other classes that may be useful when implementing an application that uses the HTTP Server.

## Predefined `IRequestProcessor` implementations
There are two predefined `IRequestProcessor` implementations.

`HTTPRequestProcessorFileRetriever` can be used to return a file.

`HTTPRequestProcessorPostFrom` can be used to process a POST request for a form. To use this class derive from it and implement the abstract method `ProcessRequest`. The form fields are passed to this method. 

## Response body classes
The content of the HTTP response body can be specified using one of the following classes.

* `HTTPResponseBodyBinaryFile`
* `HTTPResponseBodyTextFile`
* `HTTPResponseBodyString`

# Sample
The source includes the TestContainer project which is a simple UWP application that uses the HTTP Server.


