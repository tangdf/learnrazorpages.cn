# Textarea 标签助手


Textarea 标签助手的作用是渲染 HTML 中的多行文本框`textarea`元素。

Textarea标签助手只有一个属性：

| 属性 | 描述 |
| --- | --- |
| `for` | 表示元素在 PageModel 上属性的表达式 |

## 备注

Textarea 标签助手根据传递给`asp-for`属性的实体属性的名称来呈现`id`和`name`属性。 它还输出验证所需的相关`data`属性。

下面的`MainText`属性的最大长度为300：

```csharp
[BindProperty, MaxLength(300)]
public string MainText { get; set; }
```

这是传递给标签助手`asp-for`属性的值：

```html
<textarea asp-for="MainText"></textarea>
```

输出的HTML包含Unobtrusive验证框架需要的验证属性，以及模型绑定对应的`name`属性值：

```html
<textarea 
    data-val="true" 
    data-val-maxlength="The field MainText must be a string or array type with a maximum length of &#x27;300&#x27;." 
    data-val-maxlength-max="300" 
    id="MainText" 
    name="MainText">
```