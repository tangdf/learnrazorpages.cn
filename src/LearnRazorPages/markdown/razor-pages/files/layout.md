# 布局（Layout ）页面

大多数网站在多数页面甚至每个页面内都存在相同的内容，比如页眉、页脚和导航系统等，整个网站的脚本和样式也存在类似的问题。 在网站的每一页面都添加相同的标题，违反了DRY原则（Don't Repeat Yourself）， 如果您需要更改标题的外观，则需要编辑每个页面。 同样也存在其它问题，例如，如果您想升级客户端框架。虽然一些IDE提供了在多个文件中进行替换的工具，但是这不是一个可靠的解决方案。 这个问题的正确的解决方案是布局（Layout） 页面。

布局页面充当所有引用它的页面的模板。引用布局页面的页面被称为内容页面，内容页面不是完整的网页， 它们只包含从一页与另一页不同的内容。 
下面的示例代码演示了一个非常简单的布局页面：
```html
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="utf-8" />
        <title></title>
        <link href="/css/site.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        @RenderBody()..
    </body>
</html>
```

布局页面是对`RenderBody`方法的调用， 这是处理内容页的结果将被放置的地方。 内容页面通过页面的“Layout ”属性来引用布局页面，可以在内容页顶部的代码块中指定布局页的相对位置：
```csharp
@{
    Layout = "/_Layout";
}
```
布局页面通常被命名为_\_Layout.cshtml_，以下划线开头，防止它们被直接浏览。将路径赋给Layout属性时，扩展名是可选的。 标准做法是在_\_ViewStart.cshtml_文件中指定布局页面的路径，该文件会影响当前文件夹以及所有子文件夹的所有内容页面。

默认情况下，布局文件位于_Pages_文件夹中，也可以存储在应用程序文件夹结构中的任何位置。 如果您有 MVC 经验，您可能更希望创建一个_Shared_文件夹，并将布局移动此处。 使用_\_ViewStart_文件可以轻松地更新到新位置：
```csharp
@{
    Layout = "Shared/_Layout";
}
```

## Sections

布局页面中`RenderBody`方法的位置决定了内容页面输出的位置，还可以在布局页面内输出内容页面的其它内容，通过调用`RenderSection`方法来实现。下面的示例来自默认模板中的布局页面：

```csharp
@RenderSection("Scripts", required: false)
```

这个调用定义了一个名为“Scripts”的代码片段 —— 用于页面特定的脚本文件引用或JavaScript代码块，可以在关闭的`</ body>`标签之前输出内容。 第二个参数`required`规定内容页面是否必须提供指定的代码片段。 在这个例子中，`required`被设置为`false`，说明该部分是可选的。 如果该部分不是可选的，则每个引用布局页面的内容页面都必须使用`@section`语法提供这一代码片段：
```html
@section Scripts{
    // content here
}
```

在某些情况下，您可能希望将某个代码片段设置为可选，但是如果内容页面没有提供该代码片段，则需要输出默认的内容，可以使用`IsSectionDefined`方法：
```html
@if(IsSectionDefined("OptionalSection"))
{
    @RenderSection("OptionalSection")
}
else
{
    // default content
}
```

## 嵌套布局

布局页面可以嵌套，也就是说，指定布局页面的布局是完全合法的。 以下示例演示了包含头部和样式引用的主布局以及两个子布局页面（一个包含一个内容列，另一个包含两列，其中第二列包含一个代码片段）。 内容页面可以引用两个子布局页面中的任一个，仍然可以获得主布局文件提供的公共标签。
_\_MasterLayout.cshtml_
```html
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="utf-8" />
        <title></title>
        <link href="/css/site.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        @RenderBody()
    </body>
</html>
```

_\_SubLayout1.cshtml_
```html
@{
    Layout = "/_MasterLayout";
}
<div class="main-content-one-col">
@RenderBody()
</div>
```
_\_SubLayout2.cshtml_
```html
@{
    Layout = "/_MasterLayout";
}
<div class="main-content-two-col">
@RenderBody()
</div>
<div>
@RenderSection("RightCol")
</div>
```
