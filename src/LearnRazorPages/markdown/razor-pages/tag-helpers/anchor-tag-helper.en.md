# The Anchor tag helper

The anchor tag helper targets the HTML anchor (`<a>`) tag which is used to generate relative links from the values passed to various custom attributes. It can also used to generate absolute URLs to external resources.

<div class="alert alert-warning">

The anchor tag helper's role is to generate an `href` attribute from the parameter values passed to its custom attributes. Therefore, if you try to add an `href` attribute to the anchor tag helper as well as values to the custom attributes, an exception will be raised:

<div class="exception-message">

**InvalidOperationException**: Cannot override the 'href' attribute for `<a>`. An `<a>` with a specified 'href' must not have attributes starting with 'asp-route-' or an 'asp-action', 'asp-controller', 'asp-area', 'asp-route', 'asp-protocol', 'asp-host', 'asp-fragment', 'asp-page' or 'asp-page-handler' attribute.

</div>

</div>

<div>

| Attribute | Description |
| --- | --- |
| `action` | The name of the action method on an MVC controller |
| `all-route-data`<sup>1</sup> | Multiple route parameter values |
| `area` | The name of the MVC area |
| `controller` | The name of the MVC controller |
| `fragment`<sup>2</sup> | The fragment in the URL |
| `host` | The domain |
| `page`<sup>3</sup> | The Razor page to link to |
| `page-handler`<sup>4</sup> | The Razor [page handler](/razor-pages/handler-methods) method to invoke |
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

    <a asp-all-route-data="d">Click</a>

    ```

    If the route has parameters defined, the anchor tag helper will output the values as URL segments: `<a href="/Page/value1/value2">Click</a>`. If it doesn't, the route parameters will be appended to the URL as query string values: `<a href="/Page?key1=value1&amp;key2=value2">Click</a>`.

2.  The fragment is the value after a hash or pound sign (`#`) in a URL used to identify a named portion of a document. The "Notes" heading above has an identity value of "notes" and can be referenced in a URL using the anchor tag helper like this:

    ```
    <a asp-fragment="notes">Click</a>

    ```

    producing this HTML: `<a href="/Page#notes">Click</a>`

3.  The name of the Razor page to link to must be provided without the file extension:

    ```
    <a asp-page="page">Click</a>

    ```

    If no valid page name is specified, the tag helper will generate a link to the current page. If you want to generate a link to the default page in a folder, you must include the default page's file name:

    ```
    <a asp-page="/folder/index">Folder</a>

    ```

    This renders as `<a href="/folder">Folder</a>`

4.  The [page handler method](/razor-pages/handler-methods) name will appear as a query string value unless it has been included as a route parameter for the target page.

5.  Razor pages doesn't support named routes. This parameter will only be used for MVC routes.

6.  The `route-` parameter enables you to specify the value for a single route value. The route parameter name is added after the hyphen. Here, the route parameter name is "key1":
    `<a asp-route-key1="value1">Click</a>`
    This renders as
    `<a href="/Page/value1">Click</a>`
    if the `key1` parameter is defined as part of the page's route template, or
    `<a href="/Page?key1=value1">Click</a>`
    if not.

## Routing Options

The anchor tag helper will generate URLs with the page name capitalised when you pass a value to the `page` attribute e.g.

```
<a asp-page="page">Click</a>

```

becomes

```
<a href="/Page">Click</a>

```

You can [configure](/configuration) `RouteOptions` if you prefer the generated URL to be all lower case:

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.Configure<RouteOptions>(options =>
    {
        options.LowercaseUrls = true;
    });
}

```

Another option, `AppendTrailingSlash` will result in a trailing slash being appended to the page name in every case:

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.Configure<RouteOptions>(options =>
    {
        options.AppendTrailingSlash = true;
    });
}

```

With both options enabled, the resulting HTML looks like this:

```
<a href="/page/">Click</a>

```

</div>