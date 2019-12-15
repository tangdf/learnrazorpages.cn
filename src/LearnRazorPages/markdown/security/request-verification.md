# 请求验证


ASP.NET Razor 页面中的请求验证防止可能的[跨站点请求伪造攻击（Cross Site Request Forgery）](https://www.owasp.org/index.php/Cross-Site_Request_Forgery_(CSRF))的机制，也称为 XSRF 和  CSRF 缩写。在 CSRF 攻击过程中，恶意用户将使用经过身份验证的用户凭据在网站上执行某些操作，以获取利益。

用来说明这种类型攻击的典型例子涉及网上银行业务。当在线登录您的银行账户时，浏览器会收到一个身份验证Cookie，然后在每次提交请求时都会将其提交给银行站点，以保持登录状态。身份验证Cookie具有预定的使用期限。它可能是基于会话的，也就是说，在您关闭网上银行的浏览器选项卡而不使用银行应用程序的注销功能，并且没有关闭浏览器之后的一段时间内，它可能还是有效的。

当这个Cookie仍然有效时，当您浏览另一个站点时，它可能会向您的银行站点发起表单提交，将您的帐户转到其它账户。此表单提交通过您的Cookie进行身份验证，所以银行站点接收此交易，因为它无法验证请求来自何处。

ASP.NET 框架为这种攻击提供的预防机制，验证对 Razor 页面的任何请求是否来自同一站点上的表单。

这是通过在每个表单的末尾注入一个`__RequestVerificationToken`隐藏的表单字段来实现的，该字段包含一个加密的值，并将对应的值传递给发送给表单请求的Cookie。当 ASP.NET Core 处理POST请求时，验证这两个项目及其值是否存在。如果验证失败，框架将返回一个 400 的HTTP状态码，表示一个错误的请求。

## 禁用验证

这种行为在框架中默认启用的。但是，可以关闭防伪令牌验证。可以在`Startup`类的`ConfigureServices`方法全局禁用：

```csharp
services.AddMvc().AddRazorPagesOptions(options =>
{
    options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
});
```

或者，通过添加`IgnoreAntiforgeryTokenAttribute`到相关的 PageModel 类来绕过检查：

```
[IgnoreAntiforgeryToken(Order = 1001)]
public class IndexModel : PageModel
{
    public void OnPost()
    {

    }
}
```

`ValidateAntiForgeryToken`标记默认的`Order`属性为1000，因此`IgnoreAntiforgeryToken`标记需要一个更高的序号。

或者，在全局上关闭令牌验证，然后根据情况选择性地在某一个案例中应用令牌验证，通过使用`ValidateAntiForgeryToken`标记来修饰适当的页面模型类(使用相同的`Order`值):

```csharp
[ValidateAntiForgeryToken(Order = 1000)]
public class IndexModel : PageModel
{
    public void OnPost()
    {

    }
}
```

<div class="alert alert-note">

`ValidateAntiForgeryToken`属性在其名称中具有大写字母“F”，而`IgnoreAntiforgeryToken`具有小写字母“f”。这是故意设计成这样的。

</div>


选择禁用防伪令牌验证不会阻止隐藏字段或 Cookie 的生成，它所做的只是跳过验证过程。一般来说，没有理由这样做。如果您希望您的站点接受来自外部站点的 POST 请求，建议的解决方案是使用Web API控制器。