# ViewImports 文件

_\_ViewImports.cshtml_ 文件的目的是提供一种机制，将指令符应用到全局的 Razor 页面，不必单独添加到每一个页面中。

默认的 Razor 页面项目模板在_Pages_（Razor 页面的根文件夹）文件夹中包含一个 _\_ViewImports.cshtml_ 文件。_\_ViewImports.cshtml_ 文件中设置的指令会影响当前文件夹层次结构中的所有 Razor 页面。

_\_ViewImports.cshtml_ 文件支持以下指令
*  ` @addTagHelper`
*   `@inherits`
*   `@inject`
*   `@model`
*   `@removeTagHelper`
*  `@tagHelperPrefix`
*   `@using`

`@addTagHelper`、`@removeTagHelper`和`@tagHelperPrefix`指令与[标签助手](/razor-pages/tag-helpers)的管理有关；`@inherits`指令用于指定所有受影响的页面继承的基类；`@inject`指令通常用来支持[依赖注入](/advanced/dependency-injection)；` @model`指令用于指定PageModel；`@using`指令引用命名空间到文件夹层次结构中的所有页面。


默认的 _\_ViewImports.cshtml_ 文件通常包含两个指令：
* `@using`指令，用于指定提供给_Pages_文件夹的名称空间（例如`MyApplication.Pages`）；
* `@addTagHelper`指令，使`Microsoft.AspNetCore.Mvc.TagHelpers`库中的标签助手可应用到网页。

```html
@namespace MyApplication.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

Razor 页面应用程序可以支持多个 _\_ViewImports.cshtml_ 文件。可以将其它 _\_ViewImports.cshtml_ 文件放置在子文件夹中，可以在上一级  _\_ViewImports.cshtml_ 文件基础上追加或覆盖设置。  `@addTagHelper`、`@removeTagHelper`、`@inject`和`@using`指令是追加，其它的指令则是覆盖。 因此，如果在子文件夹的 _\_ViewImports.cshtml_ 文件中指定了不同的`@model`指令，那么在_Pages_根文件夹中指定的模型将被子文件夹中的覆盖。
