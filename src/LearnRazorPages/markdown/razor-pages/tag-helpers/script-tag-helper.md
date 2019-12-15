#  Script 标签助手

Script 标签助手的作用是在主文件不可用的情况下动态生成脚本文件和备用的链接，例如主文件位于远程内容分发网络（CDN），因为不可知的原因不可用。。

| 属性 | 描述 |
| --- | --- |
| `src-include` | 载多个脚本文件，使用逗号分隔列表。文件路径是相对于应用程序的`webroot`路径 |
| `src-exclude` | 排除多个脚本文件，使用逗号分隔列表。文件路径是相对于应用程序的`webroot`路径， 必须与`src-include`结合使用。 |
| `fallback-src` | 在主文件加载失败的情况下，脚本文件的备用URL。。|
| `fallback-src-include` | 逗号分隔的多个脚本文件列表，在主文件加载失败的情况下的备用文件。文件路径是相对于应用程序的`webroot`路径。|
| `fallback-src-exclude` | 逗号分隔的多个脚本文文件列表，在主要文件失败的情况下从后备列表中排除的文件。文件路径是相对于应用程序的`webroot`路径，必须与`fallback-src-include`结合使用。 |
| `fallback-test` | 用于备用测试的Javascript表达式。 如果主脚本加载成功，应该解析为`true`。 |
| `append-version` | 布尔值，表示是否应将文件版本标记追加到`src`上。 |



## 备注

如果您对脚本文件使用CDN版本，如果 CDN 不可用的情况，您的网站将无法使用。这时您可以使用此标签助手。 标签助手通过输出的表达式测试是否为`true`来检查脚本文件的 CDN 是否可用。


Razor 页面默认的项目模板展示了它的用法：

```html
<script src="https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.7/bootstrap.min.js"
        asp-fallback-src="~/lib/bootstrap/dist/js/bootstrap.min.js"
        asp-fallback-test="window.jQuery && window.jQuery.fn && window.jQuery.fn.modal"
        crossorigin="anonymous"
        integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa">
</script>

```

_bootstrap.min.js_的首选版本托管在Microsoft CDN上，并且在本地保存了备用版本。 标签助手在其输出中包含测试表达式：

```html
<script src="https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.7/bootstrap.min.js"
 crossorigin="anonymous" 
integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa">
</script>
<script>
    (window.jQuery && window.jQuery.fn && window.jQuery.fn.modal||document.write("\u003Cscript src=\u0022\/lib\/bootstrap\/dist\/js\/bootstrap.min.js\u0022 crossorigin=\u0022anonymous\u0022 integrity=\u0022sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa\u0022\u003E\u003C\/script\u003E"));
</script>
```

测试表达式被合并到一个`OR`语句中，如果测试表达式（在这种情况下查找jQuery的存在，特别是Bootstrap `modal`函数）解析为`false`，那么会输出使用本地版本的`script`标记。

