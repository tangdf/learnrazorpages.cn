# Razor 页面中间件

## 请求管线

当向 Web 应用程序发出请求时，需要通过多种方式进行处理。需要考虑许多因素，应该将请求定向或路由到哪里？是否应该记录请求的细节？应用程序是否应该简单地返回文件的内容？应该压缩响应？如果在处理请求时遇到异常，会发生什么情况？发出请求的人实际上是否允许访问他们请求的资源？如何处理Cookie或其它与请求相关的数据？

这些处理操作中的每一个都由独立的组件执行。用来描述这些组件的术语是中间件 。它们一起构成请求管线。


## ASP.Net Core 中间件

在以前的 ASP.NET 版本中，影响请求管道的组件全部捆绑在 _System.Web.dll_ 类库中，您可能不需要在Web应用程序运行的所有组件。

在 ASP.NET Core 中，请求管线中间件是在`Startup`类的`Configure`方法中注册。默认项目模板包含如下代码：

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
	if (env.IsDevelopment())
	{
		app.UseDeveloperExceptionPage();
		app.UseDatabaseErrorPage();
	}
	else
	{
		app.UseExceptionHandler("/Error");
	}

	app.UseStaticFiles();

	app.UseAuthentication();

	app.UseMvcWithDefaultRoute();
}

```

包括错误处理中间件，用于处理静态文件（图像，样式表，脚本文件，PDF等）请求的中间件，认证管理中间件（如果在创建项目时启用认证）和 MVC 框架。每个组件都使用`IApplicationBuilder`类型的扩展方法进行注册。

组件注册的顺序决定了它们的执行顺序。处理中间件的错误是最先被注册的，这样它就可以在所有代码中都可以使用，并且可以在管线中获得异常。

中间件可以终止管线执行并返回响应，也可以将控制权交给下一个组件。静态文件中间件终止管线的执行，并在响应中发送请求的静态文件内容。当请求静态文件时，不会调用身份验证和MVC中间件。其它组件将执行传递给下一个注册的组件。

## 创建中间件

这里有两个中间件的例子。第一个简单地返回一个响应，并在`Configure`方法中使用`IApplicationBuilder.Run()`方法定义：

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.Run(async (context) =>
    {
        await context.Response.WriteAsync("All done");
    });
	...

}
```

此示例终止或短路管线。不会有其它任何中间件组件被执行。第二个示例使用推荐的方式创建中间件，创建单独的类，然后在`IApplicationBuilder`类型上创建一个扩展方法来注册它：

_MyMiddleware.cs_

```
public class ElapsedTimeMiddleware
{
    private readonly ILogger _logger;
    public ElapsedTimeMiddleware(RequestDelegate next, ILogger<ElapsedTimeMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {

        var sw = new Stopwatch();
        sw.Start();

        await _next(context);

        var isHtml = context.Response.ContentType?.ToLower().Contains("text/html");
        if (context.Response.StatusCode == 200 && isHtml.GetValueOrDefault())
        {
            _logger.LogInformation($"{context.Request.Path} executed in {sw.ElapsedMilliseconds}ms");
        }
    }
}
```

该中间件计算处理请求所用的时间，然后记录该信息。

这个类包含一个`RequestDelegate`和`ILogger`参数的构造函数。这些由依赖注入系统提供。`RequestDelegate`代表在管线中的下一个中间件。`Invoke`方法包含中间件的主体。

在这个例子中，一个`Stopwatch`实例被启动。然后调用请求委托，执行其余的管线。所有后续中间件执行后，该行后面的代码被执行。如果当前请求结果是HTML，则记录已用时间：

![Elapsed Time Middleware](/images/09-01-2018-12-46-18.png)

用于注册中间件的扩展方法如下：

```csharp
public static class BuilderExtensions
{
    public static IApplicationBuilder UseElapsedTimeMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ElapsedTimeMiddleware>();
    }
}
```

该方法在`Startup`类的`Configure`方法中调用：

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
    }

    app.UseStaticFiles();

    app.UseElapsedTimeMiddleware();

    app.UseAuthentication();

    app.UseMvcWithDefaultRoute();
}
```

ElapsedTime中间件在StaticFiles中间件 _之后_ 注册，指定对静态文件的请求不会启动秒表。