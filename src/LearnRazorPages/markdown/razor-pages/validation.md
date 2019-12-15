# Razor 页面用户输入校验

当处理用户提供的值时，需要确保传入的值属于预期的数据类型，位于允许的范围内，并且需要有相应的值。这个过程称为输入校验。

在Web应用程序中有两个地方校验用户输入：在浏览器中使用客户端脚本或浏览器的内置数据类型验证；或者在服务端处理。但是，应该只将客户端验证视为对用户的一种体验人优化，因为它很容易绕过。

在构建 Razor 页面的MVC框架中，包括强大健壮的校验框架，该框架可以在客户端**和**服务器上对提交的模型属性进行操作。

验证框架中的关键角色是：

* 数据注解标记
* 标签助手
* jQuery Unobtrusive Validation
* ModelState

## 数据注解标记

验证框架的主要构建块是从`ValidationAttribute`中继承的一组标记。这些标记大部分驻留在`System.ComponentModel.DataAnnotations`命名空间中。

| 标记                  | 描述                                       |
| ------------------- | ---------------------------------------- |
| `Compare`           | 用于指定值与另一个表单域的进行比较，应该相等性                  |
| `MaxLength`         | 设置可接受的最大字符数/字节数/项目数                      |
| `MinLength`         | 设置可接受的最小字符数/字节数/项目数                      |
| `Range`             | 设置范围的最小值和最大值                             |
| `RegularExpression` | 根据指定的正则表达式检查值                            |
| `Remote`            | 针对服务器端资源（如数据库检查）启用客户端验证，如查看用户名是否已被使用     |
| `Required`          | 指定必须为此属性提供一个值。请注意，非空值类型（例如`DateTime`和数值）默认情况下被视为已经存在值，不需要将此标记应用于它们 |
| `StringLength`      | 设置允许的最大字符串字符数                            |

除了`Remote`标记之外，所有其它标记都会在客户端和服务器上进行验证。`Remote`标记还有不同于其它标记，因为它不属于`DataAnnotations`名称空间，位于`Microsoft.AspNetCore.Mvc`命名空间中。

<div class="alert-info" alert="">

在`DataAnnotations`命名空间中还有许多其它的标记继承自`DataTypeAttribute`，包括`Phone`、`EmailAddress`、`Url`、`CreditCard`等等。这些不构成验证框架的一部分，但是，这些标记会影响与已应用的模型属性相关联`input`输入框`type`属性的渲染。
[Inpu标签助手](/razor-pages/tag-helpers/input-tag-helper) 将根据标记的数据类型呈现对应的HTML5类型，以利用浏览器提供的支持特性，_其中包括浏览器特定类型校验_。这个验证在各个浏览器中表现不一致，不能被依赖，如果您想验证一个数据类型，应该实现自定义的解决方案。

</div>

标记应用于提交数据模型的属性——通常是 PageModel 或 ViewModel：

```csharp
public class UserModel : PageModel
{
    [BindProperty]
    [Required]
    [MinLength(6)]
    public string UserName { get; set; }

    [BindProperty]
    [Required, MinLength(6)]
    public string Password { get; set; }

    [BindProperty, Required, Compare(nameof(Password))]
    public string Password2 { get; set; }

    ...
```

每个标记可以单独声明，也可以作为逗号分隔列表或两者的混合。

## 客户端校验

客户端校验支持由Microsoft开发的jQuery Unobtrusive Validation库提供。它与标签助手输出的特殊 HTML5 属性`data-*`结合运行。要看看这是如何工作，这是一个使用简单标签助手的表单，上面有一些属性：

```html
<form method="post">
    <div>
        <input asp-for="UserName" />
        <span asp-validation-for="UserName"></span>
    </div>
    <div>
        <input asp-for="Password" />
        <span asp-validation-for="Password"></span>
    </div>
    <div>
        <input asp-for="Password2" />
        <span asp-validation-for="Password2"></span>
    </div>
    <div>
        <input type="submit" />
    </div>
</form>
```


这是它输出的HTML：

```html
<div>
    <input type="text" data-val="true" data-val-minlength="The field UserName must be a string or array type with a minimum length of &#x27;6&#x27;." data-val-minlength-min="6" data-val-required="The UserName field is required." id="UserName" name="UserName" value="" />
    <span class="field-validation-valid" data-valmsg-for="UserName" data-valmsg-replace="true"></span>
<div>
    <input type="text" data-val="true" data-val-minlength="The field Password must be a string or array type with a minimum length of &#x27;6&#x27;." data-val-minlength-min="6" data-val-required="The Password field is required." id="Password" name="Password" value="" />
    <span class="field-validation-valid" data-valmsg-for="Password" data-valmsg-replace="true"></span>
</div>
<div>
    <input type="text" data-val="true" data-val-equalto="&#x27;Password2&#x27; and &#x27;Password&#x27; do not match." data-val-equalto-other="*.Password" data-val-required="The Password2 field is required." id="Password2" name="Password2" value="" />
    <span class="field-validation-valid" data-valmsg-for="Password2" data-valmsg-replace="true"></span>
</div>
<div>
    <input type="submit" />
</div>
```

通过将`data-val`属性设置为`true`来激活验证。添加了各种其它的`data-val-*`属性，作为标记助手渲染的一部分，以指定所需的验证类型和错误消息，错误信息可以作为标记声明的一部分：

```csharp
[Compare(nameof(Password)), ErrorMessage ="Make sure both passwords are the same")]
public string Password2 { get; set; }
```

```html
<input type="text" 
    data-val="true" 
    data-val-equalto="Make sure both passwords are the same" 
    data-val-equalto-other="*.Password" 
    data-val-required="The Password2 field is required." 
    id="Password2" name="Password2" value="" />
```

必须在包含表单的页面中包含jQuery Unobtrusive Validation库，让客户端验证生效。最简单方式，通过在页面中包含  _ValidationScriptsPartial.cshtml_ 文件：

```html
@section scripts{
    @await Html.PartialAsync("_ValidationScriptsPartial")
}
```

最后，需要[Validation Message标签助手](/tag-helpers/validation-message-tag-helper)或[Validation Summary 标签助手](/tag-helpers/validation-summary-tag-helper)提供要显示的错误消息的位置。如果没有这些，任何提交校验失败的表单都不会成功，但是又没有提示显示任何错误来说明原因，因此会让用户感到困惑。

## 服务端校验

如果表单中没有包含  _ValidationScriptsPartial.cshtml_ ，或者如果不使用标签助手为表单控件生成HTML，则客户端验证将不会发生。

还有其它一些方法可以规避客户端验证：

* 使用浏览器的开发者工具，改变 `data-val="true"` 为 `data-val="false"` 
* 将表单副本保存到桌面并删除验证脚本
* 使用Postman，Fiddler或Curl直接提交表单值 
* 等等 

由于绕过客户端验证非常容易，因此应该将服务器端验证作为验证框架的一部分。一旦属性值被绑定，框架将查找这些属性的所有校验标记并执行它们。任何失败都会导致该条目被添加到存储验证错误的`ModelStateDictionary`类似字典的结构中。在 PageModel 类中是可用的`ModelState`，它具有一个`IsValid`属性，如果任何校验测试失败，则返回`false`：

```csharp
public IActionResult  OnPost()
{
    if (ModelState.IsValid)
    {
        // do something
        return RedirectToPage("Contact"));
    }
    else
    {
        return Page();
    }
}
```

上面的代码片段说明了在`OnPost`处理方法中处理验证的最常见模式——访问`ModelState`的`IsValid`属性，如果它返回`true`，则处理表单，否则重新显示表单，让框架负责从`ModelState`中提取错误消息，并将其传递给[Validation Message标签助手](/tag-helpers/validation-message-tag-helper)或[Validation Summary 标签助手](/tag-helpers/validation-summary-tag-helper)。