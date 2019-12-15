# Razor 页面身份验证


Razor 页面使用 ASP.NET Identity 作为默认成员资格和身份验证系统。在本节中，将探索与 ASP.NET Identity 中涉及到 Razor 页面的各个部分，首先介绍在默认项目模板生成的文件。

如果您正在使用 Visual Studio，可以在创建 Razor 页面项目时，指定使用身份验证系统。

![Razor Pages Identity](https://www.mikesdotnetting.com/images/30-08-2017-13-55-27.png)

单击更改身份认证按钮，然后选择使用个人用户帐户，并将用户数据存储在自己的数据库中：

![Razor Pages Identity](https://www.mikesdotnetting.com/images/30-08-2017-13-56-55.png)

创建项目后，构建它以确保恢复所有必需的包。

如果您更喜欢使用命令行工具来生成模板文件，导航到想要保存项目的文件夹，并使用以下命令:

```cmd
dotnet new razor --auth individual
```

项目创建的两种方法的主要区别是，Visual Studio 模板会导致应用程序使用Sql Server localdb作为数据存储，而命令行使用跨平台的SQLite数据库。

ASP.NET Identity 使用 Entity Framework Core 进行数据访问。EF Core使用迁移来保持数据库与模型同步。第一次迁移生成脚本并创建 ASP.NET Identity 使用的表结构。SQLite版本已经应用了第一个迁移，因此您将在项目根目录中找到一个 _app.db_ 的文件。如果使用Visual Studio模板，则需要运行`update-database`命令来执行第一次迁移。在这两种情况下，数据库的架构都是一样的：

![Razor Pages Identity Schema](https://www.mikesdotnetting.com/images/04-09-2017-13-41-56.png) ![Razor Pages Identity Schema](https://www.mikesdotnetting.com/images/04-09-2017-13-45-57.png)


## 项目概览

在项目包括一些通过默认模板创建的文件夹和文件：

![Razor Pages Identity](https://www.mikesdotnetting.com/images/05-09-2017-09-01-48.png)


_Data_ 文件夹包含 Entity Framework Core 所需要的文件，包括迁移和`DbContext`类。


 _Services_ 文件夹中包含`EmailSender`类及其接口，并且具有一个空的`SendEmailAsync`方法。如果打算使用这个类，需要提供自己的实现。

_Extensions_ 文件夹包含一些有用的扩展方法，用于在电子邮件中创建确认链接。


_Pages_ 文件夹中的 _Account_ 文件夹包含多个 Razor 页面文件，用于管理最常见的身份验证相关任务。

最后， _Controllers_ 文件夹包含一个MVC控制器的代码——`AccountController`，它已经包含在内，用于注销。它有一个Action方法——`Logout`将用户注销，记录操作，然后重定向到主页。这个过程使用MVC控制器是故意的——因为注销过程没有关联的UI，所以认为没有必要使用 Razor 页面，因为它的目的就是生成UI。

## 将注销控制器更改为 Razor 页面

如果您不喜欢在应用程序中使用MVC控制器，可以更改注销过程，改为使用 Razor 页面：

1. 在 _Pages_ 文件夹添加一个  _Logout.cshtml_ Razor 页面；
2. 添加`Layout = null;`代码块到  _.cshtml_ 文件中；
3. 将`LogoutModel`文件中的代码更改为以下内容：

    ```csharp
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using RazorPagesIdentity.Data;
    using System.Threading.Tasks;

    namespace RazorPagesIdentity.Pages
    {
        public class LogoutModel : PageModel
        {
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly ILogger _logger;

            public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
            {
                _signInManager = signInManager;
                _logger = logger;
            }
            public async Task<IActionResult> OnPost()
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out.");
                return RedirectToPage("/Index");
            }
        }
    }

    ```

4.  打开 _LoginPartial.cshtml_ 文件。从 Form 标签助手中删除以下属性：

    ```html
    asp-controller="Account" asp-action="Logout" method="post"
    ```
    
5. 用下面的`asp-page`属性替换它们：

    ```html
    asp-page="Logout"
    ```
    
	开始标签现在应该是这样的：
	
    ```html
    <form asp-page="Logout" id="logoutForm" class="navbar-right">
    ```

现在可以安全地删除 _Controllers_ 文件夹和它的所有内容，但是在做之前，我们应该看一下`AccountController`的内容，并将其与`LogoutModel`类文件内容进行比较。它们几乎是相同的。
