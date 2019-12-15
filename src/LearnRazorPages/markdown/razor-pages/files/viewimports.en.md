# The ViewImports File

The purpose of the __ViewImports.cshtml_ file is to provide a mechanism to make directives available to Razor pages globally so that you don't have to add them to pages individually.

The default Razor Pages template includes a __ViewImports.cshtml_ file in the _Pages_ folder - the root folder for Razor pages. All Razor pages in the folder heirarchy will be affected by the directives set in the __ViewImports.cshtml_ file.

The __ViewImports.cshtml_ file supports the following directives:

*   `@addTagHelper`
*   `@inherits`
*   `@inject`
*   `@model`
*   `@removeTagHelper`
*   `@tagHelperPrefix`
*   `@using`

The `@addTagHelper`, `@removeTagHelper` and `@tagHelperPrefix` directives relate to the management of [Tag Helpers](/razor-pages/tag-helpers). The `@inherits` directive is used to specify a base class that all affected pages derive from. [Dependency injection](/advanced/dependency-injection) is supported through the use of the `@inject` directive. The `@model` directive is used to specify the Model, and the `@using` directive makes namespaces available to all pages in the folder hierarchy.

The default __ViewImports.cshtml_ file typically contains two directives: a `@using` directive specifying the namespace that your _Pages_ folder has been provided (e.g. `MyApplication.Pages`) and an `@addTagHelper` directive making the `Microsoft.AspNetCore.Mvc.TagHelpers` library contents available to your pages:

```
@namespace MyApplication.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

```

The is no limit to the number of __ViewImports.cshtml_ files that a Razor Pages application can support. You can place additional __ViewImports.cshtml_ files in sub-folders to either add to the top level __ViewImports.cshtml_ file's directives, or to override its settings. The `@addTagHelper`, `@removeTagHelper`, `@inject` and `@using` directives are additive, while the other directives override eachother, the closer you get to the page. So, for example, the model specified in the root _Pages_ folder will be overriden for pages in a sub-folder if a different `@model` directive is specified in __ViewImports.cshtml_ file in that sub-folder.