# Select 标签助手

Select 标签助手的作用是输出 HTML`select`元素，元素由从`SelectListItem`对象集合生成的选项填充，列举其它选项通过[Option 标签助手](/razor-pages/tag-helpers/option-tag-helper)设置。

| 属性 | 描述 |
| --- | --- |
| `for` | 表示Select元素在 [PageModel](/razor-pages/pagemodel)上的属性。 |
| `items` | `SelectListItem`对象集合、`SelectList`对象或为选择列表提供选项的枚举。|

## 备注

### 通常情况

`for`属性值是 PageModel 上的一个属性。Select 标签助手使用属性名的名称来为呈现的`select`元素生成`name`和`id`属性的值。如果属性是[模型绑定](/razor-pages/model-binding)的，则将选项的值绑定到模型属性。如果属性是一个集合，那么将通过在呈现元素中包含`multiple`属性来支持多选。

### 选项

`items`属性值表示出现在选择列表中的选项的`SelectListItem`或`SelectList`集合，或者可以是返回`SelectListItem`或`SelectList`集合的表达式。 下面的例子演示了创建数字集合，并在`OnGet()`处理方法将其添加到`ViewData`中，然后绑定到`items`属性：

```csharp
public void OnGet()
{
    ViewData["Numbers"] = Enumerable.Range(1,5)
        .Select(n => new SelectListItem {
            Value = n.ToString(),
            Text = n.ToString()
        }).ToList();
}

```

```html
<select asp-for="Number" asp-items="@((List<SelectListItem>)ViewData["Numbers"])">
    <option value="">Pick one</option>
</select>
```

`ViewData`方法需要转换为正确的类型，所以建议将集合添加为 PageModel 的一个属性：

```csharp
public List<SelectListItem> Numbers => Enumerable.Range(1, 5)
    .Select(n => new SelectListItem {
        Value = n.ToString(),
        Text = n.ToString()
    }).ToList();
```

```html
<select asp-for="Number" asp-items="Model.Numbers">
    <option value="">Pick one</option>
</select>
```

这是另一种方法，如何使用一个通用的`List`作为`SelectList`来源：

```csharp
[BindProperty]
public int Person { get; set; }
public List<Person> People { get; set; }
public void OnGet()
{
    People = new List<Person> {
        new Person { Id = 1, Name="Mike" },
        new Person { Id = 2, Name="Pete" },
        new Person { Id = 3, Name="Katy" },
        new Person { Id = 4, Name="Carl" }
    };
}
```

```html
<select asp-for="Person" asp-items="@(new SelectList(Model.People, "Id", "Name"))">
    <option value="">Pick one</option>
</select>
```

### 设置选项

`SelectListItem`有一个`Selected`的属性。 在以前版本的ASP.NET MVC和Web页面中，您可以将其设置为`true`，将其呈现为`selected`。`SelectList`的构造函数，也可以指定所选选项。Select 标签助手在`for`属性中设置了所选项目，这个属性的值被忽略，所选项目的`value`将与属性相匹配：

```csharp
[BindProperty]
public int Person { get; set; } = 3;
public List<SelectListItem> People { get; set; }
public void OnGet()
{
    People = new List<SelectListItem> {
        new SelectListItem { Value = "1", Text = "Mike" },
        new SelectListItem { Value = "2", Text = "Pete" },
        new SelectListItem { Value = "3", Text = "Katy" },
        new SelectListItem { Value = "4", Text = "Carl" }
    };
}
```

```html
<select asp-for="Person" asp-items="Model.People">
    <option value="">Pick one</option>
</select>
```

这是输出的HTML：

```html
<select data-val="true" data-val-required="The Person field is required." id="Person" name="Person">
    <option value="">Pick one</option>
    <option value="1">Mike</option>
    <option value="2">Pete</option>
    <option selected="selected" value="3">Katy</option>
    <option value="4">Carl</option>
</select>
```

### 枚举

`Html.GetEnumSelectList`方法可以很轻易的将枚举作为选择列表的数据源。下一个示演示如何使用 [System.DayOfWeek](https://msdn.microsoft.com/en-us/library/system.dayofweek(v=vs.110).aspx) 枚举来呈现星期作为选项值，并且假定 PageModel 具有一个`DayOfWeek`正确类型的属性：

```csharp
public DayOfWeek DayOfWeek { get; set; }
```

```html
<select asp-for="DayOfWeek" asp-items="Html.GetEnumSelectList<DayOfWeek>()">
    <option value="">Pick one</option>
</select>
```

输出的HTML如下所示：

```html
<select data-val="true" data-val-required="The DayOfWeek field is required." id="DayOfWeek" name="DayOfWeek">
    <option value="">Pick one</option>
    <option selected="selected" value="0">Sunday</option>
    <option value="1">Monday</option>
    <option value="2">Tuesday</option>
    <option value="3">Wednesday</option>
    <option value="4">Thursday</option>
    <option value="5">Friday</option>
    <option value="6">Saturday</option>
</select>
```

在这个例子中，第一个选项被选中。 这是因为它匹配`DayOfWeek`的默认值。 如果您不希望默认值被选定，可以将属性改为可空类型：

```csharp
public DayOfWeek? DayOfWeek { get; set; }
```

### 选项组

`SelectListGroup`类表示一个HTML`optgroup`元素。 如果您想使用选项组，可以根据需要创建`SelectListGroup`实例，然后将它们应用到单独的`SelectListItem`：

```csharp
public int Employee { get; set; }
public List<SelectListItem> Staff { get; set; }

public void OnGet()
{
    var Sales = new SelectListGroup { Name = "Sales" };
    var Admin = new SelectListGroup { Name = "Admin" };
    var IT = new SelectListGroup { Name = "IT" }; 

    Staff = new List<SelectListItem>
    {
        new SelectListItem{ Value = "1", Text = "Mike", Group = IT},
        new SelectListItem{ Value = "2", Text = "Pete", Group = Sales},
        new SelectListItem{ Value = "3", Text = "Katy", Group = Admin},
        new SelectListItem{ Value = "4", Text = "Carl", Group = Sales}
    };
}
```

下面显示了呈现的HTML（缩进格式的清晰明了）：

```html
<select data-val="true" data-val-required="The Employee field is required." id="Employee" name="Employee">
    <option value="">Pick one</option>
    <optgroup label="IT">
        <option value="1">Mike</option>
    </optgroup>
    <optgroup label="Sales">
        <option value="2">Pete</option>
        <option value="4">Carl</option>
    </optgroup>
    <optgroup label="Admin">
        <option value="3">Katy</option>
    </optgroup>
</select>
```