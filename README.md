# UWP-Embedded-HTTPServer
A simple HTTP server that can be embedded in a UWP application, in particular an Windows 10 IoT Core application.

![Logo](/images/logo.png?raw=true)

# Quick start
To use the HTTP server do the following.

## Implement the HTTP request processing.
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



