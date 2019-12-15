# Layout Pages

Most sites feature the same content on every page, or within a large number of pages. Headers, footers, and navigation systems are just some examples. Site-wide scripts and style sheets also fall into this category. Adding the same header to every page in your site breaks the DRY principle (Don't Repeat Yourself). If you need to change the appearance of the header, you need to edit every page. The same applies to other common content, if you want to upgrade your client-side framework, for example. Some IDEs include tools for making replacements in multiple files, but that's not really a robust solution. The proper solution to this problem is the Layout page.

The layout page acts as a template for all pages that reference it. The pages that reference the layout page are called content pages. Content pages are not full web pages. They contain only the content that varies from one page to the next. The code example below illustrates a very simple layout page:

```
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="utf-8" />
        <title></title>
        <link href="/css/site.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        @RenderBody()
    </body>
</html>

```

What makes this a layout page is the call to the `RenderBody` method. That is where the result from processing the content page will be placed. Content pages reference their layout page via the `Layout` property of the page, which can be assigned in a code block at the top of a content page to point to a relative location:

```
@{
    Layout = "/_Layout";
}

```

Layout pages are typically named __Layout.cshtml_, the leading underscore preventing them from being browsed directly. Inclusion of the file extension is optional when supplying their location to the Layout property. Standard practice is to specify the location of the layout page in a __ViewStart.cshtml_ file, which affects all content pages in the folder in which it is placed, and all subfolders.

By default, the layout file is placed in the _Pages_ folder, but it can be placed anywhere in the application folder structure. If you come from an MVC background, you might prefer to create a folder called _Shared_ and to move the layout there. Use of the _ViewStart_ file makes updating to the new location easy:

```
@{
    Layout = "Shared/_Layout";
}

```

## Sections

The `RenderBody` method placement within the layout page determines where the content page will be rendered, but it is also possible to render other content supplied by the content page within a layout page. This is controlled by the placement of calls to the `RenderSection` method. The following example of a call to this method is taken from the layout page that forms part of the default template Razor Pages site:

```
@RenderSection("Scripts", required: false)

```

This call defines a section named "Scripts" - intended for page-specific script file references or blocks of JavaScript code so that they can be located just before the closing `</body>` tag. The second argument, `required` determines whether the content page must provide content for the named section. In this example, `required` is set to `false`, resulting in the section being optional. If the section is not optional, every content page that references the layout page must use the `@section` syntax to provide content for the section:

```
@section Scripts{
    // content here
}

```

In some cases, you might want to make a section optional, but you want to provide some default content in the event that the content page didn't provide anything for the section. You can use the `IsSectionDefined` method for this:

```
@if(IsSectionDefined("OptionalSection"))
{
    @RenderSection("OptionalSection")
}
else
{
    // default content
}

```

## Nested Layouts

Layout pages can be nested, that is, it is perfectly legal to specify the layout for a layout page. The following example shows a master layout which contains the head and style references, and two sub-layout pages. One has a single column for content and the other has two columns, the second of which contains a section. Content pages can refrence either of the two sub-layout pages and still benefit from the common markup provided by the master layout file.

__MasterLayout.cshtml_

```
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="utf-8" />
        <title></title>
        <link href="/css/site.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        @RenderBody()
    </body>
</html>

```

_SubLayout1.cshtml_

```
@{
    Layout = "/_MasterLayout";
}
<div class="main-content-one-col">
@RenderBody()
</div>

```

_SubLayout2.cshtml_

```
@{
    Layout = "/_MasterLayout";
}
<div class="main-content-two-col">
@RenderBody()
</div>
<div>
@RenderSection("RightCol")
</div>

```