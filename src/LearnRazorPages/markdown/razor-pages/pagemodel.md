# Razor 页面 PageModel



PageModel 类型的主要目的是为了将页面的UI层（.cshtml _视图_ 文件）和处理逻辑层之间提供清晰的分离。这种分离有很多优点：

*   降低了UI层的复杂性，使其更容易维护；
*   有利于自动化单元测试；
*   为团队提供了更大的灵活性，可以将 Web 视图与业务逻辑明确分工到不同的开发人员；
*   鼓励开发精简、可重用的代码单元实现特定的功能，这有助于维护和可伸缩性(例如，为了满足将来更多的需求，可以很容易将应用程序的代码库添加到其中)。

在添加新项时，选择“Razor页面”选项时，会创建PageModel类：

![Razor Page with page model](/images/13-07-2017-08-19-45.png)


PageModel 类是在一个单独的类文件中声明的——扩展名为 _.cs_ 的文件。PageModel 类放置在与页面相同的[命名空间](/miscellaneous/namespaces) 中，默认情况下遵循该模式：`<默认命名空间>.<根文件夹名称>`，以页面文件名和“Model”作为后缀作为类名。例如： _About.cshtml_ 的 PageMode 的名称为 `AboutModel`，在指定的文件夹中生成 _About.cshtml.cs_ 。


就其功能而言，PageModel 类是 _Controller_ 和 _ViewModel_ 的合并。


## 控制器

控制器与很多涉及到应用程序表示层的设计和架构模式有关，它们可以在模型 - 视图 - 控制器（MVC）模式、前端控制器、应用程序控制器和页面控制器模式中找到。Razor 页面是页面控制器模式的一个实现。

页面控制器模式的特点是在页面与其控制器之间存在一对一的映射关系。在页面控制器模式中，控制器的作用是接受来自页面请求的输入，以确保所有请求操作反馈到数据模型，然后确定用于生成页面的正确视图。


## ViewModel

ViewMode 是 Presentation Model 设计模式的一个实现，是一个独立的类，表示特定“视图”或页面的数据和行为。ViewModel 模式在 MVC 应用程序开发中得到广泛应用，主要表示数据，通常很少有行为。Razor 页面 PageModel 的 ViewModel 是相似的，只表示要在页面上显示的数据。


## 默认模板

下面的代码演示了当您使用" Razor 页面"选项向 Razor 页面应用程序添加新页面时为每个文件生成的默认内容：
(Index.cshtml)

```html
@page
@model IndexModel
@{
}
```

(Index.cshtml.cs)

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LearnRazorPages.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
```


PageModel 类通过`@model`指令提供给视图文件。生成的 PageModel 类继承自`Microsoft.AspNetCore.Mvc.RazorPages.PageModel`，它包括很多属性，方便您处理与HTTP请求相关的各种操作，例如`HttpContext`、`Request`，`Response`、`ViewData`、`ModelState`和`TempData`。它还包括一系列方法，方便您返回指定的[响应结果](/razor-pages/action-results)，包括返回另一个 Razor 页面、文件、JSON、字符串或重定向到另一个资源。


## 请求处理

在 PageModel 中的请求处理是在[处理方法（handler methods）](/razor-pages/handler-methods)中执行的，类似于 ASP.NET MVC 控制器上的 Action 方法。按照约定，处理方法选择的原则是请求使用的HTTP谓词与处理名称相匹配，处理方法使用`On<verb>`模式，并使用`Async`结尾来表示该方法的是异步执行的。`OnGet`、`OnGetAsync`方法被用于GET请求，`OnPost`、`OnPostAsync`方法被用于POST请求。如果您想创建一个完整的 REST-ful 应用程序，所有其它的 HTTP 谓词（PUT，DELETE等）也是支持的。

匹配原则仅根据方法的名称，返回类型和参数均未考虑在内。处理方法的唯一其它约定是它必须是`public`的。


[指定处理方法](/razor-pages/handler-methods#指定处理方法)允许您为特定的 HTTP 谓词指定多个备选方法。如果您的页面包含多个表单，并且每个表单都需要执行不同的流程，则可能需要使用这种方式。


## 属性和方法

应用到 PageModel 类的属性和方法都可以在Razor页面的`Model`属性上获取。属性可以是简单的类型，比如`string`、`int`、`DateTime`等，也可以是复杂的类，也可以是组合。如果某个页面设计为将新产品添加到数据库，那么可能拥有以下一系列属性：

```csharp
public string Name { get ; set; }
public SelectList Categories { get; set; }
public int CategoryId { get; set; }
```

还可以将属性或方法添加到 PageModel 中，负责显示的格式化，以最大限度地减少添加到 Razor 页面中的代码量。下面的示例演示了如何使用属性来格式化计算结果：

```csharp
public List<OrderItems> Orders { get; set; }
public string TotalRevenue => Orders.Sum(o => o.NetPrice).ToString("f");
```

那么在 Razor 页面将只需要使用`@Model.TotalRevenue`即可显示所有销售的总数到小数点后两位，否则需要在 HTML 中进行 LINQ 计算。


添加到 PageModel 的属性还可以以强类型的方式开发表单，从而减少运行时错误的可能性。例如，它们可用于[Label 标签](/razor-pages/tag-helpers/label-tag-helper)和[Input 标签助手](/razor-pages/tag-helpers/input-tag-helper) 的`for`属性。