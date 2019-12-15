# What is ASP.NET Core?

ASP.NET Core is the latest version of Microsoft's framework for building web-based applications. It sits on top of .NET Core, which is an open source development platform, consisting of a set of framework libraries, a software development kit (SDK) and a runtime.

## Why should you choose ASP.NET Core?

Web development is changing. There is an increasing need for more modular frameworks, where you decide the features to include in your application. Applications should be cloud-ready - designed to run on any platform and to scale up quickly. They should also embrace client-side frameworks and make developing RESTful APIs easy. And the frameworks themselves need to be nimble. They need to iterate quickly to deliver new features in response to new innovations in the web development sphere. And developers want to be able to choose the tools they use to author sites.

Some frameworks already address these concerns, such as [Express](https://expressjs.com/) that runs on Node.js. The old version of ASP.NET doesn't. It is wedded to Internet Information Services, a Windows-only web server. New features take ages to come to market because of the way that the framework is tied to the full .NET framework. And it's very dependent on Visual Studio - a monster of an IDE that only runs on Windows.

<div class="alert alert-info">

Note that the product known as [Visual Studio for Mac](https://docs.microsoft.com/en-us/visualstudio/mac/) is not actually a version of the IDE that most .NET developers are familiar with. It is a version of Xamarin Studio that supports .NET Core development.

</div>

ASP.NET is designed to be modular. The HTTP pipeline is composed of separate components that can be plugged in as needed. The benefits that this approach delivers include:

*   your application is more lightweight as it only incorporates the components it needs
*   you can choose pipeline components from multiple sources
*   you can choose which platform to host your application on
*   new features are added much more quickly than in previous versions of ASP.NET.

ASP.NET Core provides a web development framework based on the Model-View-Controller (MVC) pattern. On top of that sits the Razor Pages framework for developers who are more familiar with or prefer a page-centric development approach to building web applications. ASP.NET Core also includes a framework for developing REST-based web services (Web API). Work is also being done to include a Web Sockets-based framework (SignalR) which will enable real-time updating of page content initiated by the server.