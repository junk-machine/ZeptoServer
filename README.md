# ZeptoServer
**Minimalistic light-weight server.**

The server comes with the FTP implementation, but it should be fairly easy to extend it to any text protocol. Technically it can be hosted in-proc by just using a `ServerHost` class from the `ZeptoServer` project. 

This is not claiming to be a super performant and secure FTP server, but rather a very simple implementation for educational purposes and prototyping.

## Quick solution overview

- `ZeptoServer` - server core, binds to a socket and listens for new connections. When client connects, `IServerFactory` is used to create new server instance. One `IServer` will be created per connected client, which simplifies the implementation - you take care of single connection and core infrastructure will scale it.
- `ZeptoServer.Configuration.Xml` - utility project to instantiate `ServerHost` class based on XML definition.
- `ZeptoServer.Ftp` - FTP server implementation.
- `ZeptoServer.Ftp.AzureStorage` - virtual file system for the FTP server backed by Azure Blob Storage.
- `ZeptoServer.Ftp.LocalFileSystem` - virtual file system for the FTP server backed by local file system.
- `ZeptoServer.IotHost` - UWP host app to run ZeptoServer on **Windows 10 IoT Core** devices.
- `ZeptoServer.ServiceHost` - NT service to run ZeptoServer on any Windows machine. If you want to run it as a regular console application, then use `-c` command line switch.
- `ZeptoServer.Telnet` - base server implementation for text-based protocols: FTP, HTTP, POP3, IMAP, etc.
- `ZeptoServer.TextLogger` - simple logger implementation that writes text messages to replacable sinks.

Since it has pluggable architecture (`ZeptoServer.ServiceHost` does not have any references to FTP implementation) - you will need to build all projects separately and place the output in one folder, if you want to use all components. There is no meta-project that will bin-drop everything. Both host projects rely on XML configuration to be provided: section in `app.config` for `ZeptoServer.ServiceHost` and `Configuration.xml` file for `ZeptoServer.IotHost`. XML definition is a poor man's XAML, where you can reference CLR types and it will call their constructors with matching set of arguments. `ZeptoServer.IotHost` references FTP implementation to simplify `appx` packaging in Visual Studio.

Happy coding!