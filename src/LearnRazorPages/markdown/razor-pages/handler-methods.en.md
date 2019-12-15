# Handler Methods in Razor Pages

Handler methods in Razor Pages are methods that are automatically executed as a result of a request. The Razor Pages framework uses a naming convention to select the appropriate handler method to execute. The default convention works by matching the HTTP verb used for the request to the name of the method, which is prefixed with "On": `OnGet()`, `OnPost()`, `OnPut()` etc. They also have optional asynchronous equivalents: `OnPostAsync()`, `OnGetAsync()` etc. You do not need to add the `Async` suffix. The option is provided for developers who prefer to use the `Async` suffix on methods that contain asynchronous code.

Handler methods must be `public` and can have any return type, although typically, they are most likely to have a return type of `void` (or `Task` if asynchronous) or an [action result](/razor-pages/action-results).

The following example illustrates basic usage in a single Razor pages file:

```
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

The HTML part of the page includes a form that uses the `POST` method and a hyperlink, which initiates a `GET` request:

```
<h3>@Message</h3>
<form method="post"><button class="btn btn-default">Click to post</button></form>
<p><a href="/handlerexample" class="btn btn-default">Click to Get</a></p>

```

When the page is first navigated to, the "Get used" message is displayed because the HTTP `GET` verb was used for the request, firing the `OnGet()` handler.

When the "Click to post" button is pressed, the form is posted and the `OnPost()` handler fires, resulting in the "Post used" message being displayed.

Clicking the hyperlink results in the "Get used" message being displayed once more.

## Named Handler Methods

Imagine that you have a number of forms on the page, how will you know which one was submitted? Razor Pages includes a feature called "named handler methods". The following code shows a collection of named handler methods declared in a code block at the top of a Razor page (although they can also be placed in the PageModel class if you are using one):

```
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

The convention is that the name of the method is appended to "OnPost" or "OnGet", dpending on whether the handler should be called as a result of a `POST` or `GET` request. The next step is to associate a specific form action with a named handler. This is achieved by setting the asp-page-handler attribute value for a form taghelper:

```
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

The code above renders as three buttons, each in their own form along with the default value for the Message property:

![FormTagHelpers](https://www.mikesdotnetting.com/images/2017-05-19_22-10-01.png)

The name of the handler is added to the form's action as a querystring parameter: ![Handlers](https://www.mikesdotnetting.com/images/2017-05-19_22-22-34.png)

As you click each button, the code in the handler associated with the querystring value is executed, changing the message each time.

![Handlers](https://www.mikesdotnetting.com/images/2017-05-19_22-43-02.png)

If you prefer not to have querystring values in the URL, you can use [routing](/razor-pages/routing) and add an optional route value for "handler" as part of the @page directive:

```
@page "{handler?}"

```

The name of the handler is then appended to the URL:

![Handler Routevalue](https://www.mikesdotnetting.com/images/22-05-2017-08-04-11.png)

## Parameters in Handler Methods

Handler methods can be designed to accept parameters:

```
public void OnPostView(int id)
{
    Message = $"View handler fired for {id}";
}

```

The parameter name must match a form field name for it to be automatically bound to the value:

```
<div class="col-lg-1">
    <form asp-page-handler="view" method="post">
        <button class="btn btn-default">View</button>
        <input type="hidden" name="id" value="3" />
    </form>
</div>

```

![Handler parameters](https://www.mikesdotnetting.com/images/22-05-2017-08-30-22.png)

Alternatively, you can use the [form tag helper's](/razor-pages/tag-helpers/form-tag-helper) `asp-route` attribute to pass parameter values as part of the URL, either as a query string value or as route data:

```
<form asp-page-handler="delete" asp-route-id="10" method="post">
    <button class="btn btn-default">Delete</button>
</form>

```

You append the name of the parameter to the `asp-route` attribute (in this case "id") and then provide a value. This will result in the parameter being passed as a querystring value:

![Parameter as Query String](https://www.mikesdotnetting.com/images/22-05-2017-09-17-21.png)

Or you can extend the route defintion for the page to account for an optional parameter:

```
@page "{handler?}/{id?}"

```

This results in the parameter value being added as a separate segment in the URL:

![Parameter as route segment](https://www.mikesdotnetting.com/images/22-05-2017-09-21-41.png)

## Handling Multiple Actions For The Same Form

Some forms need to be designed to cater for more than one possible action. Where this is the case, you can either write some conditional code to determine which action should be taken, or you can write separate handler methods and then use the [form action tag helper](/razor-pages/tag-helpers/form-action-tag-helper) to specify the handler method to execute on submission of the form:

```
<form method="post">
    <button asp-page-handler="Register">Register Now</button>
    <button asp-page-handler="RequestInfo">Request Info</button>
</form>

```

The value passed to the `page-handler` attribute is the name of the handler method without the `OnPost` prefix or `Async` suffix:

```
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