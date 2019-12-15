# The Link Tag Helper

The role of the link tag helper is to generate links dynamically to CSS files and fallbacks in the event that the primary file is not available, such as if the primary file is located on a remote Content Delivery Network (CDN) which is unavailable for any reason.

| Attribute | Description |
| --- | --- |
| `href-include` | A comma separated list of globbed file patterns of CSS stylesheets to load. The glob patterns are assessed relative to the application's 'webroot' setting. |
| `href-exclude` | A comma separated list of globbed file patterns of CSS stylesheets to exclude from loading. The glob patterns are assessed relative to the application's 'webroot' setting. Must be used in conjunction with `href-include` |
| `fallback-href` | The URL of a CSS stylesheet to fallback to in the case the primary one fails. |
| `fallback-href-include` | A comma separated list of globbed file patterns of CSS stylesheets to fallback to in the case the primary one fails. The glob patterns are assessed relative to the application's 'webroot' setting. |
| `fallback-href-exclude` | A comma separated list of globbed file patterns of CSS stylesheets to exclude from the fallback list, in the case the primary one fails. The glob patterns are assessed relative to the application's 'webroot' setting. Must be used in conjunction with `fallback-href-include`. |
| `fallback-test-class` | The class name defined in the stylesheet to use for the fallback test. Must be used in conjunction with `fallback-test-property` and `fallback-test-value`, and either `fallback-href` or `fallback-href-include`. |
| `fallback-test-property` | The CSS property name to use for the fallback test. Must be used in conjunction with `fallback-test-class` and `fallback-test-value`, and either `fallback-href` or `fallback-href-include`. |
| `fallback-test-value` | The CSS property value to use for the fallback test. Must be used in conjunction with `fallback-test-class` and `fallback-test-property`, and either `fallback-href` or `fallback-href-include`. |
| `append-version` | Boolean value indicating if file version should be appended to the href urls. |

## Notes

You are only likely to use this helper if you use a CDN version of a CSS file and your site will be unusable in the event that it is not available. The helper checks to see if CDN version of the style sheet is available by testing that the value for the specified class property is correct. If not, the helper injects a new link to the specified style sheet.

The default Razor Pages template illustrates its usage:

```
<link rel="stylesheet" href="https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.7/css/bootstrap.min.css"
        asp-fallback-href="~/lib/bootstrap/dist/css/bootstrap.min.css"
        asp-fallback-test-class="sr-only" asp-fallback-test-property="position" asp-fallback-test-value="absolute" />
<link rel="stylesheet" href="~/css/site.min.css" asp-append-version="true" />

```

The preferred version of Bootstrap is hosted on the Microsoft CDN, and the fallback is stored locally. The style sheet contains a class named `sr-only` which has the following definition:

```
.sr-only{
    position:absolute;
    width:1px;height:1px;
    padding:0;margin:-1px;
    overflow:hidden;
    clip:rect(0,0,0,0);
    border:0
}

```

The `position` property is the subject of the test, which determines (in this particular case) whether the computed value of the property is `absolute`. If the test fails, the local version of the style sheet is loaded. The tag helper is responsible for generating a meta tag and the JavaScript for the test:

```
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