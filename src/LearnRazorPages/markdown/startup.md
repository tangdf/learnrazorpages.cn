# Startup 类

ASP.NET Core 应用程序被设计为模块化，因此开发人员只需要包含他们需要的功能和服务。每个 .NET Core 应用程序都需要一个 Startup 类来配置应用程序的功能和服务。默认情况下，这个类的名字是 _Startup_，不守也可以将其命名为其它任何名称。

`Startup.cs`文件被当作 Startup 类作为项目默认模板的一部分生成。它有一个必需的`Configure`方法和一个可选的`ConfigureServices`方法。`Configure`方法用于配置请求管线，`ConfigureServices`方法用于配置应用程序可以使用服务。

<div class="alert alert-info">

## 请求管线

当向 Web 应用程序发出请求时，需要通过多种方式进行处理。需要考虑许多因素，应该将请求定向或路由到哪里？是否应该记录请求的细节？应用程序是否应该简单地返回文件的内容？应该压缩响应？如果在处理请求时遇到异常，会发生什么情况？发出请求的人实际上是否允许访问他们请求的资源？如何处理Cookie或其它与请求相关的数据？

这些处理操作中的每一个都由独立的组件执行。用来描述这些组件的术语是 [中间件](/middleware) 。它们一起构成请求管线。

</div>


## Configure方法

这是默认项目模板中的`Configure`方法：

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
	if (env.IsDevelopment())
	{
		app.UseDeveloperExceptionPage();
	}
	else
	{
		app.UseExceptionHandler("/Error");
	}

	app.UseStaticFiles();

	app.UseMvc(routes =>
	{
		routes.MapRoute(
			name: "default",
			template: "{controller=Home}/{action=Index}/{id?}");
	});
}
```

传递给方法的参数是`IApplicationBuilder`和`IHostingEnvironment`。实际上不需要任何参数，`IApplicationBuilder` 通过向它添加中间件来配置请求管线的组件，`IHostingEnvironment`提供有关应用程序当前正在运行环境的信息。

默认的`Configure`方法检查当前环境（通过`ASPNETCORE_ENVIRONMENT`环境变量设置）是否为 _Development_ 。如果是，则将中间件添加到请求管道中，在应用程序运行引发异常的情况下，将详细的错误消息输出给浏览器。中间件使用`IApplicationBuilder`的扩展方法（在本例中为`UseDeveloperExceptionPage()`）添加，这是构建请求管线的推荐方式。如果环境不是 _Development_（例如 _Production_），则会添加另一个中间件组件，用于记录有关失败请求的详细信息并重定向到 _Error_ 页面。对于一个动态的网站，建议在 _Error_ 页面显示更友好的消息，而不是堆栈跟踪。

接着添加可以提供静态文件（图像，脚本，样式表，PDF文件等）的中间件，再将 MVC 框架添加到应用程序中。这是至关重要的，因为 Razor 页面是MVC的一部分。默认项目模板为 MVC 请求设置了默认路由，如果您不打算构建 Razor 页面/ MVC / Web API 混合应用程序，可以省略这些路径。所以如果您愿意，可以将`UseMvc`方法改为如下：

```csharp
app.UseMvc();
```

## ConfigureServices方法

ASP.NET Core 广泛使用了依赖注入（DI）—— 一种有助于解耦代码的技术。组件或“服务”被表示为抽象，通常是接口，正如您已经在`Configure`方法中看到的`IApplicationBuilder`。在DI中，每个抽象都与一个实现（或“具体”）类型配对。这个配对存储在类似字典的结构中，当抽象作为参数传递给方法（特别是构造函数）时，实现类型就会被实例化并通过DI容器传入。`ConfigureServices`方法的主要目的是为应用程序注册所需的服务实现类型。它还用于配置与这些服务相关的其它选项。

如果在`Startup`类中已经添加了，则`ConfigureServices`方法在`Configure`方法之前被调用。这是有道理的，因为`Configure`方法可能会尝试使用需要事先注册的服务，以便可以解析它们。这是使用MVC的默认模板：

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.AddMvc();
}
```

`AddMvc`方法是一个包装，用于添加大量组件到服务集合，包括路由、模型绑定、缓存、视图引擎和MVC（应该是Razor页面）需要的其它组件。它还提供了一个与服务相关的配置选项。例如，`AddMvc`方法返回一个`IMvcBuilder`，它提供了多种配置选项：

![Mvc Options](/images/01-08-2017-13-57-55.png)

如果您想要更改 Razor 页面根文件夹，而不是默认的 _Pages_，那么您可以这样做：

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().AddRazorPagesOptions(options =>
    {
        options.RootDirectory = "/Content";
    });
}
```