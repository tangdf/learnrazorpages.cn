# 什么是 ASP.NET Core？

ASP.NET Core是微软构建基于Web的应用程序框架的最新版本. 它位于.NET Core之上，它是一个开源开发平台，由一组框架库、一个软件开发工具包（SDK）和一个运行库组成。

## 为什么要选择 ASP.NET Core？

Web 开发正在改变，对于模块化框架的需求越来越大，在这些框架中，由您来决定要包含在应用程序中的功能。应用程序应该是基于云架构设计的——可以在任何平台上运行，可以迅速扩展。它们还应该支持客户端框架，开发 RESTful API 更加容易。框架本身需要灵活，可以快速迭代提供新功能，响应网络开发领域的新创新；开发人员希望能够选择这样的工具来开发网站。

有一些框架已经解决了这些问题，例如在Node.js上运行的 [Express](https://expressjs.com/)。传统的 ASP.NET 则不行，IIS(Internet Information Services) 只能运行在 Windows 的 Web 服务器，框架被绑定在整个 .NET Framework 中，新功能需要很长时间才能推向市场。它非常依赖于Visual Studio——一个只能在 Windows 上运行的IDE怪物。

<div class="alert alert-info">

请注意，[Visual Studio for Mac](https://docs.microsoft.com/en-us/visualstudio/mac/) 产品实际上并不是大多数 .NET 开发人员熟悉的 IDE 版本。它是支持 .NET Core 开发的 Xamarin Studio 版本。

</div>

ASP.NET Core 被设计为模块化的，HTTP管线由独立的组件组成，因此可以根据需要选择模块。这种方式带来很多好处：
*   应用程序更轻量，因为它只包含所需的组件；
*   可以从多个来源选择管线组件；
*   可以将应用程序托管到任何平台；
*   与传统的 ASP.NET 相比，新功能添加的速度要快得多。

ASP.NET Core 提供了基于模型-视图-控制器（MVC）模式的 Web 开发框架。除此之外，Razor 页面框架对于那些更熟悉或更喜欢以页面为中心的开发方法来构建 Web 应用程序的开发人员来说，这是一个非常重要的框架。ASP.NET Core 还包含用于开发基于REST的 Web 服务（Web API）的框架。包括基于 Web Sockets 的框架（SignalR）也正在开发过程中，该框架将支持从服务器发起的页面内容实时更新。