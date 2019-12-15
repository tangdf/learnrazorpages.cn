# 介绍

标签助手是可重用的组件，用于在 Razor 页面中自动生成HTML，它针对特定的 HTML 标签。ASP.NET Core 框架中包含很多预定义的标签助手，它们针对常用的 HTML 标签和一些自定义标签： 


*   [Anchor 标签助手](/razor-pages/tag-helpers/anchor-tag-helper)
*   [Cache 标签助手](/razor-pages/tag-helpers/cache-tag-helper)
*   [Environment 标签帮手](/razor-pages/tag-helpers/environment-tag-helper)
*   [Form Action 标签助手](/razor-pages/tag-helpers/form-action-tag-helper)
*   [Form 标签助手](/razor-pages/tag-helpers/form-tag-helper)
*   [Image 标签助手](/razor-pages/tag-helpers/image-tag-helper)
*   [Input 标签助手](/razor-pages/tag-helpers/input-tag-helper)
*   [Labe l标签帮手](/razor-pages/tag-helpers/label-tag-helper)
*   [Link 标签帮手](/razor-pages/tag-helpers/link-tag-helper)
*   [Option 标签助手](/razor-pages/tag-helpers/option-tag-helper)
*   [Script 标签助手](/razor-pages/tag-helpers/script-tag-helper)
*   [Textarea 标签帮手](/razor-pages/tag-helpers/textarea-tag-helper)
*   [Validation Message 标签助手](/razor-pages/tag-helpers/validation-message-tag-helper)
*   [Validation Summary 标签助手](/razor-pages/tag-helpers/validation-summary-tag-helper)

Razor 页面中使用的标签助手是作为 ASP.NET MVC Core 的一部分引入的，可以在`Microsoft.AspNetCore.Mvc.TagHelpers`包中找到，被作为`Microsoft.AspNetCore.All`元程序包的一部分。当然您也可以[创建自定义标签助手](/advanced/custom-tag-helpers)来在您需要的场景中自动生成 HTML。


下图展示了一个 Anchor 标签助手，它针对的是 HTML 中的`<a>`标签：

![Anchor Tag Helper](/images/01-06-2017-14-07-19.png)

每个标签助手都用额外的属性来扩充目标元素，前缀为`asp-`。在上图中，您可以看到标签中`asp-page`的属性应用的值，智能感知显示了其它属性，其中有一部分属性是专门针对 Razor 页面的，有一部分属于MVC，还有一部两个平台相关共用。


## 启用标签助手


标签助手是可选择的功能，默认情况下不可用于页面，可以通过在页面上添加一个`@addTagHelper`指令，但通常添加到[_\_ViewImports.cshtml_ ](/razor-pages/files/viewimports)文件：

```csharp
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

`@addTagHelper`指令后跟一个通配符（`*`），表示在指定程序集中找到的所有标签助手都可以使用，然后提供包含标签助手程序集的名称。在大多数情况下，程序集的名称是当前 Razor 页面项目的名称，除非在单独的项目中定义标签助手。如果想启用在当前站点中定义的标签助手，项目的名称为 _LearnRazorPages.csproj_ ，可以这样做：

```csharp
@addTagHelper *, LearnRazorPages
```

<div class="alert alert-info">


注意：提供给`@addTagHelper`指令的值不用引号引起来。当 ASP.NET Core MVC RC 2 发布时，此规范被移除了，但是如果您愿意，仍然可以用引号括住值：
```csharp
@addTagHelper "*, Microsoft.AspNetCore.Mvc.TagHelpers"
```
</div>


## 选择性的处理标签

一旦启用了标签帮助程序，它将处理其目标标签的每个实例。这可能不是您所希望的，尤其是在标签不具有需要处理的特殊属性的情况下。可以有选择地加入或移除标签处理。您可以使用`@addTagHelper`和`@removeTagHelper`指令来选择处理或选择不处理某个类型的所有标签。您不必将通配符传递给`@addtagHelper`指令，而是可以传递要启用标签助手的名称：

```csharp
@addTagHelper "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper, Microsoft.AspNetCore.Mvc.TagHelpers"
```

在前面的代码片段中唯一启用的标签助手是 Anchor 标签助手。如果您只想启用一小部分标签助手，则此方法适用，如果您希望在一个类库中启用大多数标签助手，那么您可以使用`@removeTagHelper`指令来过滤所有已经启用了的标签助手。下面是如何使用这个方法禁用`Anchor 标签助手`的方法：

```csharp
@addTagHelper "*, Microsoft.AspNetCore.Mvc.TagHelpers"
@removeTagHelper "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper, Microsoft.AspNetCore.Mvc.TagHelpers"
```

通过在标签名之前放置(`!`)前缀，可以选择单个标签。以下示例演示了如何将其应用于特定标签，以防止它被无情处理：

```html
<!a href="http://www.learnrazorpages.com">Learn Razor Pages</!a>
```
前缀放在开始和结束标签中,任何标签没有（`!`）前缀将由关联的标签处理。另一种选择是在解析时选择特定的标记来处理，您可以通过`@tagHelperPrefix`指令注册自定义前缀来实现此目的，然后将您选择的前缀应用于要参与处理的标签。您可以在_\_ViewImports.cshtml_ 文件中注册需要启用标签助手处理的前缀：

```csharp
@tagHelperPrefix x
```

您可以使用任何喜欢的字符串作为前缀，然后，将它应用于开始和结束标记，就像（`!`）前缀那样： 

```html
<xa asp-page="/Index">Home</xa>
```

只有那些具有前缀的标签才会被处理。下面的图片说明了Visual Studio是如何使用不同的字体来显示启用的标签助手:

![Enabled Tag Helper](/images/2017-06-02_21-36-05.png)

为了清楚起见，大多数开发人员可能使用标点符号（`:`）来区分标签名称的前缀，例如：

```csharp
@tagHelperPrefix x:
```

```html
<x:a asp-page="/Index">Home</x:a>
```

这应该可以减少视觉上的混乱，特别是当设计人员浏览 HTML 内容时。