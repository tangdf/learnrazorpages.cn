# Introduction

Tag helpers are reusable components for automating the generation of HTML in Razor Pages. Tag helpers target specific HTML tags. The ASP.NET Core framework includes a number of predefined tag helpers targeting many commonly used HTML elements as well as some custom tags:

*   [Anchor tag helper](/razor-pages/tag-helpers/anchor-tag-helper)
*   [Cache tag helper](/razor-pages/tag-helpers/cache-tag-helper)

*   [Environment tag helper](/razor-pages/tag-helpers/environment-tag-helper)
*   [Form Action tag helper](/razor-pages/tag-helpers/form-action-tag-helper)
*   [Form tag helper](/razor-pages/tag-helpers/form-tag-helper)
*   [Image tag helper](/razor-pages/tag-helpers/image-tag-helper)
*   [Input tag helper](/razor-pages/tag-helpers/input-tag-helper)
*   [Label tag helper](/razor-pages/tag-helpers/label-tag-helper)
*   [Link tag helper](/razor-pages/tag-helpers/link-tag-helper)
*   [Option tag helper](/razor-pages/tag-helpers/option-tag-helper)
*   [Script tag helper](/razor-pages/tag-helpers/script-tag-helper)
*   [Textarea tag helper](/razor-pages/tag-helpers/textarea-tag-helper)
*   [Validation Message tag helper](/razor-pages/tag-helpers/validation-message-tag-helper)
*   [Validation Summary tag helper](/razor-pages/tag-helpers/validation-summary-tag-helper)

The Tag helpers used in Razor Pages were introduced as part of ASP.NET MVC Core and are found in the `Microsoft.AspNetCore.Mvc.TagHelpers` package which is included as part of the `Microsoft.AspNetCore.All` meta-package. It is also possible to [create your own custom tag helpers](/advanced/custom-tag-helpers) to automate the generation of HTML in a limitless range of scenarios.

The following image illustrates an Anchor tag helper, which targets the HTML anchor `<a>` tag :

![Anchor Tag Helper](/images/01-06-2017-14-07-19.png)

Each tag helper augments the target element with additional attributes, prefixed with `asp-`. In the image above, you can see that the `asp-page` attribute in the tag has a value applied, and additional attributes are shown by Intellisense (in IDEs that provide this feature). Some of the attributes are specific to Razor Pages and some are specific to MVC. Others are relevant to both development platforms.

## Enabling Tag Helpers

Tag helpers are an opt-in feature. They are not available to the page by default. They are enabled by adding an `@addTagHelper` directive to the page, or more usually to a [__ViewImports.cshtml_ file](/razor-pages/files/viewimports):

```
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

```

The `@addTagHelper` directive is followed by a wildcard character (`*`) to specify that all tag helpers found in the specified assembly should be used, and then the name of the assembly containing the tag helpers is provided.

<div class="alert alert-info">

Note: The value provided to the `@addTagHelper` directive is not enclosed in quotes. This requirement was removed when ASP.NET Core MVC was at Release Candidate 2\. However, if you prefer, you can still surround values in quotes:

```
@addTagHelper "*, Microsoft.AspNetCore.Mvc.TagHelpers"

```

</div>

## Selective tag processing

Once a tag helper has been enabled, it will process every instance of the tag that it targets. That may not be desirable, especially so where tags don't feature special attributes that need to be processed. It is possible to opt in or out of tag processing selectively. You can use the `@addTagHelper` and `@removeTagHelper` directives to opt in or opt out of processing all tags of a certain type. Rather than pass the wildcard character to the `@addtagHelper` directive, you can pass the name(s) of the tag helpers that you want to enable:

```
@addTagHelper "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper, Microsoft.AspNetCore.Mvc.TagHelpers"

```

The only tag helper that is enabled in the previous snippet is the `AnchorTagHelper`. This approach is suitable if you only want to enable a small selection of tag helpers. If you want to enable most of the tag helpers in a library, you can use the `@removeTagHelper` directive to filter out tag helpers having enabled all of them. Here's how you would disable the `AnchorTagHelper` using this method:

```
@addTagHelper "*, Microsoft.AspNetCore.Mvc.TagHelpers"
@removeTagHelper "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper, Microsoft.AspNetCore.Mvc.TagHelpers"

```

You can opt individual tags out of processing by placing the! prefix just prior to the tag name. The following example illustrates how that is applied to a specific tag to prevent it being processed unncessarily:

```
<!a href="http://www.learnrazorpages.com">Learn Razor Pages</!a>

```

The prefix is placed in both the start and end tag. Any tag without the ! prefix will be processed by an associated TagHelper. The alternative option is to opt specific tags in to processing at parse time. You achieve this by registering a custom prefix via the `@tagHelperPrefix` directive and then applying your chosen prefix to tags you want to take part in processing. You can register your prefix in the __ViewImports.cshtml_ file, where you enabled TagHelper processing:

```
@tagHelperPrefix x

```

You can use pretty much any string you like as a prefix. Then you apply it to both the start and end tag, just like the ! prefix:

```
<xa asp-page="/Index">Home</xa>

```

Only those tags that feature the prefix will be processed. The image below illustrates how Visual Studio shows enabled tag helpers with a different font:

![Enabled Tag Helper](/images/2017-06-02_21-36-05.png)

For the sake of clarity, most developers are likely to use punctuation to separate the prefix from the tag name, for example:

```
@tagHelperPrefix x:

```

```
<x:a asp-page="/Index">Home</x:a>

```

This should reduce any visual confusion especially for designers when they look at the HTML content.