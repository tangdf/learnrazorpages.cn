# Razor 页面

所有的 Razor 文件皆以 _.cshtml_ 结尾，绝大多数的Razor文件都是可浏览的，客户端和服务端代码混合在一起，这些代码被处理后会以HTML的格式发送到浏览器。这些页面通常被称为“内容页面”。本节将深入研究内容页面及其相关的 [PageModel](/razor-pages/pagemodel) 文件。

## 内容页面

对于一个 Razor 内容文件来说，它必须具备三个特征：
- 文件名不能以下划线开头；
- 文件扩展名(后缀名)为 _.cshtml_；
- 文件中的第一行以 `@page` 开头。 

将 `@page` 指令作为第一行代码至关重要。如果没有这样做，文件将不会被视为一个Razor文件页面，如果您尝试浏览该页面，系统将提示无法找到该文件。在 `@page` 指令之前可以有空格，但是不能有任何其它字符，甚至是一个空的代码块。与 `@page` 指令同一行上唯一允许的其它内容是[路由模板](/razor-pages/routing#adding-constraints)。

内容页面可以指定一个[布局文件](/razor-pages/files/layout)，但这不是强制性的。布局文件可以选择性地包含代码块、HTML、JavaScript 和 Razor  代码。

## Razor 语法

内容页面主要由 HTML 组成，也可包括 [Razor 语法](https://www.mikesdotnetting.com/article/153/inline-razor-syntax-overview) ，允许在内容中包含可执行的C#代码。C#代码在服务器上执行，通常会在发送到浏览器的响应中包含动态内容。

## 单文件方式

尽管不推荐，但是可以开发仅依赖于内容页面的Razor页面应用程序。下面的例子展示一种类似于PHP或传统的ASP，具有脚本背景开发人员所熟悉的方法：
_Example.cshtml_：

```html
@page 
@{
    var name = string.Empty;
    if (Request.HasFormContentType)
    {
        name = Request.Form["name"];
    }
}

<div style="margin-top:30px;">
    <form method="post">
        <div>Name: <input name="name" /></div>
        <div><input type="submit" /></div>
    </form>
</div>
<div>
    @if (!string.IsNullOrEmpty(name))
    {
        <p>Hello @name!</p>
    }
</div>

```

Razor内容页面需要位于文件顶部的`@page`指令。`HasFormContentType`属性用于确定是否为表单请求，在Razor代码块中访问表单集合，并将相应的值赋给`name`变量。。

Razor代码块用`@{ }`符号表示，块内的内容是标准的C#代码。 

单行控制结构不需要代码块，可以用`@`符号作为前缀，正如前面例子中演示的`if`块。

要呈现C#变量或表达式的值，可以在`if`块使用`@`符号作为前缀来显示`name`变量。

下一个示例的功能与前面的示例相同，但是它使用`@functions`块来声明公有属性，并使用`BindProperty`标记进行装饰，确保属性参与模型绑定，从而避免了手动为变量分配表单值的需要。

_Example.cshtml_：

```html
@page

@functions {
    [BindProperty]
    public string Name { get; set; }
}

<div style="margin-top:30px;">
    <form method="post">
        <div>Name: <input name="name" /></div>
        <div><input type="submit" /></div>
    </form>
    @if (!string.IsNullOrEmpty(Name))
    {
        <p>Hello @Name!</p>
    }
</div>

```

这种方式是对前一种方式的改进，使用强类型，处理逻辑被限制为`@functions`块，页面将变得更加难以维护和测试。


## PageModel 文件

开发 Razor 页面应用程序的推荐方法是将内容页面中的服务器端代码的数量降到最低。任何与用户输入或数据处理相关的代码都应该放在 [PageModel](/razor-pages/pagemodel) 文件中，与其关联的内容页面共享一个一对一的映射，它们甚至共享相同的文件名，尽管在最后还有一个额外的_.cs_文件，以表明它们实际上是C#类文件。

下面的代码展示了使用 PageModel 的 _Example.cshtml_ 文件：

_Example.cshtml_：

```html
@page
@model ExampleModel

<div style="margin-top:30px;">
    <form method="post">
        <div>Name: <input asp-for="Name" /></div>
        <div><input type="submit" /></div>
    </form>
    @if (!string.IsNullOrEmpty(Model.Name))
    {
        <p>Hello @Model.Name!</p>
    }
</div>

```

这是PageModel类的代码：

_Example.cshtml.cs_：

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPages.Pages
{
    public class ExampleModel : PageModel
    {
        [BindProperty]
        public string Name { get; set; }
    }
}

```

PageModel 类定义了与前一个示例中相同的属性，并且它使用`BindProperty`标记进行修饰。内容页面不再有`@functions`块，但是它现在包含了一个`@model`指令，指定`ExampleModel`是页面的模型。还在页面中启用[标签助手](/razor-pages/tag-helpers/)，进一步利用编译时类型检查。

项目默认生成了与PageModel文件相匹配的内容页面，这是推荐方式。但是，了解如何在没有 PageModel 的情况下如果何使用内容页面也是非常有用的。