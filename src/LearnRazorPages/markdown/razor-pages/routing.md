# 路由


路由是将 URL 与 Razor 页面匹配的系统。与大多数以页面为中心的框架一样，ASP.NET Razor 页面中的主要路由系统基于将 URL 匹配到文件路径，从 Razor 页面根文件夹开始，默认情况下是 _Pages_ 。

## URL如何匹配

当一个 Razor 页面应用程序启动时，会构造 _Attribute Routes_ 集合（使用过 ASP.NET MVC 5 或 MVC Core 应该都熟悉），使用以 _Pages_ 文件夹为根目录的文件和文件夹路径作为路由模板。

默认的站点模板在根文件夹中包括4个页面：

```
Index.cshtml
About.cshtml
Contact.cshtml
Error.cshtml
```

集合中包括以下五个 _路由模板_ ：

```
""
"Index"
"About"
"Contact"
"Error"
```


默认情况下，路由模板是通过获取每个 [内容页面](/razor-pages#内容页面)  的虚拟路径生成的，然后在前面删除根文件夹名称，从末尾删除文件扩展名。

_Index.cshtml_ 被认为是所有文件夹中的默认文档，所以它定义了**两**个路由 —— 一个用没有扩展名的文件名表示，一个用空字符串表示。因此，您可以通过浏览`http://yourdomain.com`和`http://yourdomain.com/index`访问 _Index.cshtml_ 。

如果您创建一个 _Test_ 文件夹并向其中添加一个 _Index.cshtml_ 文件，则将增加以下两个路由模板定义：

```
"Test"
"Test/Index"
```

这两个路由将被映射到相同的虚拟路径：`/<root>/Test/Index.cshtml`。

但是，如果现在将  _Test.cshtml_ 页面文件添加到根页面文件夹并尝试浏览它，则会引发异常：

> AmbiguousActionException: Multiple actions matched.<br/>
> The following actions matched route data and had all constraints satisfied:<br/>
> 
> Page: /Test/Index <br/>
> Page: /Test

正如异常消息所述，将单个URL映射到多个 Action 或路由是错误的，框架无法知道要调用哪个页面。您可以通过向模板添加路由参数或约束来消除路由之间的歧义。

## 更改默认的根文件夹

您可以通过 [配置](/configuration) 更改 Razor 页面的根文件夹。以下示例将根文件夹从默认的  _Pages_ 更改为 _Content_ ：

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().AddRazorPagesOptions(options => {
        options.RootDirectory = "/Content";
    });
}
```

## 路由参数

假设您已经创建了一个博客，可能在根页面文件夹中有一个 _Post.cshtml_ 页面，在其中显示特定帖子的内容。在主页上提供了一系列链接到单个帖子，每个链接在 URL 中包含一个值以标识要从数据库检索的特定帖子。可以此值作为查询字符串值（`www.myblog.com/post?title=my-latest-post`），或者将其添加为路由参数（`www.myblog.com/post/my-latest-post`） —— URL中的`/my-latest-post`段在磁盘上没有匹配的文件。最后一段或  _参数_ 在 URL 中传递可以是任意一段数据。使用路由参数的方式有多种原因 ，更具可读性 —— 特别是如果您有多个参数值 ， 并且它更适合搜索引擎。

## 路由模板

路由参数在路由模板中定义，在 _.cshtml_ 文件中通过`@page`指令来表示。为了满足上面例子中的`title`的值，在 _Post.cshtml_ 文件顶部作如下所示声明：

```html
@page "{title}"
```

此路由创建的模板是`Post/{title}`。模板中的`{title}`部分是一个占位符，表示`post/`之后追加到 URL 中的任何值。模板定义必须以双引号出现，参数必须用引号引起来。

在这个例子中，值是必需的，所以您不能浏览到`/post`。必须在网址中提供值才能与“title”部分相匹配，否则将获得状态码404（ Not Found）。但是，可以通过在其后添加一个参数`?`来使参数为可选：

```html
@page "{title?}"
```

或者您也可以为参数提供默认值：

```html
@page "{title=first post}"
```

添加到路由的参数数量没有限制。在博客帖子的 UR L 包括发布的年份、月份和日期以及标题是很常见的。达到这一目的的路由定义如下：

```html
@page "{year}/{month}/{day}/{title}"
```

## 访问路由参数值

路由参数值存储在`RouteValueDictionary`中，可以通过`RouteData.Values`属性访问。您可以通过基于字符串的键来检索值：

```csharp
@RouteData.Values["title"]
```

这种方法的潜在问题是它依赖于通过字符串检索值，这些字符串很容易出现拼写错误，从而导致运行时错误。建议的替代方法是将这些值绑定到 [PageModel](/razor-pages/pagemodel) 上的属性。要做到这一点，需要向 PageModel 类添加适当数据类型的公共属性，并向`OnGet()`方法增加与路由具有相同名称和相应数据类型的参数：

```csharp
public class PostModel : PageModel
{
    public string Title { get; set; }

    public void OnGet(string title)
    {
        Title = title;
    }
}
```

将参数值分配给公共属性，在内容页面通过`Model`属性访问：

```html
@page "{title?}"
@model PostModel
@{

}
<h2>@Model.Title</h2>
```

推荐这种方法的关键原因是，可以从强类型以及IDE中的智能感知中提高效率：

![strong typing](/images/2017-06-09_20-39-08.png)


## 添加约束

约束是消除路由之间歧义的另一种手段。根据默认约定，路由参数值的唯一约束就是它必须提供。还可以通过 [数据类型和范围](/miscellaneous/constraints) 限制路由参数值。以下示例演示如何将参数值的约束改为整数类型：

```html
@page "{id:int}"
```

`id`的值必需提供，并且必须是整数。下一个示例演示了一个可选参数，如果提供了值，则该参数必须是小数类型：

```html
@page "{latitude:double?}"
```

下一个示例演示了`min`约束的使用，确保提供的值是一个整数，并且满足最小值10000。最小值在括号中提供：

```html
@page "{id:min(10000)}"
```

最后一个例子演示示了如何使用冒号指定多个约束：

```html
@page "{username:alpha:minlength(5):maxlength(8)}"
```

该模板指定用户名值是必需的（即不是可选的），必须由大小写字母（不能为数字或其它符号）组成，最小长度为5个字符，最大长度为8个字符。

## 友好路由

Razor 页面路由中的最后要补充是基于 ASP.NET Web Forms（另一种以页面为中心的开发模型）中的“友好路由（Friendly URLs）”功能，它可以绕过 URL 与文件路径和页面名称之间的紧密关系。

友好路由映射通过以下方法进行配置：在 [Startup](/startup) 的`ConfigureServices`方法中通过`AddPageRoute`方法中向`RazorPagesOptions.PageConventions`集合中添加路由：

```csharp
services
    .AddMvc()
    .AddRazorPages(options =>
    {
        options.Conventions.AddPageRoute("Archive/Post", "Post/{year}/{month}/{day}/{title}");
    });
```

这个方法有两个参数。第一个是没有扩展名的 Razor 页面文件的相对路径，第二个是映射到它的路由定义。

友好路由是附加的，也就是说它们不会取代现有的路由。仍然通过`/archive/post`导航到达上面的资源。所以可以添加一个“catchall”友好路由而不影响物理文件生成的路由。以下示例演示了一个路由，捕获未映射到任何物理文件的URL，通过 _Index.cshtml_ 文件来处理请求：

```csharp
services
    .AddMvc()
    .AddRazorPages(options =>
    {
        options.Conventions.AddPageRoute("index", "{*url}");
    }
```

例如，如果 _Index.cshtml_ 文件负责根据 URL 定位和处理Markdown文件，则可以这样做，就像本网站的情况一样。