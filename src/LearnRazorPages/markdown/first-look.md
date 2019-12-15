

# 初识 Razor 页面


在本节中，您将看到如何使用 .NET Core 命令行工具创建一个简单的 Razor 页面应用程序以及如何在浏览器中构建和运行它。您还将探索应用程序的各个组成部分，并了解每个部分所扮演的角色。


## 尝试：创建你的第一个 Razor 页面应用程序


1. 如果您还没有安装，请先[下载.NET Core 2.0 SDK](https://www.microsoft.com/net/download/core)。


2. 打开您首选的命令行工具（Windows 中的 cmd.exe 或 Powershell；Mack中终端；在Linux上使用Bash或类似的方式）并输入`dotnet --version`。


3. 检查输出以确认您的 .NET Core 版本至少为2.0。

    ![.Net Core version](/images/15-08-2017-08-20-06.png)


4. 如果需要，可以选择创建应用程序的合适位置，然后在控制台中输入`mkdir RazorPages`，创建应用程序文件的文件夹。


5. 在控制台中输入`cd RazorPages`，进入应用程序文件夹。


6. 在控制台中输入`dotnet new razor`。这个命令从标准的站点模板生成应用程序文件。您会收站点已经创建提示信息以及`dotnet restore`命令正在运行。`dotnet restore`命令的含义是检查应用程序所需的依赖关系，并从 NuGet 获取它们。

    ![dotnet restore](/images/15-08-2017-08-26-24.png)


7. 在控制台中输入`dotnet run`编译应用程序并在端口5000上启动程序：

    ![dotnet run](/images/06-06-2017-09-20-23.png)


8. 打开您喜欢的浏览器并导航到`http://localhost:5000`，网站应该显示如下：

    ![Default template](/images/06-06-2017-09-26-34.png)


## Razor 页面应用程序剖析


Razor 页面应用程序包含许多文件夹和文件。在下一节中，您将探索在上一节创建、恢复、编译和运行应用程序时生成的文件。以下是应用程序文件夹结构的资源管理器视图，然后进一步剖析每个文件夹或每组文件：

![Razor Pages file structure](/images/15-08-2017-08-33-20.png)


### Pages文件夹

![Razor Pages Folder](/images/15-08-2017-08-42-52.png)


_Pages_ 文件夹是 Razor 页面文件的默认存放位置。以 _.cshtml_ 结尾的文件是 Razor 文件，以 _.cs_ 结尾的文件是C#类文件。这些与 Razor 文件配对，被称为 PageModel 文件。并非所有的 Razor 文件都有一个匹配的类文件。一些Razor文件的文件名中带有下划线(`_`)，这些文件不能浏览，但它们作为 Razor 页面应用程序的一部分，扮演着不同的角色。您可以[阅读更多](/razor-pages/files)关于这些特殊的 Razor 文件及其作用。


### wwwroot文件夹

![wwwroot Folder](/images/15-08-2017-08-49-06.png)


_wwwroot_ 文件夹是 .NET Core Web 应用程序存放静态文件的文件夹。这些文件包括网站使用的CSS样式表、图像和 JavaScript 文件。另一个名为lib的文件夹包含第三方客户端软件包，这些包由Bower（Razor页面和MVC应用程序默认的客户端包管理器）管理。



### lib 文件夹

![lib folder](/images/15-08-2017-08-51-06.png)


在模板中包含的第三方客户端软件包有：
*   BootStrap：由 Twitter 团队构建的UI框架，可减少站点布局设计所需的工作，还包括一系列内置样式和组件，比如按钮、表单、表格、文本输入框等等。
*   jQuery：BootStrap依赖的 JavaScript 库。
*   jQuery Validation：用于客户端表单校验的 jQuery 插件
*   jQuery Unobtrusive Validation：另一个由ASP.NET团队设计的 jQuery 校验插件，专门用于由Razor标签助手生成的表单输入。



### 根文件夹

![root files](/images/15-08-2017-08-49-57.png)


根文件夹里包括很多文件。


| 文件                             | 描述                                       |
| ------------------------------ | ---------------------------------------- |
| _.bowerrc_                     | Bower 配置文件                                |
| _appsettings.json_             | 基于 json 格式的文件，用于应用程序范围的[配置设置](/configuration) |
| _appsettings.Development.json_ | 基于 json 格式的文件，用于应用程序范围的[配置设置](/configuration)，仅在开发过程中生效 |
| _bower.json_                   | Bower 清单文件                                |
| _bundleconfig.json_            | 客户端文件压缩或打包合并配置文件                         |
| _Program.cs_                   | Razor 页面应用程序的入口点                          |
| _RazorPages.csproj_            | 基于 XML 格式的文件，包含项目有关信息                      |
| _Startup.cs_                   | [Startup类](/startup)配置请求管线，用于处理所有对应用程序请求。 |




### Bin 文件夹

![bin Folder](/images/15-08-2017-09-08-59.png)


_bin_ 文件夹是应用程序编译时生成二进制文件的默认输出位置。通常，这个文件夹包含两个子文件夹，_Debug_和_Release_。第一个是在 Debug 模式下编译应用程序所产生的二进制文件，第二个是放置在Release模式下编译生成的二进制文件。这两个文件夹都包含名为 _netcoreapp[版本号]_ 的子文件夹，其中[版本号]表示用于创建应用程序的 .NET Core 的版本。如果您使用的是 .NET Core 2.0，则文件夹名称将为 _netcoreapp2.0_。您在上图中看到的文件来自 _/bin/Debug/netcoreapp2.0/_。



### Obj文件夹

![obj Folder](/images/15-08-2017-09-16-50.png)


_Obj_ 文件夹用于存储临时对象文件和其它用于在编译过程中创建最终二进制文件的文件。

