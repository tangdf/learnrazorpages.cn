# Formaction 标签助手

 

Formaction 标签助手将一个`formaction` 属性添加到目标元素，其值来自传递给各种自定义属性的参数。


Formaction 标签助手针对下面两种元素：

*   Button
*   包括`type`属性的`image`或`submit`


Formaction 标签助手是个列，标签助手的名称不遵循与目标元素名称相匹配的约定。

<div class="alert alert-info">

`formaction` 属性指定了表单的提交的路径，会覆盖表单的`action`属性。这是HTML5的新功能，在IE9或更早版本中不受支持。

</div>


| 属性 | 描述 |
| --- | --- |
| `action` | MVC控制器上的Action方法的名称 |
| `all-route-data`<sup>1</sup> | 多个路由参数值 |
| `area` | MVC区域的名称 |
| `controller` | MVC控制器的名称 |
| `fragment`<sup>2</sup> | URL中的片段 |
| `host` | 主机名 |
| `page`<sup>3</sup> | 要链接到的 Razor 页面 |
| `page-handler`<sup>4</sup> | 要调用的Razor页面[处理方法](/razor-pages/handler-methods) |
| `protocol` | 	协议（http，https，ftp等）|
| `route`<sup>5</sup> | 路由的名称|
| `route-`<sup>6</sup> |单个路由参数值|

## 备注



1. 如果目标URL包含多个[路由参数](/razor-pages/routing#route-data)，则它们的值可以打包为一个`Dictionary<string, string>`并传递给`all-route-data`参数：

    ```html
    @{   
        var d = new Dictionary<string, string>
            {
               { "key1", "value1" },
               { "key2", "value2" }
            };
    }

    <button asp-all-route-data="d">Submit</button>
    ```
	如果路由定义了参数，则 Formaction 标签助手将输出的URL为：`<button formaction="/Page/value1/value2">Submit</button>`。如果路由没有定义参数，路由参数将作为查询字符串值附加到URL中`<button formaction="/Page?key1=value1&amp;key2=value2">Click</button>`。


2. 片段是用于标识文档的命名部分的URL中的哈希值或井号（`＃`）之后的值。上面的“[备注](#备注)”标题的标识值为“notes”，可以使用 Formaction 标签助手在URL中引用，如下所示：

    ```html
    <button asp-fragment="notes">Submit</button>
    ```

	产生的HTML为：`<button formaction="/Page#notes">Click</button>`


3. 必须提供链接到 Razor 页面的名称，不包含文件扩展名：

    ```html
    <button asp-page="page">Submit</button>
    ```

	如果没有指定页面名称，则标签助手将生成到当前页面的链接。

4.  除非目标页面已经定义了路由参数`handler`,否则[处理方法](/razor-pages/handler-methods)的名称将作为查询字符串。

5.  Razor 页面不支持路由名称，该参数将仅用于MVC路由。

6.  `route-`参数能够为单个路由值指定值。 连字符（`-`）后面追加路由参数名称。在这里，路由参数名称是`key1`：<br/>	
  `<button asp-route-key1="value1">Submit</button>`<br/>		
  如果`key1`参数被定义为页面路径模板的一部分，输出结果为：<br/>
  `<button formaction="/Page/value1">Submit</button>`<br/>
    否则为：<br/>
    `<button formaction="/Page?key1=value1">Submit</button>`

