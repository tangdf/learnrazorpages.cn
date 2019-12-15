# Option 标签助手

Option 标签助手被设计为与 [Select 标签助手](/ razor-pages/tag-helpers/select-tag-helper) 结合工作。 它没有自定义属性。 它有两个主要用途：

1. 能够将项目手动添加到要呈现的选项列表（如默认选项）。
2. 提供的任何选项值与匹配 Select 标签助手的`for`属性的值相匹配，则会将其设置为`selected`。

第一个例子演示了手动添加 Select 标签助手的默认选项。 首先，这里是一个简单的[PageModel](/razor-pages/pagemodel)，它具有一个`Items`属性，要绑定到 Select 标签助手的`SelectListItem`集合。 选项是数字1到3。PageModel 还也有一个`Number`属性，表示所选的项目：

```csharp
public class TaghelpersModel : PageModel
{
    public List<SelectListItem> Items => 
        Enumerable.Range(1, 3).Select(x => new SelectListItem {
            Value = x.ToString(),
            Text = x.ToString()
        }).ToList();
    public int Number { get; set; }
    public void OnGet()
    {

    }
}
```

接下来是带有单个 Option 标签助手的 Select 标签助手，它没有应用任何值，内容如下：

```html
<select asp-for="Number" asp-items="Model.Items">
    <option value="">Pick one</option>
</select>
```

最终呈现的HTML如下：

```
<select data-val="true" data-val-required="The Number field is required." id="Number" name="Number">
    <option value="">Pick one</option>
    <option value="1">1</option>
    <option value="2">2</option>
    <option value="3">3</option>
</select>
```

第二个例子演示了手动添加 Select 标签助手的 Option，其中一个与`for`属性的值相匹配。 这与前面例子中的 PageModel 类似，只不过在这个例子中，这些选项不存在，`Number`属性默认值为`2`：

```csharp
public class TaghelpersModel : PageModel
{
    public int Number { get; set; } = 2;
    public void OnGet()
    {

    }
}
```

这里 Option 标签助手的所有选项都是手动添加的：

```html
<select asp-for="Number">
    <option value="">Pick one</option>
    <option>1</option>
    <option>2</option>
    <option>3</option>
</select>
```

呈现的HTML说明，尽管 Option 标签助手的`value`属性没有明确设置，但是显示为`2`的 Option 已被设置为 `Selected`：

```html
<select data-val="true" data-val-required="The Number field is required." id="Number" name="Number">
    <option value="">Pick one</option>
    <option>1</option>
    <option selected="selected">2</option>
    <option>3</option>
</select>
```