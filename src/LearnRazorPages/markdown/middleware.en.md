# Middleware in Razor Pages

## The Request Pipeline

When requests are made to a web application, they need to be processed in some way. A number of considerations need to be taken into account. Where should the request be directed or routed to? Should details of the request be logged? Should the application simply return the content of a file? Should it compress the response? What should happen if an exception is encountered while the request is being processed? Is the person making the request actually allowed to access the resource they have requested? How should cookies or other request-related data be handled?

Each of these processing actions are performed by separate components. The term used to describe these components is Middleware. Together, they form the request pipeline.

## Middleware in ASP.NET Core

In previous versions of ASP.NET, the components that affected the request pipeline were all bundled into one library, _System.Web.dll_, along with everything else you might or might not need to make your web application run.

In ASP.NET Core, request pipeline middleware is registered in the `Configure` method of the `Startup` class. The standard template includes the following code:

```
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

Various components are registered including error handling middleware, middleware for processing requests for static files (images, style sheets, script files, PDFs etc), authentication management middleware (if you enable authentication when creating your project), and the MVC framework. Each component is registered using an extension method on the `IApplicationBuilder` type.

The order in which the components are registered determines the order in which they are executed. Error handling middleware is registered first so that it is available to all code further along the pipleline where exceptions may be raised.

Middleware can either terminate the pipeline execution and return a response or it can pass control on to the next component. The Static File middleware terminates execution of the pipeline and sends the content of the requested static file in the response. Authentication and MVC middleware are not invoked when static files are requested. Other components pass execution on to the next registered component.

## Creating Middleware

Here are two examples of middleware. The first simply returns a response and is defined in the `Configure` method using the `IApplicationBuilder.Run()` method:

```
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.Run(async (context) =>
    {
        await context.Response.WriteAsync("All done");
    });
	...

}

```

This example terminates or short-circuits the pipeline. No other middleware components are executed. The second example follows the recommended pattern for creating middleware which is to create a separate class for it, and then to create an extension method on the `IApplicationBuilder` type to register it:

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

This middleware measures the time taken to process a request and then logs that information.

The class takes a `RequestDelegate` as a parameter to its constructor along with an `ILogger` . These are provided by the [dependency injection](/advanced/dependency-injection) system. The `RequestDelegate` represents the next middleware in the pipeline. The `Invoke` method contains the body of the middleware.

In this example, a `Stopwatch` instance is started. Then the request delegate is invoked, resulting in the rest of the pipeline being executed. The code after this line is executed once all subsequent middleware has executed. If the current request results is HTML the elapsed time is logged:

![Elapsed Time Middleware](/images/09-01-2018-12-46-18.png)

The extension method used to register the middleware is as follows:

```
public static class BuilderExtensions
{
    public static IApplicationBuilder UseElapsedTimeMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ElapsedTimeMiddleware>();
    }
}

```

This method is called in the `Configure` method in `Startup`:

```
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

The ElapsedTime middleware is registered _after_ the StaticFiles middleware, ensuring that requests for static files do not start the stop watch.