# Razor 页面模型绑定

Razor 页面中的模型绑定是从HTTP请求获取值并将其映射到[处理方法](/razor-pages/handler-methods) 参数或 [PageModel](/razor-pages/pagemodel) 属性的过程。模型绑定可以减少开发人员从请求中手动提取值，然后逐个将其分配给变量或属性供以后处理的需要。这个工作是重复的，单调乏味，而且容易出错，主要是因为请求值通常是通过基于字符串的索引来检索的。

## 问题

为了说明模型绑定扮演的角色，创建一个新的 Razor 页面，将其命名为 _ModelBinding.cshtml_ 。将内容页面中的代码更改为以下内容：

```html
@page 
@model ModelBindingModel
@{
}

<h3>@ViewData["confirmation"]</h3>
<form class="form-horizontal" method="post">
    <div class="form-group">
        <label for="Name" class="col-sm-2 control-label">Name</label>
        <div class="col-sm-10">
            <input type="text" class="form-control" name="Name">
        </div>
    </div>
    <div class="form-group">
        <label for="Email" class="col-sm-2 control-label">Email</label>
        <div class="col-sm-10">
            <input type="text" class="form-control" name="Email">
        </div>
    </div>
    <div class="form-group">
        <div class="col-sm-offset-2 col-sm-10">
            <button type="submit" class="btn btn-default">Register</button>
        </div>
    </div>
</form>
```

它是一个标准的 HTML 表单，它接受一个`Name`和一个`Email`，在页面顶部有确认信息。表单完成并提交后，值将以键值对的形式发送到请求主体中，其中“Name”表示输入框的`name`属性，值是用户输入的值。如果您的浏览器支持开发者工具，可以切换到浏览器的“Network”标签：

![Posted values](/images/2017-07-22_16-52-57.png)

将以下处理方法的代码添加到在 _ModelBinding.cshtml.cs_ 文件的 PageModel 类中：

```csharp
public void OnPost()
{
    var name = Request.Form["Name"];
    var email = Request.Form["Email"];
    ViewData["confirmation"] = $"{name}, information will be sent to {email}";
}
```

这是在许多Web框架中处理服务器端代码中值的传统方式。通过基于字符串的索引访问当前 `Request`集合，然后将集合中的值分配给局部变量进行进一步处理。

在浏览器中启动页面，并在表单中输入一些值，然后提交它，应该看到确认消息中包含的值。
 
这种方法对于小型企业来说是可持续的，但是如果您正在处理大型表单，比如代表多个订单（包括包装和运输详细信息），那么分配代码就会变得非常繁琐。而且，由于开发工具没有提供代码提示或智能感知支持，这很容易输错，将`Request.Form["Email"]`变成`Request.Form["Emial"]`，因此，引入微小但具有破坏性的bug，并且很难在30或40个其它表单字段中找到。

## 将表单值绑定到处理方法参数

Razor 页面提供了两种利用模型绑定的方法。第一种方法是在处理方法中添加参数。参数以表单字段命名，并为预期数据提供相类的类型。为了看到这个方法的实际操作，删除`OnPost`处理方法中的赋值代码，并将两个参数添加到方法中：

```csharp
public void OnPost(string name, string email)
{
     ViewData["confirmation"] = $"{name}, information will be sent to {email}";
}
```

当表单被提交时，Razor 页面框架调用`OnPost`方法并且看到它有两个参数，它会提取与参数名称匹配提交表单的值，并自动将表单中的值分配给参数。不需要任何分配代码。

## 将表单值绑定到 PageModel 属性

前一种方法适用于不需要在参数所属处理方法之外的提供值。如果需要在处理方法之外的提供值(可能是在页面上显示)，则第二种方法更合适。这种方法涉及到在 [PageModel](/razor-page / PageModel) 中添加公共属性(如果您不想使用 PageModel 方法，则将其添加到`@function`块中)，然后使用`BindProperty`标记对它们进行修饰。为此，修改 PageModel 代码如下:

```csharp
public class ModelBindingModel : PageModel
{
    [BindProperty]
    public string Name { get; set; }
    [BindProperty]
    public string Email { get; set; }
    public void OnGet()
    {
    }

    public void OnPost()
    {
        ViewData["confirmation"] = $"{Name}, information will be sent to {Email}";
    }
}
```

请注意公共属性名称的大小写，与字符串中变量不一样。模型绑定本身不区分大小写。现在当运行这个页面时，结果和以前一样：

![Model binding to page model properties](/images/2017-07-22_17-18-09.png)

## 从GET请求中绑定数据

如果要绑定来自`GET`请求的数据（作为查询字符串附加到URL），则规则同样适用。绑定到处理程序方法参数是自动的，不需要额外的配置。但是，默认情况下，当使用`BindProperty`标记时，只考虑检索`POST`提交的值。`BindProperty`标记有一个`SupportsGet`属性，默认是`false`。必须将其设置`true`来支持对`GET`请求的模型绑定：

```csharp
public class ModelBindingModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Email { get; set; }
    [[BindProperty(SupportsGet = true)]
    public string Password { get; set; }
    public void OnGet()
    {
        ViewData["welcome"] = $"Welcome {Email}";
    }
}
```

<div class="alert alert-warning">

注意：创建支持`GET`的登录表单显然并不是一个好主意。表单值将出现在URL中，这可能是一个安全漏洞。

</div>

## 路由数据绑定

到目前为止，这些示例都介绍了模型绑定如何与表单值一起工作的， 它也适用于路由数据，这是 Razor 页面使用的路由系统的一部分。要测试它，请按照以下方式更改 _ModelBinding.cshtml_ 中的代码：

```html
@page "{id}"
@model ModelBindingModel
@{
}

<h3>Id = @ViewData["id"]</h3>
```

已经添加了一个`id`路由参数，并且改变`h3`标题的内容，显示`ViewData`中`id`的值。

接下来，从 PageModel 中删除公共属性，并将`int`类型`id` 参数添加到`OnGet`处理方法中，并在方法体中将其值赋给`ViewData`：

```csharp
public class ModelBindingModel : PageModel
{
    public void OnGet(int id)
    {
        ViewData["id"] = id;
    }
}
```

再次，模型绑定负责将路由中的值分配给处理程序方法参数，这也适用于 PageModel 上的公共属性，与表单值完全相同：

```csharp
public class ModelBindingModel : PageModel
{
    [BindProperty]
    public int Id { get; set; }

    public void OnGet()
    {
        ViewData["id"] = Id;
    }

    public void OnPost()
    {

    }
}
```

## 绑定复杂对象

到目前为止，已经介绍完如何使用模型绑定来填充简单的属性。随着表单字段数量的增长，PageModel 类将开始使用一长列属性，所有的属性都被`BindProperty`修饰，或者大量的参数被应用到一个处理器方法中。幸运的是，模型绑定也适用于复杂的对象。所以要绑定的属性可以被包装在一个对象中，该对象可以作为 PageModel 的一个属性或处理器方法的一个参数公开。这里有一个类Login，它代表了前面例子中的表单字段：

```csharp
public class Login
{
    public string Email { get; set; }
    public string Password { get; set; }
}
```

现在可以将其添加为 PageModel 类的一个属性：

```csharp
public class ModelBindingModel : PageModel
{
    [BindProperty]
    public Login Login { get; set; }
    public void OnGet()
    {
    }

    public void OnPost()
    {
        ViewData["welcome"] = $"Welcome {Login.Email}";
    }
}

```

或者可以根据使用需求将其作为`OnPost`方法的参数：

```csharp
public class ModelBindingModel : PageModel
{
    public void OnGet()
    {
    }

    public void OnPost(Login Login)
    {
        ViewData["welcome"] = $"Welcome {Login.Email}";
    }
}
```

## 简单集合绑定

下面示例代码演示了允许用户选择多个选项的表单。在这种情况下，要求用户指定他们喜欢哪个电影类别：

```html
<form class="form-horizontal" method="post">
    <div class="form-group">
        <label for="CategoryId" class="col-sm-2 control-label">Which types of film do you like? (Tick all that apply)</label>
        <div class="col-sm-10">
            <input type="checkbox" name="CategoryId" value="1"> Factual<br />
            <input type="checkbox" name="CategoryId" value="2"> Horror<br />
            <input type="checkbox" name="CategoryId" value="3"> Historical<br />
            <input type="checkbox" name="CategoryId" value="4"> SciFi<br />
            <input type="checkbox" name="CategoryId" value="5"> Comedy<br />
            <input type="checkbox" name="CategoryId" value="6"> Fantasy<br />
        </div>
    </div>
    <div class="form-group">
        <div class="col-sm-offset-2 col-sm-10">
            <button type="submit" class="btn btn-default">Submit</button>
        </div>
    </div>
</form>
```

表单包含多个复选框，每个复选框都具有相同的`name`属性值：CategoryId。收集提交值的正确类型是一个整数集合——数组、列表<int> '、' ICollection<int> '等等。绑定到处理方法的参数，并将提交的值传递给“ViewData”，如下所示：

```csharp
public void OnPost(int[] categoryId)
{
    ViewData["categoryId"] = categoryId;
}

```

如果没有提交值，`categoryId`将是`null`，同样`ViewData[“categoryId”]`也一样。因此必须检测是否为`null`(以及对相关类型的转换)：

```html
@if (ViewData["categoryId"] != null)
{
<h3>You selected the following categories: @string.Join(",", (int[])ViewData["categoryId"])</h3>
}
```

如果选择绑定到 PageModel 属性，可以在定义集合时初始化：

```csharp
public class ModelBindingModel : PageModel
{
    [BindProperty]
    public int[] CategoryId { get; set; } = new int[0];

    public void OnPost()
    {

    }
}

```

然后，可以使用`Any()`来检查集合是否已填充：

```
@if (Model.CategoryId.Any())
{
<h3>You selected the following categories: @string.Join(",", Model.CategoryId)</h3>
}
```

## 复杂集合绑定

模型绑定器还支持绑定到复杂对象的集合。下面的类表示地址簿中的联系人：

```csharp
public class Contact
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}
```

可能希望提供一种能够让用户一次提交多个联系人的表单。和前面的例子一样，可以通过一个处理方法参数来做到这一点：

```csharp
public void OnPost(List<Contact> contacts)
{
    // process the contacts
}
```

下面是如何设计表单来满足这种情况：

```html
<form class="form-horizontal" method="post">
    <table class="table table-striped">
            <tr>
                <th>First Name</th>
                <th>Last Name</th>
                <th>Email</th>
            </tr>
        @for (var i = 0; i < 5; i++)
        {
            <tr>
                <td>
                    <input type="text" name="[@i].FirstName" />
                </td>
                <td>
                    <input type="text" name="[@i].LastName" />
                </td>
                <td>
                    <input type="text" name="[@i].Email" />
                </td>
            </tr>
        }
    </table>
    <div class="form-group">
        <div class="col-sm-offset-2 col-sm-10">
            <button type="submit" class="btn btn-default">Submit</button>
        </div>
    </div>
</form>
```

![Multiple contacts](/images/2017-07-25_08-17-46.png)

复杂对象绑定的关键是方括号中的顺序索引，它被添加到表单字段的`name`属性中，例如`[0].FirstName`。下面的代码显示了表单中前三行的生成的HTML：

```html
<tr>
    <td>
        <input type="text" name="[0].FirstName" />
    </td>
    <td>
        <input type="text" name="[0].LastName" />
    </td>
    <td>
        <input type="text" name="[0].Email" />
    </td>
</tr>
<tr>
    <td>
        <input type="text" name="[1].FirstName" />
    </td>
    <td>
        <input type="text" name="[1].LastName" />
    </td>
    <td>
        <input type="text" name="[1].Email" />
    </td>
</tr>
<tr>
    <td>
        <input type="text" name="[2].FirstName" />
    </td>
    <td>
        <input type="text" name="[2].LastName" />
    </td>
    <td>
        <input type="text" name="[2].Email" />
    </td>
</tr>
```

在这个例子中，用于表单字段名称的格式是`[index].propertyname`。如果您喜欢，`parametername[index].propertyname`也可以正常运行，例如：

```html
@for (var i = 0; i < 5; i++)
{
    <tr>
        <td>
            <input type="text" name="Contacts[@i].FirstName" />
        </td>
        <td>
            <input type="text" name="Contacts[@i].LastName" />
        </td>
        <td>
            <input type="text" name="Contacts[@i].Email" />
        </td>
    </tr>
}
```

当表单被提交时，包含五个（在这个例子中）`Contact`对象的集合被实例化并且填充了提交的值。如果用户仅提供前三个联系人的值，则最后两个联系人的属性将设置为字符串的默认值——`null`。

绑定到 PageModel 属性时，同样的方法也可以工作。也可以使用 [Input标签助手](/razor-pages/tag-helpers/input-tag-helper) 的`asp-for`属性：

```html
@for (var i = 0; i < 5; i++)
{
    <tr>
        <td>
            <input type="text" asp-for="Contacts[i].FirstName" />
        </td>
        <td>
            <input type="text" asp-for="Contacts[i].LastName" />
        </td>
        <td>
            <input type="text" asp-for="Contacts[i].Email" />
        </td>
    </tr>
}
```

```csharp
public class ModelBindingModel : PageModel
{
    [BindProperty]
    public List<Contact> Contacts { get; set; } 

    public void OnPost()
    {
        // process the contacts
    }
}
```

## 防止Overposting或Mass Assignment攻击

将`BindProperty`属性添加到类中时，该类中的所有属性都会自动包含在模型绑定中。这可能不是需要的，特别是在使用实体框架模型类时。

例如，填充的实体上可能拥有一个`IsDeleted`属性以允许“软删除”（即，标记指定记录的状态，而不是从数据库中永久删除记录）。只有管理员允许设置这个属性，所以不应该在用户编辑表单中包含`IsDeleted`字段：

```html
<form class="form-horizontal" method="post">
    <input type="hidden" asp-for="ContactId">
    <div class="form-group">
        <label asp-for="Name" class="col-sm-2 control-label"></label>
        <div class="col-sm-10">
            <input type="text" class="form-control" asp-for="Name">
        </div>
    </div>
    <div class="form-group">
        <label asp-for="Email" class="col-sm-2 control-label"></label>
        <div class="col-sm-10">
            <input type="text" class="form-control" asp-for="Email">
        </div>
    </div>
    <div class="form-group">
        <div class="col-sm-offset-2 col-sm-10">
            <button type="submit" class="btn btn-default">Edit</button>
        </div>
    </div>
</form>
```

然而，对于一个有一定HTML经验的用户来说，个性表单（例如使用标准的浏览器开发工具）包含一个`IsDeleted`属性，并将其提交给服务器，这是轻而易举。该值将作为合法编辑操作的一部分进行处理。这就是所谓的Mass Assignment或Over Posting攻击。

出于这个原因，建议在使用复杂类型的模型绑定时要小心。如果它们包含不应该由非真实用户设置的属性，那么应该只包含可以设置的属性，或者作为 PageModel 上的单个属性，或者包装在 ViewModel 类中。

另外，最后一招，您可以使用`[BindNever]`标记，指定对应属性不需要模型绑定。
```csharp
[BindNever]
public string LastName { get; set; }
```

毫无疑问，将导致`LastName`属性永远不会被绑定。