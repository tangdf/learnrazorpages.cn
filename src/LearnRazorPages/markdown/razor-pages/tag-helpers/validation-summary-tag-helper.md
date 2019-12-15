# Validation Summary 标签助手

Validation Summary 标签助手以HTML的`div`元素为目标，用于呈现所有表单验证错误信息的提示。

| 属性 | 描述 |
| --- | --- |
| `validation-summary` | `ValidationSummary`枚举，指定了验证错误显示方式。 |

可能的`ValidationSummary`枚举值是：

*   `None`, 不提供验证信息。
*   `ModelOnly`, 只提供实体验证错误信息。
*   `All`，提供实体  _和_  属性验证错误信息。

有关各种类型的错误（实体或属性）的更多信息，请参见[验证主题](/razor-pages/validation)。

## 注意事项

Validation Summary 标签助手通常放在窗体的顶部。 构成提示信息的各个项目显示在无序列表中：

```html
<div class="validation-summary-errors" data-valmsg-summary="true">
    <ul>
        <li>The FirstName field is required.</li>
        <li>The LastName field is required.</li>
        <li>The DateOfBirth field is required.</li>
    </ul>
</div>
```

您可以在无序表之前添加其他内容，添加到 Validation Summary 标签助手的内容中：

```html
<div asp-validation-summary="All">
    <span>Please correct the following errors</span>
</div>
```

额外的内容默认是可见的。 如果您不希望用户在页面有效的情况下查看内容，请更改`validation-summary-valid` CSS类（当页面有效时由标签助手注入`div`），将`div`或其内容隐藏：

```css
.validation-summary-valid { display: none; }
```

或者，针对上面的例子我们可以对`span`标签做处理

```css
.validation-summary-valid span { display: none; }
```

如果指定`None`作为`validation-summary`属性的值，则输出一个空的`div`。