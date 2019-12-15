
# 创建自定义标签助手


作为 Razor 页面框架的一部分，包含了大量的[标签助手](/razor-pages/tag-helpers)。也可以创建自定义标签助手来在复杂的场景中自动生成HTML


第一个示例使用一个标签助手来生成分页链接。标签助手将在`div`生成一个无序列表，每个列表项都链接到一个数据页面。生成的HTML应该是这样的：

```html
<div>
    <ul class="pagination">
        <li class="active"><a href="/page?page=1" title="Click to go to page 1">1</a></li>
        <li class=""><a href="/page?page=2" title="Click to go to page 2">2</a></li>
        <li class=""><a href="/page?page=3" title="Click to go to page 3">3</a></li>
        <li class=""><a href="/page?page=4" title="Click to go to page 4">4</a></li>
        <li class=""><a href="/page?page=5" title="Click to go to page 5">5</a></li>
    </ul>
</div>

```

应用的CSS类来自BootStrap样式，因此HTML在浏览器中呈现如下所示：

![Bootstrap pagination tag helper](/images/05-06-2017-08-50-51.png)

有两种编写标签助手的方法。第一个方法是基于对标记助手的属性应用的值进行解析。第二种方法将展示如何将值从元素属性绑定到类属性，这是推荐的方法。完整地展示第一个方法的代码，然后解释它是如何工作的。


## 属性解析

```csharp
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace LearnRazorPages.TagHelpers
{
    [HtmlTargetElement("pager", Attributes = "total-pages, current-page, link-url")]
    public class PagerTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (
                int.TryParse(context.AllAttributes["total-pages"].Value.ToString(), out int totalPages) && 
                int.TryParse(context.AllAttributes["current-page"].Value.ToString(), out int currentPage))
            {
                var url = context.AllAttributes["link-url"].Value;
                output.TagName = "div";
                output.PreContent.SetHtmlContent(@"<ul class=""pagination"">");

                var content = new StringBuilder();
                for (var i = 1; i <= totalPages; i++)
                {
                    content.AppendLine($@"<li class=""{(i == currentPage ? "active" : "")}""><a href=""{url}?page={i}""  title=""Click to go to page {i}"">{ i}</a></li>");
                }
                output.Content.SetHtmlContent(content.ToString());
                output.PostContent.SetHtmlContent("</ul>");
                output.Attributes.Clear();
            }
        } 
    }
}
```

标签助手继承自抽象类`TagHelper`类，它定义了几个虚方法：`Process`和`ProcessAsync`。这些方法是生成HTML的地方。绝大多数助手都实现了同步方法`Process`。标签助手生成的输出在需要与"进程外”资源（如文件，数据库，Web服务等）进行通信的情况下，应该使用异步版本。`Process`方法的两个版本都有两个参数，`TagHelperContext`对象和`TagHelperOutput`对象。`TagHelperContext`对象包含有关当前正在操作标签信息，包括其所有属性。`TagHelperOutputobject`表示由标签助手生成的输出。当 Razor 解析器页面中遇到与标签助手相关联的元素时，标签助手的`Process(Async)`方法被调用并相应地生成输出。

### 将标签与TagHelper关联

建议您将标签助手的类名使用`TagHelper`为后缀。在这个例子中，标签助手类已经被命名为`PagerTagHelper`。按照惯例，标签助手（去掉后缀后）会将与名称相同的元素作为目标 —— 在本例中：`<pager>`。如果您想要忽略后缀约定或使用不同名称的元素，则必须使用`HtmlTargetElement`标记修饰，指定应处理元素的名称。

您可以通过`HtmlTargetElement`标记的`Attributes`参数一步细化要定位的元素。在上面的例子中，三个属性被传递给`Attributes`参数：`current-page`、`total-pages`和`link-url`。它们被指定为参数后使得它们是强制性的，所以这个标签助手将仅对所有具有三个属性的`<pager>`元素进行操作。由于在本例中目标元素和标签助手名称之间存在匹配，因此将`pager`传递给`HtmlTargetElement`属性可能看起来是多余的，但是如果省略，标记将被重载为`Tag`属性设置为默认的通配符(' * ')。换句话说，省略标签名称，传递所需属性的列表将导致标签助手匹配任何具有所有必需属性的元素。如果您想要定位一组元素，则可以设置多个`HtmlTargetElement`标记。

### 生成HTML

`TagHelperContext.Attributes`集合通过基于字符串的索引检索值。只有在`totalPages`和`currentPage`属性值可以被解析为数字时，才会进一步处理这个标签助手。

`TagHelperOutput`参数的`TagName`属性设置为`div`。这将导致在最终输出中`pager`被`div`取代。这是必须的，否则标签被转换为HTML时将保留“pager”名称，结果将不会在浏览器中呈现。

`Process`方法的`output`参数具有以下属性：`PreContent`、`Content`和`PostContent`。`PreContent`出现在由`TagName`属性指定的开始标记之后，以及在应用于`Content`属性之前；`PostContent`出现在`Content`之后，在`TagName`属性指定的结束标记之前。

![Tag helper structure](https://www.mikesdotnetting.com/images/taghelpercontent.png)

每个属性都有一个可以设置内容的`SetHtmlContent`方法。在这个例子中，`PreContent`和`PostContent`属性被设置为`<ul>`标记的开始和结束。`StringBuilder`用于构建一组`<li>`，其中包含将应用于`Content`属性的链接。最后，`Attributes`集合的`Clear()`方法用于从输出中删除所有的自定义属性（`total-pages`、`link-url`等）。如果您不删除自定义属性，它们将在最终的HTML中呈现。

最后，在  _\_ViewImports.cshtml_ 文件中使用`@addTagHelper`指令启用标签助手：

```html
@addTagHelper *, LearnRazorPages
```

当您使用依赖于属性解析的标签助手时，标签助手本身在页面中出现的颜色与其它HTML标记不同，但是这些属性与HTML标记的其它属性相同：

![Attribute Parsing](/images/05-06-2017-09-20-49.png)

它们还可用于智能提示：

![Attribute Parsing](/images/05-06-2017-09-25-50.png)

<div class="alert alert-warning">

在可接受值的类型方面没有对属性进行约束。标签助手的使用者完全有可能提供非整数值，这就是为什么上面的`Process`方法中需要检查数据类型的原因。您可以通过将元素属性值绑定到类属性来解决此问题。

</div>

### 绑定到简单的属性

下面是一个修改为使用简单属性分页标签助手的例子：

```csharp
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace LearnRazorPages.TagHelpers
{

    [HtmlTargetElement("pager", Attributes = "total-pages, current-page, link-url")]
    public class PagerTagHelper : TagHelper
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        [HtmlAttributeName("link-url")]
        public string Url { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.PreContent.SetHtmlContent(@"<ul class=""pagination"">");

            var content = new StringBuilder();
            for (var i = 1; i <= TotalPages; i++)
            {
                content.AppendLine($@"<li class=""{(i == CurrentPage ? "active" : "")}""><a href=""{Url}?page={i}"" title=""Click to go to page {i}"">{i}</a></li>");
            }
            output.Content.SetHtmlContent(content.ToString());
            output.PostContent.SetHtmlContent("</ul>");
            output.Attributes.Clear();
        }
    }
}

```

`Process`方法的主体与前面的例子几乎相同，只是现在对`PagerTagHelper`类的属性进行了修改。提取属性值的代码已被删除。由于标签助手将负责为每个公有属性生成元素的属性，并将元素的属性值绑定到标签助手的公有属性，因此不再需要它。默认情况下，元素的属性名称为对应标签助手公有属性名称的小写版本。如果标签助手的属性名称在第一个字符之后包含大写字母，则会插入连字符，所以公有属性`CurrentPage`在元素中的属性为`current-page`。

有时可能想要将属性名称映射到元素不同的属性名称。这是通过使用`HtmlAttributeName`标记修饰属性来实现，在标记中传递元素的属性名称。可以在上面看到`Url`属性映射到`link-url`属性。

生成的HTML与第一个示例相同，但主要区别在于，标签助手现在是强类型的，将不正确的类型值传递给属性会导致设计时错误 —— 可以在这里看到一个非整数值被提供给页面中的`total-pages`属性：

![strongly typed tag helper](/images/05-06-2017-10-02-07.png)

您也会得到智能感知的好处，告诉您什么类型的值，以及它是否是必需的：

![strongly typed tag helper required value](/images/05-06-2017-10-06-04.png)

最后，还可以从可以标签助手属性的XML注释中受益：

```csharp
public class PagerTagHelper : TagHelper
{
  public int CurrentPage { get; set; }
  public int TotalPages { get; set; }
  /// <summary>
  /// The url that the paging link should point to
  /// </summary>
  [HtmlAttributeName("link-url")]
  public string Url { get; set; }

  public override void Process(TagHelperContext context, TagHelperOutput output)
  
```

![XML comments](/images/05-06-2017-10-09-28.png)

### 绑定到复杂的属性

最后的示例演示如何将标记助手绑定到一个复杂的属性。这个标签助手使用谷歌的[Rich Snippets](https://developers.google.com/structured-data/rich-snippets/)来输出公司的详细信息——添加到HTML中的附加属性来提供内容的结构。

标签助手将绑定到的复杂属性由以下`Organisation`类表示：

```csharp
public class Organisation
{
    public string Name { get; set; }
    public string StreetAddress { get; set; }
    public string AddressLocality { get; set; }
    public string AddressRegion { get; set; }
    public string PostalCode { get; set; }
}
```

它被定义为TagHelper类的一个属性：

```csharp
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace LearnRazorPages.TagHelpers
{
    public class CompanyTagHelper : TagHelper
    {
        public Organisation Organisation { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.Add("itemscope itemtype", "http://schema.org/Organization");

            output.Content.SetHtmlContent(
                $@"<span itemprop=""name"">{Organisation.Name}</span>
                <address itemprop=""address"" itemscope itemtype=""http://schema.org/PostalAddress"">
                <span itemprop=""streetAddress"">{Organisation.StreetAddress}</span><br>
                <span itemprop=""addressLocality"">{Organisation.AddressLocality}</span><br>
                <span itemprop=""addressRegion"">{Organisation.AddressRegion}</span> 
                <span itemprop=""postalCode"">{Organisation.PostalCode}</span>");
        }
    }
}

```

将`Organisation`类型的`Company`属性添加到`PageModel`中，并在页面的`OnGet()`处理方法中实例化：

```csharp
public class ContactModel : PageModel
{
    public string Message { get; set; }
    public Organisation Company { get; set; }

    public void OnGet()
    {
        Message = "Contact Details";
        Company = new Organisation
        {
            Name = "Microsoft Corp",
            StreetAddress = "One Microsoft Way",
            AddressLocality = "Redmond",
            AddressRegion = "WA",
            PostalCode = "98052-6399"
        };
    }
}

```

在页面中添加标签助手，页面模型的`Company`属性被传递给由标签助手的`organisation`属性：

```html
<company organisation="Model.Company"></company>
```

这就是生成的HTML样子：

```html
<div itemscope itemtype="http://schema.org/Organization">
    <span itemprop="name">Microsoft Corp</span>
    <address itemprop="address" itemscope itemtype="http://schema.org/PostalAddress">
        <span itemprop="streetAddress">One Microsoft Way</span><br>
        <span itemprop="addressLocality">Redmond</span><br>
        <span itemprop="addressRegion">WA</span> 
        <span itemprop="postalCode">98052-6399</span>
    </address>
</div>
```

复杂属性提供了一种更简化的方式，可以用强类型的方式与标签助手一起工作，并且可以免除指定属性列表的需要。