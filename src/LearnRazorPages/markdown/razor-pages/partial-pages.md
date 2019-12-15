# 局部页面

局部页面是 Razor 文件，其中包含 HTML 代码片段和服务器端代码，可以包含在任意数页面或布局中。局部页面可用于将复杂页面分解为更小的单元，从而降低复杂性，并允许团队同时处理不同的单元。

## 渲染局部页面

局部页面有多种方式包含在调用页面中。页面的`Html`属性包含一个`Partial()`方法：

```html
@Html.Partial("MenuPartial")
```

`Html`属性还提供其它3种渲染局部面面的方式：`PartialAsync`、`RenderPartial`和`RenderPartialAsync`。这两种方法以`Async`结尾都是用于渲染包含异步代码的部分，尽管呈现依赖于异步处理UI单元的首选方法是使用[视图组件](/razor-pages/view-components)。两个名称中包括`Render`的方法返回`void`，另外两个方法返回`IHtmlString`（原始HTML）。因此，这些`Render`方法必须在代码块中作为一个语句块来调用：

```html
@{ Html.RenderPartial("MenuPartial"); }
```

这些`Render`方法它们的输出直接写入响应，因此在某些情况下可能会提高性能。但是，在大多数情况下，这些改进可能不太重要的，因此建议您应尽量使用`Partial`和`PartialAsync`方法，减少 Razor 页面中的代码块的数量。

调用任何一种渲染方法都不会执行[Viewstart](/razor-pages/files/#_viewstartcshtml)文件。

## 局部页面命名和加载

不管使用的`Partial`方法还是`RenderPartial`都不是传递局部文件的路径。框架搜索预定位置查找传入的文件名：根页面文件夹和 _Views/Shared_文件夹。其中第二个位置是MVC应用程序中局部视图的默认位置。在[GitHub](https://github.com/aspnet/Mvc/issues/6604)有一个扩展搜索位置的问题，使其包含 _[Pages root]/Shared_。

可以在`StartUp`类的`ConfigureServices`方法通过[Razor 视图引擎选项](/configuration) 增加一个搜索的位置。下面的代码将 _Pages/Partials_文件夹添加到搜索路径，可以在其中存放局部文件并找到它们：

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().AddRazorOptions(options =>
    {
        options.PageViewLocationFormats.Add("/Pages/Partials/{0}.cshtml");
    });
}
```

局部文件没有任何特殊的命名约定。默认项目模板包含一个 _\_ValidationScriptsPartial.cshtml_ 局部视图。文件名中的前导下划线不起任何作用，但习惯上将任何不打算在直接在 MVC 中显示的 Razor 文件使用前导下划线，在 Razor 视图引擎使用这种规则的文件有： _\_ Layout .cshtml_，_\_ ViewStart.cshtml_ 等。原因在于在以前 ASP.NET Web Pages 框架引入Razor 时，网页框架被配置为明确禁止提供名称中带有下划线的文件。


## 强类型局部视图

和标准的 Razor 页面一样，局部页面也支持`@model`指令。所有的渲染方法都有重载的版本，需要传递一个模型。

下面的例子演示  _PartialDemo.cshtml_ 标准 Razor 页面和 PageModel文件内容：

 _PartialDemo.cshtml.cs_
 
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesTest.Pages
{
    public class PartialDemoModel : PageModel
    {
        public List<string> Animals = new List<string>();
        public void OnGet()
        {
            Animals.AddRange(new[] { "Antelope", "Badger", "Cat", "Dog" });
        }
    }
}
```

 _PartialDemo.cshtml_

```html
@page
@model PartialDemoModel
@{
}
@Html.Partial("_Partial1", Model.Animals)
```

最后，局部文件的内容 _\_ Partial1.cshtml_：

```html
@model List<string>
<h1>List of Animals</h1>
<ul>
    @foreach (var item in Model)
    {
        <li>@item</li>
    }
</ul>
```

PageModel 包含一个`Animals`属性，它是一个字符串集合。在`OnGet`处理方法中填充，然后传递给`Html.Partial`方法调用局部页面。

局部视图使用`@model`指令，指定该面的模型是一个字符串集合。内容被迭代并呈现为无序列表：

![endering partial with a model](/images/2017-07-29_09-59-01.png)

请注意，局部页面 **未**设有`@page`指令。它会使文件成为一个完整的 Razor 页面，将导致找不到对应的 PageModel 抛出 `NullReferenceException`异常。








