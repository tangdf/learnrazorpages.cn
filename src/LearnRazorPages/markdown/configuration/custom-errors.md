# 自定义错误页面配置


当访问者试图请求站点上不存在的页面，或执行服务器端代码时发生未处理的异常时，Web服务器会向浏览器返回一个HTTP状态码，表示发生了错误。作为一个网站开发者，当发生这种情况时，有两种选择：让浏览器显示其默认通知页面：

![Browser 404 page](/images/404.png)

或者控制并选择显示自定义信息。

## 状态码页面中间件

ASP.NET 团队已经包含了用于管理 400 和 500 范围内 HTTP 错误的中间件。它被叫做`StatusCodePagesMiddleware`，可以在[Microsoft.AspNetCore.Diagnostics package](https://github.com/aspnet/Diagnostics/blob/dev/src/Microsoft.AspNetCore.Diagnostics/StatusCodePage/StatusCodePagesMiddleware.cs)中找到。

在`Startup`类`Configure`的方法中，可以使用多种扩展方法将`StatusCodePagesMiddleware`注册为在管线的一部分的。最基本的用法如下：

```csharp
app.UseStatusCodePages();
```

当在指定范围内发生错误时，此用法会产生一个带有默认信息的纯文本响应：

![404 not found error page](/images/24-01-2018-08-13-17.png)

![404 not found response](/images/24-01-2018-08-10-24.png)

`UseStatusCodePages`方法的重载可提供更多控制应用程序如何响应错误，包括设置响应的内容类型和正文的方法：

```csharp
app.UseStatusCodePages("text/html", "<h1>Error! Status Code {0}</h1>");
```

![Use Status Code Pages](/images/24-01-2018-08-54-01.png)

同时，还有其它一些扩展方法可以简化更多的事情。比如`UseStatusCodePagesWithRedirects`和`UseStatusCodePagesWithReExecute`。这两个都使用创建的自定义页面来生成响应，因此可以完全控制其内容。下面的示例是一个简单的 Razor 页面，让访问者知道他们请求的页面不存在：

```html
@page
@{
    ViewData["Title"] = "No such page";
}
<h1>404 Not found</h1>
<p>No such page exists at this location.</p>
```

此代码位于 _Pages/Errors_ 文件夹中  _404.cshtml_ 文件内。

`UseStatusCodePagesWithRedirects`方法接受一个表示自定义错误页面路径字符串，该字符串还可以包含一个占位符`{0}`，该占位符将由状态码填充：

```csharp
app.UseStatusCodePagesWithRedirects("/errors/{0}");
```

当发生错误时，用户被重定向到指定的位置，在这种情况下，占位符被中间件填充为： `/errors/404`。

`UseStatusCodePagesWithReExecute`方法使用备用路径（页面）重新执行请求管线，还是使用一个占位符作为字符串：

```csharp
app.UseStatusCodePagesWithReExecute("/errors/{0}");
```

## 重定向或重新执行？

从用户的角度来看，使用哪种方法几乎没有什么不同，两者都会显示指定的页面。唯一可见的区别是浏览器中显示的URL。`WithRedirects`方法导致浏览器地址栏中显示的URL不同于请求的URL。

这是Chrome中的网络输出，用于请求不存在的页面：

![With Redirects](/images/24-01-2018-09-54-55.png)

当请求`/nonexistent-page`页面时，会返回一个302状态码，`location`响应头的值是`/errors/404`，导致浏览器重定向一个新的请求。这个请求是成功的，由200状态码表示。现在，浏览器显示的地址是错误页面的地址，而不是最初请求的地址：

![WithRedirects changes URL](/images/24-01-2018-11-06-30.png)

使用`WithReExecute`选项，原始的HTTP状态代码将与浏览器地址栏中的URL一样保留。

![With ReExecute retains URL](/images/24-01-2018-11-03-47.png)

但是响应的内容来自您指定的替代页面：

![With ReExecute](/images/24-01-2018-11-02-22.png)

`WithReExecute`选项提供一个更好的体验：访问者可以查看浏览器地址栏，查看他们提供的 URL 是否存在明显错误。


## SEO 注意事项

如果搜索引擎优化对您很重要，那么`WithReExecute`选项更胜一筹。让搜索引擎知道它尝试检索的不正确的URL不存在的最好方法是使用`404 Not Found`状态码进行响应。如果您提供`302 Found`然后是`200 OK`响应，搜索引擎会在其索引中保留不正确的网址，最后可能会在搜索结果中提供。

### 相关内容
*   [中间件](/middleware)
*   [配置](/configuration)  