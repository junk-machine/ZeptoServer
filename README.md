# ZeptoServer
**Minimalistic light-weight server.**

The server comes with the FTP implementation, but it should be fairly easy to extend it to any text protocol. Technically it can be hosted in-proc by just using a `ServerHost` class from the `ZeptoServer` project, but there is also an NT service/console host under `ZeptoServer.ServiceHost`. By default it runs as an NT service, if you want to run it as a regular console application, then use `-c` command line switch.

Since it has pluggable architecture - you will need to build all project separately and place the output in one folder, if you want to use all components. There is no meta-project that will bin-drop everything. I was too lazy to roll-out my own solution to enable/disable plugins, so it relies on XAML in the app.config to instantiate the `ServerHost` in the service host project.

This is not claiming to be a super performant and secure FTP server, but rather a very simple implementation for educational purposes and prototyping.

Happy coding!