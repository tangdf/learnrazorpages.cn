# Action Results in Razor Pages

Action results in Razor Pages are commonly used as the return type of [handler methods](/razor-pages/handler-methods) and are responsible for generating responses and appropriate status codes. Action results implement either the abstract `Microsoft.AspNetCore.Mvc.ActionResult` class, or the `Microsoft.AspNetCore.Mvc.IActionResult` interface. ASP.NET Core includes more than three dozen `ActionResult` classes covering a wide range of needs, including but not limited to executing and returning the content of a Razor page, returning the content of a file, redirecting to another resource or simply returning a specific HTTP status code. It is also possible to author your own `ActionResult` classes if you need something bespoke.

The following code illustrates the use of the `RedirectToPageResult` class in a simple `OnGet` handler method, which results in the user being redirected to the specified page:

```
public IActionResult OnGet()
{
    return new RedirectToPageResult("Index");
}

```

The `OnGet` handler method has a return type of `IActionResult`, which means that the return type can be any class that implements the `IActionResult` interface somewhere in its inheritance hierarchy. Ultimately, this means any action result in the entire framework. Generally, it is a good idea to be as specific as possible with handler method return types, so this example should be refactored as follows:

```
public RedirectToPageResult OnGet()
{
    return new RedirectToPageResult("Index");
}

```

If your handler method returns more than one type of action result depending on conditions, you will need to broaden the return type of the method to `ActionResult` or `IActionResult` as this example demonstrates, which returns a `PageResult` if the `ModelState` is not valid, or a `RedirectToPageResult` (a different type) if `ModelState` is valid:

```
public IActionResult OnPost()
{
    if(!ModelState.IsValid)
    {
        return new PageResult();
    }
    // otherwise do some processing
    return new RedirectToPageResult("Index");
}

```

Many action results have associated helper methods defined on the Razor Pages `PageModel` class that negate the need to "new up" instances of `ActionResult` classes and thereby help simplify code. The `Page()` method returns a `PageResult`, and the `RedirectToPage` method returns a `RedirecToPageResult`, so the last example can be simplified using those methods:

```
public IActionResult OnPost()
{
    if(!ModelState.IsValid)
    {
        return Page();
    }
    // otherwise do some processing
    return RedirectToPage("Index");
}

```

The following table lists `ActionResult` classes that are intended to be used in Razor Pages development, along with any HTTP status codes they return, and associated helper methods, if they exist.

| ActionResult | Helper | HTTP Status Code | Description |
| --- | --- | --- | --- |
| ChallengeResult | Challenge | 401 | Used in authentication. You will return a `ChallengeResult` if the user has not authenticated successfully. It returns a 401 (Unauthorized) HTTP status code. |
| ContentResult | Content | 200 | Takes a string and returns it with a `text/plain` content-type header by default. Overloads enable you to specify the content-type to return other formats such as `text/html` or `application/json`, for example. |
| EmptyResult |  | 200 | This action result type can be used to denote that a server-side operation completed successfully where there is no return value. |
| FileContentResult | File | 200 | Returns a file from a byte array, stream or virtual path. |
| FileStreamResult |  | 200 | Returns a file from a stream |
| ForbidResult | Forbid | 403 | Used in authentication. The Forbid method returns a 403 (Forbidden) HTTP status code. |
| LocalRedirectResult | LocalRedirect
LocalRedirectPermanent
LocalRedirectPreserveMethod
LocalRedirectPreserveMethodPermanent | 302
301
307
308 | <sup>1</sup> |
| NotFoundResult | NotFound | 404 | Returns an HTTP 404 (Not Found) status code indicating that the requested resource could not be found. |
| PageResult | Page | 200 | Will process and return the result of the current page. |
| PhysicalFileResult | PhysicalFile | 200 | Returns a file form the specified physical path. |
| RedirectResult |  | 302
301
307
308 | Redirects the user to the URL specified, with an HTTP status code dependent on the options specified. The default is a temporary redirect (301) <sup>1</sup>. |
| RedirectToPageResult | RedirectToPage
RedirectToPagePermanent
RedirectToPagePreserveMethod
RedirectToPagePreserveMethodPermanent | 301
302
307
308 | Redirects the user to the specified page. <sup>1</sup> |
| SignInResult |  |  |  |
| SignOutResult |  |  |  |
| StatusCodeResult |  |  |  |
| UnauthorizedResult |  | 401 |  |

### Notes

1.  The Redirect action results include options to specify that the redirect should be permanent and/or the HTTP verb should be preserved across redirects. Temporary redirects are denoted by HTTP status codes 302 and 307\. Permanent redirects are denoted by status codes 301 and 308\. Permanent redirect instructions are usually honoured by search engines and other indexing technologies, which will replace the existing entry in their index with the new location specified in the `location` header that action result generates.

    If you do not opt to preserve the HTTP method that the original request used, the redirected request may (and often will) use GET regardless of the method used for the original request. If you want to ensure that the original HTTP method is preserved for the subsequent request (and potentially any form values), you should use one of the options that includes `PreserveMethod` in their name, or specify `true` for the appropriate parameter in the more generic `RedirectResult` method.