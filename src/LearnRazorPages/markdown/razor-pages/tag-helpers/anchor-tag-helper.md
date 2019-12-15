# Anchor 标签助手

Anchor 标签助手主要针对 HTML 锚点（`<a>`）标签，标签将各种自定义属性的值生成相对链接，还可以生成链接到外部资源的绝对URL路径。

<div class="alert alert-warning">

Anchor 标签助手的作用是将传递给它的自定义属性的参数值生成`href`属性。因此，如果您试图将`href`属性添加到 Anchor 标签助手以及自定义属性的值，那么将会引发异常：

<div class="exception-message">

__InvalidOperationException__ ：不能覆盖`<a>`的`href`属性。具有指定了`href`属性`<a>`标签不得具有以`asp-route-`、`asp-action`、`asp-controller`、`asp-area`、`asp-route`、 `asp-protocol`、`asp-host`、`asp-fragment`、`asp-page`或`asp-page-handler`开头的属性。

</div>
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

    <a asp-all-route-data="d">Click</a>
    ```
	如果路由定义了参数，则 Anchor 标签助手将输出的URL为：`<a href="/Page/value1/value2">Click</a>`。如果路由没有定义参数，路由参数将作为查询字符串值附加到URL中`<a href="/Page?key1=value1&amp;key2=value2">Click</a>`。


2. 片段是用于标识文档的命名部分的URL中的哈希值或井号（`＃`）之后的值。上面的“[备注](#备注)”标题的标识值为“notes”，可以使用 Anchor 标签助手在URL中引用，如下所示：

    ```html
    <a asp-fragment="notes">Click</a>
    ```

	产生的HTML为：`<a href="/Page#notes">Click</a>`


3. 必须提供链接到 Razor 页面的名称，不包含文件扩展名：

    ```html
    <a asp-page="page">Click</a>
    ```

	如果没有指定页面名称，则标签助手将生成到当前页面的链接。如果要生成指向文件夹中默认页面的链接，则必须包含默认页面的文件名称：

    ```html
    <a asp-page="/folder/index">Folder</a>
    ```

	这个最后呈现为：`<a href="/folder">Folder</a>`
	
4. `handler`,否则[处理方法](/razor-pages/handler-methods)的名称将作为查询字符串。

5. Razor 页面不支持路由名称，该参数将仅用于MVC路由。

6. `route-`参数能够为单个路由值指定值。 连字符（`-`）后面追加路由参数名称。在这里，路由参数名称是`key1`：<br/>	
	`<a asp-route-key1="value1">Click</a>`<br/>		
	如果`key1`参数被定义为页面路径模板的一部分，输出结果为：<br/>
	`<a href="/Page/value1">Click</a>`<br/>
    否则为：<br/>
    `<a href="/Page?key1=value1">Click</a>`

## 路由选项


向`page`属性传递值时，Anchor 标签助手将页面名称生成为大写的URL。

```html
<a asp-page="page">Click</a>
```
结果为：

```html
<a href="/Page">Click</a>
```

如果您更喜欢生成的URL是全部小写,您可以[配置](/configuration)`RouteOptions`：

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.Configure<RouteOptions>(options =>
    {
        options.LowercaseUrls = true;
    });
}
```

另一个选项是`AppendTrailingSlash`，在任何情况下都会在页面名称后附加一个斜线：

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.Configure<RouteOptions>(options =>
    {
        options.AppendTrailingSlash = true;
    });
}
```


启用这两个选项后，生成的 HTML 如下所示：

```html
<a href="/page/">Click</a>
```
