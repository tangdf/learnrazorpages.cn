# Razor 页面 ActionResult

 

Razor 页面中的 ActionResult 通常用作[处理方法](/razor-pages/handler-methods) 的返回结果，并负责生成响应和相应的状态码。ActionResult 必须继承自抽象类`Microsoft.AspNetCore.Mvc.ActionResult`或实现了`Microsoft.AspNetCore.Mvc.IActionResult`接口。ASP.NET Core 包含三十多个`ActionResult`类，涵盖了广泛的需求，包括执行和返回 Razor 页面的内容、返回文件的内容、重定向到另一个资源或简单地返回一个特定的HTTP状态码等等。如果您有定制的需求，也可以编写自己的`ActionResult`类。

下面的代码演示了如何在简单的`OnGet`处理方法中使用`RedirectToPageResult`类，这会导致浏览器被重定向到指定的页面：

```csharp
public IActionResult OnGet()
{
 return new RedirectToPageResult("Index");
}
```

`OnGet`处理方法的返回类型是`IActionResult`，这意味着返回类型可以是任何在其继承层次结构中实现了`IActionResult`接口的类，换句话说，这意味着可能是整个框架内的任何`ActionReault`。一般来说，使用处理方法返回的类型应该尽可能具体，所以这个示例应该重构如下：

```csharp
public RedirectToPageResult OnGet()
{
 return new RedirectToPageResult("Index");
}
```

如果您的处理方法根据条件返回多种类型的 ActionResult，则需要扩展该方法的返回类型，`ActionResult`或者`IActionResult`。如本示例所示，如果`ModelState`无效则返回`PageResult`，如果有效则返回`RedirectToPageResult`（不同类型）：

```csharp
public IActionResult OnPost()
{
 if(!ModelState.IsValid)
 {
 return new PageResult();
 }
 // otherwise do some processing
 return new RedirectToPageResult("Index");
}
```

很多`ActionResult`都在 Razor 页面`PageModel`类上定义了相关的帮助方法，避免了对`ActionResult`类的`new`实例的需求，这样有助于简化代码。`Page()`方法返回一个`PageResult`，`RedirectToPage`方法返回一个`RedirecToPageResult`，所以最后一个例子可以简化为使用这些方法：

```csharp
public IActionResult OnPost()
{
 if(!ModelState.IsValid)
 {
 return Page();
 }
 // otherwise do some processing
 return RedirectToPage("Index");
}
```

下表列出了在 Razor 页面开发中使用的`ActionResult`类，以及它们返回的所有 HTTP 状态码，以及相关的辅助方法(如果提供了的话)。

| ActionResult | 帮助方法 | HTTP <br/> 状态码 | 描述 |
| --- | --- | --- | --- |
| ChallengeResult | Challenge | 401 | 用于身份验证。如果用户未认证成功，可以返回`ChallengeResult`。它返回一个401（Unauthorized）的 HTTP 状态码。 |
| ContentResult | Content | 200 | 返回一个字符串并在默认情况下`content-type`的响应头是`text/plain`；可以通过重载指定返回其它格式的`content-type`，例如 `text/html`、`application/json。|
| EmptyResult | | 200 | 这个ActionResult类型可以用来表示服务器端执行成功，但是没有任何返回值。 |
| FileContentResult | File | 200 | 从字节数组、流或虚拟路径返回文件。|
| FileStreamResult | | 200 | 从流中返回文件。 |
| ForbidResult | Forbid | 403 | 用于身份验证。该方法返回403（Forbidden）HTTP状态码。 |
| LocalRedirectResult | LocalRedirect <br/> LocalRedirectPermanent <br/> LocalRedirectPreserveMethod <br/> LocalRedirectPermanentPreserveMethod | 302 301 307 308 | <sup>1</sup> |
| NotFoundResult | NotFound | 404 | 返回一个HTTP 404（Not Found）状态码，表示找不到请求的资源。|
| PageResult | Page | 200 | 将处理并返回当前页面的结果。 |
| PhysicalFileResult | PhysicalFile | 200 | 从指定的物理路径返回文件。 |
| RedirectResult | Redirect <br/> RedirectPermanent <br/>  RedirectPreserveMethod <br/> RedirectPermanentPreserveMethod | 	302 301 307 308 |  将用户重定向到指定的URL，HTTP 状态码取决于指定的选项。默认是临时重定向（302）<sup>1<sup>。 |
| RedirectToPageResult | RedirectToPage <br/> RedirectToPagePermanent <br/> RedirectToPagePreserveMethod <br/> RedirectToPagePermanentPreserveMethod | 302 301 307 308 | 将用户重定向到指定的页面。 <sup>1<sup> |
| SignInResult | | | |
| SignOutResult | | | |
| StatusCodeResult | | | |
| UnauthorizedResult | | 401 | |

### 备注

 1. 重定向 ActionResult 可以指定重定向是否为永久性以及重定向时是否保留 HTTP 谓词。临时重定向状态码使用302和307表示，永久重定向状态码使用301和308表示。永久重定向通常是为搜索引擎和其它检索技术设计的，使用在`location`响应头中指定的新位置取代现有的索引项。

	如果您选择不保留原始请求所使用的 HTTP 谓词，则重定向请求会忽略原始请求使用 HTTP 谓词，直接使用GET方式。如果您想在后续的请求中保留原始的HTTP方法（或者提交的表单值），需要使用名称中带有`PreserveMethod`的方法，或者在直接使用`RedirectResult`方法中并将为对应的参数设置为`true`。

