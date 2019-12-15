# Validation Message标签助手

Validation Message标签助手以HTML`span`元素为目标，并用于呈现特定属性的验证错误消息。

| 属性 | 描述 |
| --- | --- |
| `validation-for` | 要对当前页面实体进行验证的表达式，通常是 PageModel 属性名称。 |

## 备注

Validation Message 标签助手显示客户端和服务器端验证错误消息。 它们将为`field-validation-valid`的CSS类应用到`span`元素，在表单值无效的情况下，该类被改为`field-validation-error`。这些样式将追加到已经指定了其它样式`class`属性中。

```html
<span asp-validation-for="FirstName" class="myclass"></span>
```

如果表单值无效将显示为

```html
<span class="myclass field-validation-valid" data-valmsg-for="FirstName" data-valmsg-replace="true"></span>
```

最好将标签助手放置在它所指定控件的附近，这样以便用户更容易将错误信息与相关的表单值联系起来。