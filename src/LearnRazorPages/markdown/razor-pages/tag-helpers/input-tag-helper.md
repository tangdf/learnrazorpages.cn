# Input 标签助手

Input 标签助手根据分配给它的 PageModel 属性生成相应的`name`和`id`属性值。 它还会根据属性的类型生成相应的`type`属性值。 标签助手还会输出一些属性，为客户端验证提供支持。

Input 标签助手有一个属性：

| 属性 | 描述 |
| ---  | ---  |
| `for`  | 要对当前 PageModel 属性的表达式，通常是 PageModel 某个属性的名称 |

## 备注

虽然它只有一个属性，但是 Input 标签助手非常强大。会检查传递给`for`属性类型的元数据，包括已经应用到属性的数据注解标记并生成相应的 HTML。

下面是具有各种属性类型和数据注释标记的类：

```csharp
public class Member
{
    public int PersonId { get; set; }
    public string Name { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [DataType(DataType.PhoneNumber)]
    public string Telephone { get; set; }
    [Display(Name="Date of Birth")]
    public DateTime DateOfBirth { get; set; }
    public decimal Salary { get; set; }
    [Url]
    public string Website { get; set; }
    [Display(Name="Send spam to me")]
    public bool SendSpam { get; set; }
    public int? NumberOfCats { get; set; }
}
```

然后将其作为  _Register.cshtml_ 页面 [PageModel](/razor-pages/pagemodel) 的属性：

```csharp
public class RegisterModel : PageModel
{
    [BindProperty]
    public Member Member { get; set; }

    public void OnGet()
    {
    }
}
```

模型的属性应用于 Razor 文件中的各种 Input 标签助手：

```html
<form method="post">
    <input asp-for="Member.PersonId" /><br />
    <input asp-for="Member.Name" /><br />
    <input asp-for="Member.Email" /><br />
    <input asp-for="Member.Password" /><br />
    <input asp-for="Member.Telephone" /><br />
    <input asp-for="Member.Website" /><br />
    <input asp-for="Member.DateOfBirth" /><br />
    <input asp-for="Member.Salary" /><br />
    <input asp-for="Member.SendSpam" /><br />
    <input asp-for="Member.NumberOfCats" /><br />
    <button>Submit</button>
</form>
```
生成的 HTML：
```html
<form method="post">
    <input type="number" data-val="true" data-val-required="The PersonId field is required." id="Member_PersonId" name="Member.PersonId" value="" /><br />
    <input type="text" id="Member_Name" name="Member.Name" value="" /><br />
    <input type="email" data-val="true" data-val-email="The Email field is not a valid e-mail address." id="Member_Email" name="Member.Email" value="" /><br />
    <input type="password" id="Member_Password" name="Member.Password" /><br />
    <input type="tel" id="Member_Telephone" name="Member.Telephone" value="" /><br />
    <input type="url" data-val="true" data-val-url="The Website field is not a valid fully-qualified http, https, or ftp URL." id="Member_Website" name="Member.Website" value="" /><br />
    <input type="datetime-local" data-val="true" data-val-required="The Date of Birth field is required." id="Member_DateOfBirth" name="Member.DateOfBirth" value="" /><br />
    <input type="text" data-val="true" data-val-number="The field Salary must be a number." data-val-required="The Salary field is required." id="Member_Salary" name="Member.Salary" value="" /><br />
    <input data-val="true" data-val-required="The Send spam to me field is required." id="Member_SendSpam" name="Member.SendSpam" type="checkbox" value="true" /><br />
    <input type="number" id="Member_NumberOfCats" name="Member.NumberOfCats" value="" /><br />
    <button>Submit</button>
    <input name="__RequestVerificationToken" type="hidden" value="CfDJ8I..." />
    <input name="Member.SendSpam" type="hidden" value="false" />
</form>
```

### 根据数据类型决定输入类型

Input 标签助手输出元素的`type`属性时，会依据属性的数据类型，尽可能使用 HTML5 类型来支持浏览器提供的功能。它们在不支持呈现的 HTML5 类型的浏览器中呈现为`type ="text"`：

| .NET 类型 | Input 类型 |
| --- | --- |
| bool |checkbox |
| byte, short, int, long | number			|
| decimal, double, float | text<sup>1</sup>	|
| string | text |
| DateTime | datetime-local	|

 1. 尽管这些数据类型的输入类型被设置为`text`，但仍然会对数值进行验证。

### 根据数据注释决定输入类型

应用于属性的数据注解标记也是 Input 标签助手`type`属性决定性的因素。 下表提供了`DataType`枚举值，括号中是等效注解标记（如果存在）：

| 注解 | Input 类型|
| --- | --- |
| EmailAddress (EmailAddressAttribute) | email |
| PhoneNumber (PhoneAttribute) | tel |
| Password | password |
| Url (UrlAttribute) | url	|
| Date, Time, DateTime, Duration | datetime-local |
| HiddenInput<sup>1</sup> | hidden |

其它的`DataType`枚举值（`CreditCard`、`Currency`、`Html`、`ImageUrl`、`MultilineText`、`PostCode`和`Upload`）都会应用`type="text"`。

 1. `HiddenInput`标记需要引用`Microsoft.AspNetCore.Mvc`，其它属性都在`System.ComponentModel.DataAnnotations`命名空间中。

### 验证支持

Input 标签助手还会输出与 ASP.NET 的 Unobtrusive 客户端验证框架（jQuery Validation的扩展框架）一起使用的`data`属性。 验证规则在`data-val-*`属性中指定，根据已应用于模型属性的数据类型和所有数据注释标记进行计算。

以下标记是为验证目的而设计的，会生成相应的`data-val`错误消息和其它属性：

*   Compare
*   MaxLength
*   MinLength
*   Range
*   Required <sup>1</sup>
*   StringLength

 1. 不可空属性被视为`Required`。

另外，以下注解标记会生成`data-val`属性：

*   EmailAddress / DataType.EmailAddress
*   Phone / DataType.PhoneNumer
*   Url / DataType.Url

有关验证属性的更多信息，请参见[校验](/razor-pages/validation)。