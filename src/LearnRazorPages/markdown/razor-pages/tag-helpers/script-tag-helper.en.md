# The Script Tag Helper

The role of the script tag helper is to generate links dynamically to script files and fallbacks in the event that the primary file is not available, such as if the primary file is located on a remote Content Delivery Network (CDN) which is unavailable for any reason.

| Attribute | Description |
| --- | --- |
| `src-include` | A comma separated list of globbed file patterns of script files to load. The glob patterns are assessed relative to the application's 'webroot' setting. |
| `src-exclude` | A comma separated list of globbed file patterns of script files to exclude from loading. The glob patterns are assessed relative to the application's 'webroot' setting. Must be used in conjunction with `src-include` |
| `fallback-src` | The URL of a script file to fallback to in the case the primary one fails. |
| `fallback-src-include` | A comma separated list of globbed file patterns of script files to fallback to in the case the primary one fails. The glob patterns are assessed relative to the application's 'webroot' setting. |
| `fallback-src-exclude` | A comma separated list of globbed file patterns of script files to exclude from the fallback list, in the case the primary one fails. The glob patterns are assessed relative to the application's 'webroot' setting. Must be used in conjunction with `fallback-src-include`. |
| `fallback-test` | A Javascript expression to use for the fallback test. Should resolve to `true` if the primary script loads successfully. |
| `append-version` | Boolean value indicating if a file version token should be appended to the `src` urls. |

## Notes

You are only likely to use this helper if you use a CDN version of a script file and your site will be unusable in the event that it is not available. The test expression that you specify should resolve to `true` if the CDN version of the script is available. The helper will render the expression with

The default Razor Pages template illustrates its usage:

```
<script src="https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.7/bootstrap.min.js"
        asp-fallback-src="~/lib/bootstrap/dist/js/bootstrap.min.js"
        asp-fallback-test="window.jQuery && window.jQuery.fn && window.jQuery.fn.modal"
        crossorigin="anonymous"
        integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa">
</script>

```

The preferred version of _bootstrap.min.js_ is hosted on the Microsoft CDN, and the fallback is stored locally. The tag helper includes the test expression in its output:

```
<script src="https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.7/bootstrap.min.js"
 crossorigin="anonymous" 
integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa">
</script>
<script>
    (window.jQuery && window.jQuery.fn && window.jQuery.fn.modal||document.write("\u003Cscript src=\u0022\/lib\/bootstrap\/dist\/js\/bootstrap.min.js\u0022 crossorigin=\u0022anonymous\u0022 integrity=\u0022sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa\u0022\u003E\u003C\/script\u003E"));
</script>

```

The test expression is incorporated into an OR statement that results in a `script` tag for the local version being rendered if the test expression (which in this case is looking for the presence of jQuery and specifically the Bootstrap modal function) resolves to `false`.