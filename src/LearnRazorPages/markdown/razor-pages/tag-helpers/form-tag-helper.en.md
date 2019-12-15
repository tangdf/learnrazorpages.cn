# The Form Tag Helper

The form tag helper renders an `action` attribute within a form element. It also includes an anti-forgery token for [request verification](/security/request-verification). If a `method` attribute is not specified in the form element, the form tag helper will render one with a value of `post`.

<div class="alert alert-warning">

The form tag helper's primary role is to generate an `action` attribute from the parameters passed to its custom attributes. Therefore, if you try to provide an `action` attribute to the form tag helper in addition to the custom attributes, an exception will be raised.

</div>

| Attribute | Description |
| --- | --- |
| `action` | The name of the action method on an MVC controller |
| `all-route-data`<sup>1</sup> | Multiple route parameter values |
| `antiforgery`<sup>7</sup> | Specifies whether an anti-forgery token is rendered. |
| `area` | The name of the MVC area |
| `controller` | The name of the MVC controller |
| `fragment`<sup>2</sup> | The fragment in the URL |
| `host` | The domain |
| `page`<sup>3</sup> | The Razor page to link to |
| `page-handler`<sup>4</sup> | The Razor page handler method to invoke |
| `protocol` | The protocol (http, https, ftp etc) |
| `route`<sup>5</sup> | The name of the route |
| `route-`<sup>6</sup> | A single route parameter value |

## Notes

1.  If the target URL for the `action` includes multiple [route parameters](/razor-pages/routing#route-data), their values can be packaged as a `Dictionary<string, string>` and passed to the `all-route-data` parameter:

    ```
    @{   
        var d = new Dictionary<string, string>
            {
               { "key1", "value1" },
               { "key2", "value2" }
            };
    }

    <form asp-all-route-data="d">...</form>

    ```

    If the route has parameters defined, the form tag helper will output the values as URL segments: `<form action="/Page/value1/value2">...</form>`. If it doesn't, the route parameters will be appended to the URL as query string values: `<form action="/Page?key1=value1&amp;key2=value2" method="post">...</form>`.

2.  The fragment is the value after a hash or pound sign (`#`) in a URL used to identify a named portion of a document. The "Notes" heading above has an identity value of "notes" and can be referenced in a URL using the anchor tag helper like this:

    ```
    <form asp-fragment="notes">...</form>

    ```

    producing this HTML: `<form action="/Page#notes" method="post">...</form>`. It should be noted that fragments have no influence on the submission of a form.

3.  The name of the Razor page to link to must be provided without the file extension:

    ```
    <form asp-page="page">...</form>

    ```

    If no page name is specified, the tag helper will generate a link to the current page.

4.  The [page handler method](/razor-pages/handler-methods) name will appear as a query string value unless it has been included as a route parameter for the target page.

5.  Razor pages doesn't support named routes. This parameter will only be used for MVC routes.

6.  The `route-` parameter enables you to specify the value for a single route value. The route parameter name is added after the hyphen. Here, the route parameter name is "key1":
    `<form asp-route-key1="value1">...</form>`
    This renders as
    `<form action=/Page/value1" method="post">...</form>`
    if the `key1` parameter is defined as part of the page's route template, or
    `<form action="/Page?key1=value1" method="post">...</form>`
    if not.

7.  The anti-forgery token is rendered as a hidden input named `__RequestVerificationToken`. It will be rendered by default, unless the form has an `action` attribute specified, the form's method is set to `GET` or the `antiforgerytoken` value is set to `false`.