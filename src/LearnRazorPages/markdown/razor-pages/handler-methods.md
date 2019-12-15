# Razor 页面处理方法


Razor 页面中的处理方法是根据请求自动执行的。Razor 页面框架使用使用命名约定选择适当的处理方法来执行。默认约定的工作原理是将请求的HTTP谓词与方法名称相匹配，这些方法使用“On”作为前缀：`OnGet()`、`OnPost()`、`OnPut()`等等，同时它们也有对等的异步名称：`OnPostAsync()`、`OnGetAsync()`等等。其实并不需要添加`Async`后缀，这种可选方式是为那些喜欢在异步代码的方法上包含`Async`后缀的开发人员提供的。

处理方法必须是`public`的，并且可以返回任何类型，尽管通常它们最有可能返回`void`（或者`Task`，如果是异步的）或者 [ActionResult](/razor-pages/action-results)。

以下示例演示示单个 Razor 页面文件中的基本用法：

```csharp
@page
@{
    @functions{
        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Get used";
        }

        public void OnPost()
        {
            Message = "Post used";
        }
    }
}
```


该页面的HTML部分包括一个使用`POST`方法的表单和一个超链接，超链接发起一个`GET`请求：

```html
<h3>@Message</h3>
<form method="post"><button class="btn btn-default">Click to post</button></form>
<p><a href="/handlerexample" class="btn btn-default">Click to Get</a></p>
```

第一次导航到页面时，显示“Get used”信息，因为请求使用的 HTTP `GET`方式，触发了`OnGet()`处理方法。

当点击“Click to post”按钮时，表单被提交，`OnPost()`处理方法被触发，最终显示“Post used”信息。

单击超链接会再次显示“Get used”信息。

## 指定处理方法

想象一下，如果页面上有很多表单，您怎么知道提交了哪一个表彰？Razor 页面包含“指定处理方法”的功能。下面的代码演示了在 Razor 页面顶部的代码块中声明多个处理方法（如果您使用的是这种方法，它们也可以放在 PageModel 类中)：

```csharp
@page 
@{

    @functions{
        public string Message { get; set; } = "Initial Request";

        public void OnGet()
        {

        }

        public void OnPost()
        {
            Message = "Form Posted";
        }

        public void OnPostDelete()
        {
            Message = "Delete handler fired";
        }

        public void OnPostEdit(int id)
        {
            Message = "Edit handler fired";
        }

        public void OnPostView(int id)
        {
            Message = "View handler fired";
        }
    }
}
```

框架的约定是将方法的名称必须追加到 "OnPost" 或 "OnGet"的后面，因为它们决定处理方法是否应该作为`POST`或`GET`请求的返回结果。下一步是将特定的表单动作与一个指定的处理方法相关联，通过为表单标签助手设置`asp-page-handler`属性值来实现：

```html
<div class="row">
    <div class="col-lg-1">
        <form asp-page-handler="edit" method="post">
            <button class="btn btn-default">Edit</button>
        </form>
    </div>
    <div class="col-lg-1">
        <form asp-page-handler="delete" method="post">
            <button class="btn btn-default">Delete</button>
        </form>
    </div>
    <div class="col-lg-1">
        <form asp-page-handler="view" method="post">
            <button class="btn btn-default">View</button>
        </form>
    </div>
</div>
<h3 class="clearfix">@Model.Message</h3>
```

上面的代码呈现为三个按钮，每个按钮都有自己的表单和`Message`属性的默认值：

![FormTagHelpers](https://www.mikesdotnetting.com/images/2017-05-19_22-10-01.png)

处理方法的名称作为查询字符串参数追加到表单的Action中： 

![Handlers](https://www.mikesdotnetting.com/images/2017-05-19_22-22-34.png)

当单击每个按钮时，与查询字符串值相关联的处理方法中的代码将被执行，每次都会更改显示信息。

![Handlers](https://www.mikesdotnetting.com/images/2017-05-19_22-43-02.png)

 

如果您不想在URL中包含查询字符串值，则可以使用[路由系统](/razor-pages/routing)，将“handler”添加为一个可选路由作为`@page`指令的一部分：

```html
@page "{handler?}"
```

处理方法的名称会被追加到URL中：

![Handler Routevalue](https://www.mikesdotnetting.com/images/22-05-2017-08-04-11.png)

## 处理方法中的参数

处理器方法可以被设计为接受参数：

```csharp
public void OnPostView(int id)
{
    Message = $"View handler fired for {id}";
}
```

参数名称必须与表单字段名称匹配才能自动绑定到值：

```html
<div class="col-lg-1">
    <form asp-page-handler="view" method="post">
        <button class="btn btn-default">View</button>
        <input type="hidden" name="id" value="3" />
    </form>
</div>
```

![Handler parameters](https://www.mikesdotnetting.com/images/22-05-2017-08-30-22.png)


或者使用[表单标签助手](/razor-pages/tag-helpers/form-tag-helper)的 `asp-route`属性作为URL的一部分来传递参数值，可以作为查询字符串的值或路由参数：

```html
<form asp-page-handler="delete" asp-route-id="10" method="post">
    <button class="btn btn-default">Delete</button>
</form>
```

You append the name of the parameter to the `asp-route` attribute (in this case "id") and then provide a value. This will result in the parameter being passed as a querystring value:

将参数的名称追加到`asp-route`属性后面（在本例中为“id”），然后提供一个值，参数作为查询字符串的值进行传递：

![Parameter as Query String](https://www.mikesdotnetting.com/images/22-05-2017-09-17-21.png)

或者扩展页面的路由定义，声明为可选参数：

```html
@page "{handler?}/{id?}"
```

This results in the parameter value being added as a separate segment in the URL:

参数值将在URL中作为单独的片段：

![Parameter as route segment](https://www.mikesdotnetting.com/images/22-05-2017-09-21-41.png)

## 处理同一表单中的多个操作（Action）

有些表单需要设计为满足多个可能的操作。在这种情况下，编写一些条件代码来确定应该采取哪些操作，或者编写单独的处理方法，然后使用[Formaction 标签助手](/razor-page /tag-helper /form-action-tag-helper)来指定在提交表单时执行的处理方法：

```html
<form method="post">
    <button asp-page-handler="Register">Register Now</button>
    <button asp-page-handler="RequestInfo">Request Info</button>
</form>
```

传递给该`page-handler`属性的值是去掉处理方法`OnPost`前缀和`Async`后缀后的名称：

```csharp
public async Task<IActionResult> OnPostRegisterAsync()
{
    //…
    return RedirectToPage();
}

public async Task<IActionResult> OnPostRequestInfo()
{
    //…
    return RedirectToPage();
}

```