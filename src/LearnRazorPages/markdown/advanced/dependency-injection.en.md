# Dependency Injection in Razor Pages

Dependency Injection (DI) is a technique that promotes loose coupling of software through separation of concerns. In the context of a Razor Pages application, DI encourages you to develop discrete components for specific tasks, which are then injected into classes that need to use their functionality. This results in an application that is easier to maintain and test.

## The Problem

Many people think that the real problem with DI is the terminology that surrounds it. This section seeks to address that by providing an illustration of the problem that DI is designed to solve.

The following sample of code features the [page model](/razor-pages/pagemodel) class for a contact form:

```
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

And, for completeness, here is the contact form:

```
<form method="post">
    <label asp-for="From"></label> <input type="text" asp-for="From"/><br>
    <label asp-for="Email"></label> <input type="text" asp-for="Email"/><br>
    <label asp-for="Subject" ></label> <input type="text" asp-for="Subject" /><br>
    <label asp-for="Comments"></label> <textarea asp-for="Comments"></textarea><br>
    <input type="submit"/>
</form>

```

When the form is posted, the email is constructed in the `OnPost` handler method and sent, and the user is redirected to a page named "Thanks".

This is an extremely simple example for the purposes of explanation. The code is brief and looks similar to countless other examples of sending email using ASP.NET. But there are issues with the code - if you want to change the way that the comments are handled, you have to change the `ContactModel` class, which increases the chances of introducing bugs into the `ContactModel`. Also, you cannot possibly unit test the code in the `ContactModel`'s `OnPost` method without causing an email to be sent which means that the unit test is not a unit test. It's an integration test. Finally, if you have other pages on the site that use the same code (e.g. a support form), you have multiple places to update if you want to change from Outlook to Gmail, for example.

Developers are advised to implement the [**SOLID** principals of software design](https://en.wikipedia.org/wiki/SOLID_(object-oriented_design)) to ensure that their applications are robust and easier to maintain and extend. Another important guiding principal for developers is Don't Repeat Yourself (DRY), which states that you should aim to reduce code repetition wherever possible.

The `ContactModel` contravenes the **S** in **SOLID** - the _Single Responsibility Principal_ (SRP) which states that a class should only have _one_ responsibility. Page model classes have a responsibility - to determine the response based on the request. Any other tasks that need to be performed as part of processing the request should be handled by different classes, designed solely for those responsibilities.

The `ContactModel` class also contravenes the **D** in **SOLID** - the _Dependency Inversion Principal_ (DIP) which states that high level modules (the `ContactModel` class) should not rely (depend) on low level modules (in this case, `System.Net.Mail`). They should rely on abstractions (typically interfaces, but also abstract classes) instead. Dependency Injection is the most common way to achieve DIP.

## Single Responsibility Principal and DRY

The first part of the solution to reducing the issues outlined above is to implement SRP, and at the same time, adhere to DRY. This is achieved by creating a separate class for handling the comments.

```
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

Now the `OnPost` method can be refactored:

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

The code for sending emails is located in one place - the `CommentService` class. Its `Send` method contains the code that previously occupied the majority of the `ContactModel`'s `OnPost` method. The service class can be called anywhere in the application where its functionality is required. This satisfies DRY. The `ContactModel` is no longer responsible for creating and sending the email. It uses the `CommentService` to do that. Both classes satisfy SRP.

## Dependency Inversion Principal

The `ContactModel` is still dependent on a specific comment handling component - the `CommentService` class. It is "tightly coupled" to this dependency. It instatiates an instance of `CommentService` in the `OnPost` method. There is currently no getting away from it. If you want to change the way that comments are handled, you still have to make changes to the body of the `ContactModel` to change the component that provides the service, and/or the method that is called.

DIP states that the `CommentService` should be represented as an abstraction - an interface or abstract class. The most common approach is to use interfaces to provide the abstraction. Here is an interface that represents sending a message:

```
using System.Threading.Tasks;

namespace RazorPages.Services
{
    public interface ICommentService
    {
        Task Send(string from, string subject, string email, string comments);
    }
}

```

Next, the existing CommentService has to implement the interface:

```
namespace RazorPages.Services
{
    public class CommentService : ICommentService
    {
        public async Task Send(string from, string subject, string email, string comments)
        {
            // rest of existing code

```

Now the `ContactModel` can depend on an interface:

```
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

The change sees a private field called `_commentService` added to the `ContactModel`. The `ContactModel` also has a constructor added that takes a parameter of type `ICommentService`. This is assigned to the private field in the constructor, and then it is used in the `OnPost` method.

Now you can provide any component to the `ContactModel`, so long as it implements the `ICommentService` interface i.e. it has a `Send` method that takes four strings. It doesn't matter whether the Send method uses SMTP to send an email, stores the comments in a text file, Tweets them or posts them to Facebook. The `ContactModel` doesn't need to know, nor will it need to be modified if the Send action changes. Concerns are separated into different classes which are now loosely coupled. They are not dependent on each other.

At the moment, the code above will compile, but it will generate an `InvalidOperationException` at runtime whenever the ASP.NET framework attempts to create an instance of `ContactModel`. The reason for this is that currently, the framework is unable to resolve an implementation of `ICommentService` to pass to the constructor of the `ContactModel` when an instance is instantiated.

So how does the `CommentService` class used by the `ContactModel` class get resolved?

## Inversion of Control Containers

At their most basic, Inversion of Control (IoC) containers, also know as Dependency Injection Containers, are components that

*   maintain a registry of interfaces and concrete implementations
*   resolve and provide the registered concrete implementation when they are requested
*   manage the lifetime of the .

ASP.NET Core's built in DI system supports _constructor injection_, so it resolves implementations of dependencies passed in as parameters to the constructor method of objects. Before it can do that, the implementations must be registered with the container. Typically, implementations (or "services") are registered in the `ConfigureServices` method in the [Startup](/startup) class. The following code shows the `CommentService` being registered:

```
public void ConfigureServices(IServiceCollection services)
{
	services.AddMvc();
    services.AddTransient<ICommentService, CommentService>();
}

```

The `CommentService` is registered with the `AddTransient` method. This ensures that a new instance of `CommentService` is created each time it is requested. There are two other methods for registering services: `AddSingleton`, which ensures that an instance is created the first time one is requested, and then maintained for the lifetime of the application, and `AddScoped`, which results in an instance of the dependency being created and maintained for the duration of a request.

## IServiceCollection Extension Methods

The `AddMvc` method is an extension method on `IServiceCollection` that takes care of registering all the dependencies related to the MVC framework, such as model binding, action and page invokers and so on. Similar methods exist for registering other commonly used services within a Razor Pages application such as `AddDbContext` to register an Entity Framework `DbContext`.

You can create your own extension methods easily enough. Here's an example for the `CommentService`:

```
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

This can be used in the `ConfigureServices` method as follows:

```
public void ConfigureServices(IServiceCollection services)
{
	services.AddMvc();
    services.RegisterCommentService();
}

```

This approach helps to keep the Startup class a lot less cluttered, especially as you can chain calls to the various `AddTransient`, `AddScoped` etc. methods, which means your extension method could look like this:

```
public static IServiceCollection RegisterMyServices(this IServiceCollection services)
{
    return services
            .AddTransient<ICommentService, CommentService>()
            .AddTransient<ISecondService, SecondService>()
            .AddTransient<IThirdService, ThirdService>()
            .AddTransient<IFourthService, FourthService>();
}

```

And then only one line is required in the `ConfigureServices` method to register numerous services:

```
public void ConfigureServices(IServiceCollection services)
{
	services.AddMvc();
    services.RegisterMyServices();
}

```