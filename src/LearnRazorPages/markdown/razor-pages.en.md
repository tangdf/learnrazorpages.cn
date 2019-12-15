# Razor Pages

All Razor files end with _.cshtml_. Most Razor files are intended to be browsable and contain a mixture of client-side and server-side code, which, when processed, results in HTML being sent to the browser. These pages are usually referred to as "content pages". This section takes a deeper look at content pages, and their associated [PageModel](/razor-pages/pagemodel) files.

## Content Pages

For a file to act as a Razor content page, it must have three characteristics:

*   It cannot have a leading underscore in its file name
*   The file extension is _.cshtml_
*   The first line in the file is `@page`

Placing the `@page` directive as the first line of code is critical. If this is not done, the file will not be seen as a Razor page, and will not be found if you try to browse to it. There can be empty space before the `@page` directive, but there cannot be any other characters, even an empty code block. The only other content permitted on the same line as the `@page` directive is a [route template](/razor-pages/routing#adding-constraints).

Content pages can have a [layout file](/razor-pages/files/layout) specified, but this is not mandatory. They can optionally include code blocks, HTML, JavaScript and inline Razor code.

## Razor Syntax

Content pages are largely comprised of HTML, but they also include [Razor syntax](https://www.mikesdotnetting.com/article/153/inline-razor-syntax-overview) which enables the inclusion of executable C# code within the content. The C# code is executed on the server, and typically results in dynamic content being included within the response sent to the browser.

## Single File Approach

Although not recommended, it is possible to develop Razor Page applications that rely solely on content pages. The following example features an approach that is most like that which is familiar to developers with a scripting background, like PHP or classic ASP:

_Example.cshtml_:

```html
@page 
@{
    var name = string.Empty;
    if (Request.HasFormContentType)
    {
        name = Request.Form["name"];
    }
}

<div style="margin-top:30px;">
    <form method="post">
        <div>Name: <input name="name" /></div>
        <div><input type="submit" /></div>
    </form>
</div>
<div>
    @if (!string.IsNullOrEmpty(name))
    {
        <p>Hello @name!</p>
    }
</div>

```

The Razor content page requires the @page directive at the top of the file. The `HasFormContentType` property is used to determine whether a form has been posted and the Request.Form collection is referenced within a Razor code block with the relevant value within it assigned to the `name` variable.

The Razor code block is denoted by and opening `@{` and is temrinated with a closing `}`. The content within the block is standard C# code.

Single control structures do not need a code block. You can simpy prefix them with the `@` sign. This is illustrated by the `if` block in the preceding example.

To render the value of a C# variable or expression, you prefix it with the `@` sign as shown with the `name` variable within the `if` block.

The next example results in the same functionality as the previous example, but it uses an `@functions` block to declare a public property which is decorated with the `BindProperty` attribute, ensuring that the property takes part in [model binding](/razor-pages/model-binding), removing the need to manually assign form values to variables.

_Example.cshtml_:

```html
@page

@functions {
    [BindProperty]
    public string Name { get; set; }
}

<div style="margin-top:30px;">
    <form method="post">
        <div>Name: <input name="name" /></div>
        <div><input type="submit" /></div>
    </form>
    @if (!string.IsNullOrEmpty(Name))
    {
        <p>Hello @Name!</p>
    }
</div>

```

This approach is an improvement on the previous in that is makes use of strong typing, and while the processing logic can be restricted to the `@functions` block, the page will become more difficult to maintain and test.

## PageModel Files

The recommended way to develop Razor Pages applications is to minimise the amount of server-side code in the content page to the barest minimum. Any code relating to the processing of user input or data should be placed in [PageModel files](/razor-pages/pagemodel), which share a one-to-one mapping with their associated content page. They even share the same file name, albeit with an additional _.cs_ on the end to denote the fact that they are actually C# class files.

The following code shows the _Example.cshtml_ file adapted to work with a PageModel:

_Example.cshtml_:

```html
@page
@model ExampleModel

<div style="margin-top:30px;">
    <form method="post">
        <div>Name: <input asp-for="Name" /></div>
        <div><input type="submit" /></div>
    </form>
    @if (!string.IsNullOrEmpty(Model.Name))
    {
        <p>Hello @Model.Name!</p>
    }
</div>

```

And this is the code for the PageModel class:

_Example.cshtml.cs_:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPages.Pages
{
    public class ExampleModel : PageModel
    {
        [BindProperty]
        public string Name { get; set; }
    }
}

```

The PageModel class has a single property defined as in the previous example, and it is decorated with the `BindProperty` attribute. The content page no longer has the `@functions` block, but it now includes an `@model` directive, specifying that the `ExampleModel` is the model for the page. This also enables [Tag helpers](/razor-pages/tag-helpers) in the page, further taking advantage of compile-time type checking.

The default projects generate content pages paired with PageModel files. This is the recommended approach. However, it is also useful to know how to work with content pages without a PageModel for circumstances where they are not needed.