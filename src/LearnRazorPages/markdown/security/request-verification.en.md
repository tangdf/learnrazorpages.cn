# Request Verification

Request Verification in ASP.NET Razor Pages is a mechanism designed to prevent possible [Cross Site Request Forgery](https://www.owasp.org/index.php/Cross-Site_Request_Forgery_(CSRF)) attacks, also referred to by the acronyms XSRF and CSRF. During a CSRF attack, a malicious user will use the credentials of an authenticated user to perform some action on a web site to their benefit.

The canonical example used to illustrate this type of attack involves online banking. When you log into your bank account online, your browser receives an authentication cookie which is then passed back to the banking site each time you make a request, keeping you logged in. The authentication cookie will have a predetermined life. It may be session-based, which means it could be valid for a period of time after you have closed the online banking's browser tab without using the banking application's log out feature and not having closed your browser.

While this cookie is still valid, you find yourself on another site that initiates a form post to your banking site that makes a transfer from your account to another. This form post is authenticated by your cookie and the banking site honours the transaction because it failed to verify where the request came from.

The prevention mechanism provided by the ASP.NET framework for this type of attack involves verifying that any `POST` request made to a Razor page originates from a form on the same site.

This is achieved by injecting a hidden form field named `__RequestVerificationToken` at the end of every form with an encrypted value, and passing the same value to a cookie which is sent with the form request. The presence of both of these items and their values are validated when ASP.NET Core processes a POST request. If verification fails, the framework returns an HTTP status code of 400, signifying a Bad Request.

## Opting Out

This behaviour is baked into the framework. However, it is possible to turn off antiforgery token validation. This can be done globally in the `ConfigureServices` method in `Startup`:

```
services.AddMvc().AddRazorPagesOptions(o =>
{
    o.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
});

```

Or you can bypass the checks by adding the `IgnoreAntiforgeryTokenAttribute` to the relevant page model class:

```
[IgnoreAntiforgeryToken(Order = 1001)]
public class IndexModel : PageModel
{
    public void OnPost()
    {

    }
}

```

The `ValidateAntiForgeryToken` attribute which is applied by default has an order of 1000, therefore the `IgnoreAntiforgeryToken` attribute needs a higher order number to be activated.

Alternatively, you can turn off token validation globally as above, and then selectively apply token validation on a case by case basis by decorating the appropriate page model class with the `ValidateAntiForgeryToken` attribute (applying the same logic to the `Order` value as previously):

```
[ValidateAntiForgeryToken(Order = 1000)]
public class IndexModel : PageModel
{
    public void OnPost()
    {

    }
}

```

<div class="alert alert-note">

The `ValidateAntiForgeryToken` attribute has an upper case "F" in its name, whereas the `IgnoreAntiforgeryToken` has a lower case "f". This is by design.

</div>

Choosing to opt out of Antiforgery validation does not prevent the generation of the hidden field or the cookie. All it does is to skip the verification process. Generally, there is no good reason to do this. If you want your site to accept POST requests from external domains, the recommended solution is to use a Web API controller instead.