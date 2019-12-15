# The Formaction Tag Helper

The formaction tag helper adds a "formaction" attribute to the target element with a value generated from the parameters passed to the various custom attributes.

The formaction tag helper targets two elements:

*   Button
*   Input with a `type` attribute set to `image` or `submit`

The formaction tag helper is an example where the name of the tag helper doesn't follow the convention that matches tag helper class names with the name of the target element.

<div class="alert alert-info">

The `formaction` attribute specifies where a form is to be posted. It overrides the form's `action` attribute. It is new to HTML5 and is not supported in IE9 or earlier.

</div>

| Attribute | Description |
| --- | --- |
| `action` | The name of the action method on an MVC controller |
| `all-route-data`<sup>1</sup> | Multiple route parameter values |
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

1.  If the target URL includes multiple [route parameters](/razor-pages/routing#route-data), their values can be packaged as a `Dictionary<string, string>` and passed to the `all-route-data` parameter:

    ```
    @{   
        var d = new Dictionary<string, string>
            {
               { "key1", "value1" },
               { "key2", "value2" }
            };
    }

    <button asp-all-route-data="d">Submit</button>

    ```

    If the route has parameters defined, the formaction tag helper will output the values as URL segments: `<button formaction="/Page/value1/value2">Submit</button>`. If it doesn't, the route parameters will be appended to the URL as query string values: `<button formaction="/Page?key1=value1&amp;key2=value2">Submit</button>`.

2.  The fragment is the value after a hash or pound sign (`#`) in a URL used to identify a named portion of a document. The "Notes" heading above has an identity value of "notes" and can be referenced in a URL using the formaction tag helper like this:

    ```
    <button asp-fragment="notes">Submit</button>

    ```

    producing this HTML: `<button formaction="/Page#notes">Submit</button>`. It should be noted that fragments have no influence on the submission of a form.

3.  The name of the Razor page to link to must be provided without the file extension:

    ```
    <button asp-page="page">Submit</button>

    ```

    If no page name is specified, the tag helper will generate a link to the current page.

4.  The [page handler method](/razor-pages/handler-methods) name will appear as a query string value unless it has been included as a route parameter for the target page.

5.  Razor pages doesn't support named routes. This parameter will only be used for MVC routes.

6.  The `route-` parameter enables you to specify the value for a single route value. The route parameter name is added after the hyphen. Here, the route parameter name is "key1":
    `<button asp-route-key1="value1">Submit</button>`
    This renders as
    `<button formaction=/Page/value1">Submit</button>`
    if the `key1` parameter is defined as part of the page's route template, or
    `<button formaction="/Page?key1=value1">Submit</button>`
    if not.