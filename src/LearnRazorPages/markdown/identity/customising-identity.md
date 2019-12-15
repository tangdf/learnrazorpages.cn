
# Razor 页面自定义 Identity

在 Razor 页面应用程序的默认项目模板中，提供管理认证的代码是一个不错的开端。但是，有可能您想定制以满足应用需求。本文将讨论最常见的定制需求。

## 自定义用户注册

ASP.NET Identity 中的用户由`ApplicationUser`类表示。它的默认属性很少：`UserName`和`Email`。模板中的注册表单接收一个邮箱地址，并将其应用于`UserName`和`Email`属性。以下步骤演示如何让用户为其用户名提供不同的值：

1. _Register.cshtml.cs_ 文件中的`InputModel`类添加一个新属性`UserName`：
    ```csharp
    public class InputModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        ...

    ```

2. 更改`OnPostAsync`方法中的代码，以便为新属性分配用户名的值：
    ```csharp
    if (ModelState.IsValid)
    {
        var user = new ApplicationUser { UserName = Input.UserName, Email = Input.Email };
        ...
    ```

3. 在 _Register.cshtml_ 文件中更改注册表单以添加新的属性：
    ```html
    <form asp-route-returnUrl="@Model.ReturnUrl" method="post">
        <h4>Create a new account.</h4>
        <hr />
        <div asp-validation-summary="All" class="text-danger"></div>
        <div class="form-group">
            <label asp-for="Input.UserName"></label>
            <input asp-for="Input.UserName" class="form-control" />
            <span asp-validation-for="Input.UserName" class="text-danger"></span>
        </div>
        ...

    ```

## 将属性添加到ApplicationUser

您可能希望在注册时从用户那里获取更多的信息，而不仅仅是他们的电子邮件地址和用户名。您可以通过向`ApplicationUser`类添加属性来存储附加值，然后使用 [Entity Framework Core 迁移](http://www.learnentityframeworkcore.com/migrations) 更改应用于数据库，用于存储附加信息。以下步骤演示如何添加名字，姓氏和出生日期字段：

1. 为`ApplicationUser`类添加两个`string`属性和一个`DateTime`属性：
    ```csharp
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
    }
    ```

2. 打开程序包管理器控制台并键入以下命令：
    ```cmd
    PM> add-migration AddedFirstNameLastNameBirthDate
    ```
    
    这将创建一个迁移，在应用时，将修改数据库中`AspNetUsers`表的架构以容纳与添加的属性相关的额外数据。

3. 通过在程序包管理器控制台中输入以下命令来应用迁移：
    ```cmd
    PM> update-database
    ```
    
4. 将相应的属性添加到`InputModel`类（_Register.cshtml.cs_）以及相应的注解用来显示：

    ```csharp
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
    
    [Display(Name = "Date of birth")]
    public DateTime BirthDate { get; set; }
    ```
    
5. 更改 _Register.cshtml.cs_ 文件`OnPostAsync`方法中的代码，以将`Input`类中的值分配给`ApplicationUser`新属性：

    ```csharp
    if (ModelState.IsValid)
    {
        var user = new ApplicationUser {
            UserName = Input.UserName,
            Email = Input.Email,
            FirstName = Input.FirstName,
            LastName = Input.LastName,
            BirthDate = Input.BirthDate
        };
        ...
    ```
    
6. 最后，在 _Register.cshtml_ 中的Form添加相应的附加字段：

    ```html
    <div class="form-group">
        <label asp-for="Input.FirstName"></label>
        <input asp-for="Input.FirstName" class="form-control" />
        <span asp-validation-for="Input.FirstName" class="text-danger"></span>
    </div>
    <div class="form-group">
        <label asp-for="Input.LastName"></label>
        <input asp-for="Input.LastName" class="form-control" />
        <span asp-validation-for="Input.LastName" class="text-danger"></span>
    </div>
    <div class="form-group">
        <label asp-for="Input.BirthDate"></label>
        <input asp-for="Input.BirthDate" class="form-control" />
        <span asp-validation-for="Input.BirthDate" class="text-danger"></span>
    </div>
    ```
    
## 自定义密码选项

下列默认密码要求可能不符合您的需求:

*  密码必须至少为6位，最长为100个字符。
*  密码必须至少有一个非字母数字字符。
*  密码必须至少有一个小写字母（'a' - 'z'）。
*  密码必须至少有一个大写字母（'A' - 'Z'）。

可以通过`Startup`类的`ConfigureServices`方法中的选项更改这些默认值。大多数选项是布尔值，使用`true`或`false`。还可以指定最小长度和最小唯一字符数。选项名称一目了然：

```csharp
services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredUniqueChars = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

```

## 定制身份验证的资源。

最后，我介绍如何保护未经身份验证的用户资源。默认模板阻止用户访问 _Pages/Account/Manage_ 文件夹的内容，以及`AccountController`上的`Logout` Action 方法。这些受`Startup`类的`ConfigureServices`方法中所建立约定的保护：

```csharp
services.AddMvc()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AuthorizeFolder("/Account/Manage");
        options.Conventions.AuthorizePage("/Account/Logout");
    });

```

可以使用`AuthorizeFolder`方法添加其它约定来限制对文件夹及其所有内容的访问，或者`AuthorizePage`方法以逐个页面的方式限制访问，来保护其它资源；或者，使用`AuthorizeAttribute`来保护特定的页面。可以通过`[Authorize]`修饰 PageModel 类来实现此目的：

```csharp
[Authorize]
public class AboutModel : PageModel
{
...
```