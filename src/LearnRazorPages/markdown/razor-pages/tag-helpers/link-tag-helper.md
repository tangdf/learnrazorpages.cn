# Link 标签助手

Link 标签助手的作用是在主文件不可用的情况下动态生成 CSS 文件和备用的链接，例如主文件位于远程内容分发网络（CDN），因为不可知的原因不可用。。

| 属性| 描述 |
| ---| --- |
| `href-include` | 加载多个 CSS 样式文件，使用逗号分隔列表。文件路径是相对于应用程序的`webroot`路径。|
| `href-exclude` | 排除多个 CSS 样式文件，使用逗号分隔列表。文件路径是相对于应用程序的`webroot`路径， 必须与`href-include`结合使用。|
| `fallback-href` | 在主文件加载失败的情况下，CSS 样式文件的备用URL。|
| `fallback-href-include` | 逗号分隔的多个 CSS 样式文件列表，在主文件加载失败的情况下的备用文件。文件路径是相对于应用程序的`webroot`路径。|
| `fallback-href-exclude`| 逗号分隔的多个 CSS 样式文件列表，在主要文件失败的情况下从后备列表中排除的文件。文件路径是相对于应用程序的`webroot`路径，必须与`fallback-href-include`结合使用。|
| `fallback-test-class`	| 在样式表中定义的类名称，用于备用测试。 必须与`fallback-test-property`、`fallback-test-value`以及`fallback-href`、`fallback-href-include` 两个中的一个结合使用。|
| `fallback-test-property`	| 用于备用测试的 CSS 属性名称。 必须与`fallback-test-class`、`fallback-test-value`以及`fallback-href`、`fallback-href-include`两个中的一个结合使用。|
| `fallback-test-value` | 用于备用测试的 CSS 属性值。 必须与`fallback-test-class`、`fallback-test-property`以及`fallback-href`、`fallback-href-include`两个中的一个结合使用。		|
| `append-version` | 布尔值，表示是否在URL后面追加文件版本。|

## 备注

如果您对CSS 文件使用 CDN 加速，如果 CDN 不可用的情况，您的网站将无法使用。这时您可以使用此标签助手。 标签助手通过测试指定类属性的值是否正确来检查样式表的 CDN 是否可用。 如果没有，标签助手将引入指定样式表新链接。

Razor 页面默认的项目模板展示了它的用法：

```html
<link rel="stylesheet" href="https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.7/css/bootstrap.min.css"
        asp-fallback-href="~/lib/bootstrap/dist/css/bootstrap.min.css"
        asp-fallback-test-class="sr-only" asp-fallback-test-property="position" asp-fallback-test-value="absolute" />
<link rel="stylesheet" href="~/css/site.min.css" asp-append-version="true" />
```

Bootstrap的首选版本托管在Microsoft CDN上，并且在本地保存了备用版本。 样式表包含一个`sr-only`类，它具有以下定义:

```css
.sr-only{
    position:absolute;
    width:1px;height:1px;
    padding:0;margin:-1px;
    overflow:hidden;
    clip:rect(0,0,0,0);
    border:0
}
```

`position`属性是测试的对象，根据（在这个特定的情况下）属性值是否为`absolute`。 如果测试失败，则加载样式表的本地版本。 标签助手负责生成`meta`标记和测试的JavaScript:

```html
<meta name="x-stylesheet-fallback-test" content="" class="sr-only" />
<script>
!function(a,b,c,d){
    var e,
    f=document,
    g=f.getElementsByTagName("SCRIPT"),
    h=g[g.length-1].previousElementSibling,
    i=f.defaultView&&f.defaultView.getComputedStyle?f.defaultView.getComputedStyle(h):h.currentStyle;
    if(i&&i[a]!==b)
        for(e=0;e<c.length;e++)
            f.write('<link href="'+c[e]+'" '+d+"/>")}("position","absolute",["\/lib\/bootstrap\/dist\/css\/bootstrap.min.css"], "rel=\u0022stylesheet\u0022 ");
</script>
```