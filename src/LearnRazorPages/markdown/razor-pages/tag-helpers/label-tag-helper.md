# Label 标签助手

Label 标签助手根据分配给它的 PageModel 属性生成相应的`for`属性值和内容。 它只有一个属性：

| 属性 |描述 |
| --- | --- |
| `for` | 要针对当前 PageModel 属性的表达式 |

## 备注

Label 标签助手的目标是和 [Input 标签助手](/razor-pages/tag-helpers/input-tag-helper) 一起工作。 它将 PageModel 的属性作为`asp-for`属性的参数，并将属性的名称输出为 Label `for`属性的值， 同时做为 Label 内容。 假设 PageModel 有一个`Email`属性：

```html
<label asp-for="Email"></label>
```
输出的结果为：

```html
<label for="Email">Email</label>
```

您可以使用数据注解`Display`标记更改呈现的内容：

```csharp
[Display(Name="Email Address")]
public string EmailAddress { get; set; }
```

```html
<label for="EmailAddress">Email Address</label>
```