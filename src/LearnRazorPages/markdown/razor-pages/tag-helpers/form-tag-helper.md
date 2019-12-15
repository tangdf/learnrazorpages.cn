# Form 标签助手

Form 标签助手在表单元素中输为`action`属性，还包括用于[请求验证](/security/request-verification)的防伪标记。如果在表单元素中没有指定`method`属性，标签助手会将其设置为`post`。

<div class="alert alert-warning">

Form 标签助手的主要作用是从传递给它的自定义属性参数中生成`action`属性。因此，如果增加自定义属性，再尝试向 Form 标签助手提供`action`属性，则会引发异常。

</div>

| 属性 | 描述 |
| --- | --- |
| `action` | MVC控制器上的Action方法的名称 |
| `all-route-data`<sup>1</sup> | 多个路由参数值 |
| `antiforgery`<sup>7</sup> |是否输出防伪标记。 |
| `area` | MVC区域的名称 |
| `controller` | MVC控制器的名称 |
| `fragment`<sup>2</sup> | URL中的片段 |
| `host` | 主机名 |
| `page`<sup>3</sup> | 要链接到的 Razor 页面 |
| `page-handler`<sup>4</sup> | 要调用的Razor页面[处理方法](/razor-pages/handler-methods) |
| `protocol` | 	协议（http，https，ftp等）|
| `route`<sup>5</sup> | 路由的名称|
| `route-`<sup>6</sup> |单个路由参数值|


## 笔记

1. 如果`action`属性的路径包含多个[路由参数](/razor-pages/routing#route-data)，则它们的值可以打包为一个`Dictionary<string, string>`并传递给`all-route-data`参数：

    ```html
    @{   
        var d = new Dictionary<string, string>
            {
               { "key1", "value1" },
               { "key2", "value2" }
            };
    }

    <form asp-all-route-data="d">...</form>
    ```
    
    如果路由定义了参数，则 Form 标签助手将输出的URL为：`<form ction="/Page/value1/value2">...</form>`。如果路由没有定义参数，路由参数将作为查询字符串值附加到URL中`<form action="/Page?key1=value1&amp;key2=value2">...</form>`。


2. 片段是用于标识文档的命名部分的URL中的哈希值或井号（`＃`）之后的值。上面的“[备注](#备注)”标题的标识值为“notes”，可以使用 Form 标签助手在URL中引用，如下所示：

    ```html
    <form asp-fragment="notes">...</form>
    ```

	产生的HTML为：`<form action="/Page#notes">...</form>`。应该注意的是，片段对表单提交没有作用。

3. 必须提供链接到 Razor 页面的名称，不包含文件扩展名：

    ```html
    <form asp-page="page">...</form>
    ```

	如果没有指定页面名称，则标签助手将生成到当前页面的链接。

4.  除非目标页面已经定义了路由参数`handler`,否则[处理方法](/razor-pages/handler-methods)的名称将作为查询字符串。

5.  Razor 页面不支持路由名称，该参数将仅用于MVC路由。

6.  `route-`参数能够为单个路由值指定值。 连字符（`-`）后面追加路由参数名称。在这里，路由参数名称是`key1`：<br/>	
  `<form asp-route-key1="value1">...</form>`<br/>		
  如果`key1`参数被定义为页面路径模板的一部分，输出结果为：<br/>
  `<form action="/Page/value1">...</form>`<br/>
    否则为：<br/>
    `<form action="/Page?key1=value1">...</form>`



7.  防伪标记输出为`name`属性为`__RequestVerificationToken`的隐藏域，默认会输出，除非表单指定了`action`属性、或者表单的提交方式设置为`GET`或者`antiforgerytoken`的值设置为`false`。