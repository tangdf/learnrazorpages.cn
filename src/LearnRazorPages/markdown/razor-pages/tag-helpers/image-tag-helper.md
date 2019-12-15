# Image 标签助手

Image 标签助手的目标是`<img>`元素，并启用对图像文件版本控制。

| 属性 | 描述 |
| --- | --- |
| `append-version` | 布尔值，指定是否在图像的URL后追加文件版本 |

## 备注

如果`append-version`属性设置为`true`，则将`v`查询字符串追加到图像的 URL 中。该值是从文件内容中计算出来的，所以如果文件被修改，值会有所不同。浏览器依据查询字符串的值决定客户端缓存的文件是否满足请求。因此，如果查询字符串值更改，浏览器将从服务器中检索新版本的文件。

下面的示例是 Razor 页面（和MVC）默认模板提供的图片：

```html
<img asp-append-version="true" src="~/images/banner1.svg" />
```

呈现的HTML如下所示：

```html
<img src="/images/banner1.svg?v=GaE_EmkeBf-yBbrJ26lpkGd4jkOSh1eVKJaNOw9I4uk" />
```

这个图像是一个 _.svg_ 文件，可以使用任何文本编辑器进行编辑。下面演示了将第一个`path`元素的`fill`属性从颜色值`#56B4D9`更改为`#66B4D9`后呈现的结果：
```html
<img src="/images/banner1.svg?v=qp53a_aCPkTojNdLo1S2IvprtETqDiat_cWYbj1z8Z0" />
```