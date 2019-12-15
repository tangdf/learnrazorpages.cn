# Razor 页面依赖注入

依赖注入(DI)是一种通过分离关注点来促进软件解耦的技术。在 Razor 页面应用程序的上下文中，DI鼓励为特定的任务开发离散的组件，然后将这些组件注入到需要使用其功能的类中。这有利于应用程序的维护和测试。

## 问题

许多人认为DI的真正问题是围绕它的术语。本节试图通过提供DI设计来解决问题。

下面的示例代码展示了一个联系人表单的 [PageModel](/razor-pages/pagemodel)  类：

```csharp
public class ContactModel : PageModel
{
    [BindProperty] public string From { get; set; }
    [BindProperty] public string Email { get; set; }
    [BindProperty] public string Subject { get; set; }
    [BindProperty] public string Comments { get; set; }

    public async Task<IActionResult> OnPost()
    {
        using (var smtp = new SmtpClient())
        {
            var credential = new NetworkCredential
            {
                UserName = "user@outlook.com",  // replace with valid value
                Password = "password"  // replace with valid value
            };
            smtp.Credentials = credential;
            smtp.Host = "smtp-mail.outlook.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            var message = new MailMessage
            {
                Body = $"From: {From} at {Email}<p>{Comments}</p>",
                Subject = Subject,
                IsBodyHtml = true
            };
            message.To.Add("contact@domain.com");
            await smtp.SendMailAsync(message);
            return RedirectToPage("Thanks");
        }
    }
}

```

而且，为了完整起见，这里是联系表单：

```html
<form method="post">
    <label asp-for="From"></label> <input type="text" asp-for="From"/><br>
    <label asp-for="Email"></label> <input type="text" asp-for="Email"/><br>
    <label asp-for="Subject" ></label> <input type="text" asp-for="Subject" /><br>
    <label asp-for="Comments"></label> <textarea asp-for="Comments"></textarea><br>
    <input type="submit"/>
</form>
```

表单提交后，电子邮件将在`OnPost`处理方法中构建并发送，用户将重定向到“Thanks”的页面。

只为达到演示的目的，这是一个非常简单的例子。代码很简短，看起来类似于使用 ASP.NET 发送电子邮件的无数其它示例。但是代码有一些问题 ——如果您想改变处理`Comments`的方式，必须更改`ContactModel`类，这增加了`ContactModel`引用bug的可能性。另外，不可能在`ContactModel`的`OnPost`方法中不发送电子邮件的前提下对代码进行单元测试，这意味着单元测试不再是单元测试。这是一个集成测试。最后，如果网站上的其它页面使用相同的代码（例如支持表单），再例如，如果要从Outlook更改为Gmail，则需要更新改多个位置。

建议开发人员使用软件设计的[SOLID原则](https://en.wikipedia.org/wiki/SOLID_(object-oriented_design))，保证应用程序的健壮性、易于维护和扩展。开发人员的另一个重要的指导原则是不要重复自己（DRY），它指出您应该尽可能减少代码重复。

`ContactModel`违反了**SOLID**原则中的**S** ——_单一职责原则_（SRP），其中规定，一个类应该只有一个任务。PageMode l类的任务——根据请求确定响应。在处理请求时需要执行的任何其它任务都应该由不同的类来处理，这些类专为这些任务而设计。

`ContactModel`类还违反了**SOLID**原则中的**D** —— 依赖倒置原则（DIP）的规定，高级别模块（`ContactModel`类）不应该依赖（依赖）于低级别的模块（在这种情况下是`System.Net.Mail`）。它们应该依靠抽象（通常是接口，而不是抽象类）。依赖注入是实现DIP最常见的方式。

## SRP和DRY

解决上述问题的第一部分是实施SRP，同时坚持DRY。这是通过创建一个单独的类来处理评论。

```csharp
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RazorPages.Services
{
    public class CommentService
    {
        public async Task Send(string from, string subject, string email, string comments)
        {
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "user@outlook.com",  // replace with valid value
                    Password = "password"  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp-mail.outlook.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                var message = new MailMessage
                {
                    Body = $"From: {from} at {email}<p>{comments}</p>",
                    Subject = subject,
                    IsBodyHtml = true
                };
                message.To.Add("contact@domain.com");
                await smtp.SendMailAsync(message);
            }
        }
    }
}

```

现在对`OnPost`方法重构：

```
public class ContactModel : PageModel
{
    [BindProperty] public string From { get; set; }
    [BindProperty] public string Email { get; set; }
    [BindProperty] public string Subject { get; set; }
    [BindProperty] public string Comments { get; set; }

    public async Task<IActionResult> OnPost()
    {
        var service = new CommentService();
        await service.Send(From, Subject, Email, Comments);
        return RedirectToPage("Thanks");
    }
}

```

发送电子邮件的代码位于一个地方 ——`CommentService`类。其`Send`方法包含以前占领了`ContactModel`类`OnPost`方法大部分的代码。服务类可以在需要其功能的应用程序中的任何地方调用。这满足了DRY。`ContactModel`不再负责创建和发送电子邮件。它使用的`CommentService`来完成。两个类都满足SRP。

## 依赖倒置原理

在`ContactModel`仍然依赖于具体的评论处理组件——`CommentService`类。它与这种依赖“紧密结合”。它会在`OnPost`方法中创建一个`CommentService`实例。目前还没有摆脱它。如果要更改处理评论的方式，则还必须更改`ContactModel`提供服务的组件和/或调用的方法的主体。

DIP指出`CommentService`应该将其表示为一个抽象 —— 一个接口或抽象类。最常见的方法是使用接口来提供抽象。这是一个表示发送消息的接口：

```csharp
using System.Threading.Tasks;

namespace RazorPages.Services
{
    public interface ICommentService
    {
        Task Send(string from, string subject, string email, string comments);
    }
}

```

接下来，现有的`CommentService`必须实现这个接口：

```csharp
namespace RazorPages.Services
{
    public class CommentService : ICommentService
    {
        public async Task Send(string from, string subject, string email, string comments)
        {
            // rest of existing code

```

现在`ContactModel`可以只依靠一个接口了：

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Services;
using System.Threading.Tasks;

namespace RazorPages.Pages
{
    public class ContactModel : PageModel
    {
        private readonly ICommentService _commentService;

        public ContactModel(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [BindProperty] public string From { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Subject { get; set; }
        [BindProperty] public string Comments { get; set; }

        public async Task<IActionResult> OnPost()
        {
            await _commentService.Send(From, Subject, Email, Comments);
            return RedirectToPage("Thanks");
        }
    }
}

```

改变看到一个`_commentService`私有字段被添加到`ContactModel`类。`ContactModel`还添加一个构造函数，它接受`ICommentService`类型的参数，在构造函数中被分配给的私有字段，然后在`OnPost`方法中使用它。

现在，可以为`ContactModel`提供任何组件，只要它实现了`ICommentService`接口，即它有一个`Send`方法，该方法需要4个字符串。发送方法是否使用SMTP发送电子邮件、将评论存储在文本文件中、在Tweets中发布或将其发布到Facebook上并不重要。`ContactModel`不需要知道，也不需要在发送操作更改时进行修改。关注点被分离到现在松散耦合的不同类中。它们不相互依赖。

目前，上面的代码可以编译，但它会在运行时，每当ASP.Net框架试图创建`ContactModel`实例时生成一个`InvalidOperationException`。这种错误的原因是，当实例被实例化时，框架无法解析`ICommentService`的实现，从而传递给`ContactModel`的构造函数。

那么，`ContactModel`类使用的`CommentService`类是如何得到解决的呢？


## 控制反转容器

在最基本的情况下，控制反转(IoC)容器，也称为依赖注入容器，是具有下例组件功

* 维护接口和具体实现的注册表
* 请求时解决并提供已注册的具体实现
* 管理生命周期

ASP.NET Core 内置的DI容器支持构造函数注入，所以它将解析构造函数依赖的实现，并作为参数传递构造函数。在做到这一点之前，必须将实现注册到容器中。通常，实现（或“服务”）在`Startup`类的`ConfigureServices`方法中注册。以下代码展示`CommentService`注册的方法：

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.AddMvc();
    services.AddTransient<ICommentService, CommentService>();
}
```

`CommentService`是用`AddTransient`方法注册的。这确保每次请求时都创建一个`CommentService`的新实例。还有两个方法来注册服务：`AddSingleton`确保第一次请求时创建实例，并且只创建一次，然后维护在应用程序的生命周期中；`AddScoped`在请求期间创建和维护依赖项的实例。

## 用构造函数参数注册服务

有时注册的服务实现需要一个或多个构造函数参数传递给它。例如，使用一种数据访问技术，它需要一个明确的连接字符串传递给它（如[Dapper](https://github.com/StackExchange/Dapper)）。不必在整个应用程序中引用相同的连接字符串，而是创建一个`Factory`类来创建应用程序可以使用的链接，并将链接字符串作为参数传递到`Startup`类中。

这里是一个`Factory`类的例子，它返回一个连接对象，在它之前实现一个接口：

```csharp
public interface IConnectionFactory
{ 
    IDbConnection CreateConnection();
}

public class SqlConnectionFactory: IConnectionFactory 
{
        
	readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
```

`Factory`类的构造函数需要一个表示传递给连接的连接字符串参数。下面的例子说明了如何使用`AddSingleton`方法的重载注册`IConnectionFactory`为服务，解析为`SqlConnectionFactory`，同时满足提供连接字符串的要求：

```csharp
public void ConfigureServices(IServiceCollection services)
{
    var connString = Configuration.GetConnectionString("DefaultConnection");
    if (connString == null)
        throw new ArgumentNullException("Connection string cannot be null");
    
    services.AddSingleton<IConnectionFactory>(s =>  new SqlConnectionFactory(connString));
	//...
}
```

`AddTransient`和`AddScoped`方法存在类似的重载。

## IServiceCollection扩展方法

`AddMvc`方法是`IServiceCollection`的一个扩展方法，负责注册与MVC框架相关的所有依赖项，例如模型绑定、Action和页面调用者等。Razor 页面应用程序中还有类似的方法来注册其它常用的服务，例如`AddDbContext`注册实体框架`DbContext`。

您可以轻松创建自己的扩展方法。以下是一个`CommentService`的例子：

```csharp
using Microsoft.Extensions.DependencyInjection;
using RazorPages.Services;

namespace RazorPages
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterCommentService(this IServiceCollection services)
        {
            return services.AddTransient<ICommentService, CommentService>();
        }
    }
}
```

可以在`ConfigureServices`方法中使用如下方式：

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.AddMvc();
    services.RegisterCommentService();
}
```

这种方法有助于保持`Startup` 类清洁，尤其是当使用链式调用各种`AddTransient`，`AddScoped`方法等，这意味着您的扩展方法看起来是这样的：

```csharp
public static IServiceCollection RegisterMyServices(this IServiceCollection services)
{
    return services
            .AddTransient<ICommentService, CommentService>()
            .AddTransient<ISecondService, SecondService>()
            .AddTransient<IThirdService, ThirdService>()
            .AddTransient<IFourthService, FourthService>();
}

```

然后在`ConfigureServices`方法中只需要一行就可以注册多个服务：

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.AddMvc();
    services.RegisterMyServices();
}
```