# The Select Tag Helper

The role of the select tag helper is to render an HTML `select` element populated with options generated from a collection of `SelectListItem` objects, enumeration and/or options set via the [option tag helper](/razor-pages/tag-helpers/option-tag-helper).

| Attribute | Description |
| --- | --- |
| `for` | The property on the [PageModel](/razor-pages/pagemodel) that represents the selected element(s) |
| `items` | A collection of `SelectListItem` objects, a `SelectList` object or an enumeration that provide the options for the select list. |

## Notes

### General

The `for` attribute value is a property on the PageModel. The select tag helper uses the name of the name of the property to generate values for the `name` and `id` attributes on the rendered `select` element. The selected value(s) will be bound to the model property if the property is [model bound](/razor-pages/model-binding). If the property is a collection, support for multiple selections will be enabled by the inclusion of the `multiple` attribute in the rendered element.

### Options

The `items` attribute value is a collection of `SelectListItem` or a `SelectList` that represent the options that appear in the select list, or it can be an expression that returns a collection of `SelectListItem` or a `SelectList`. The following example shows a list of numbers being created and added to `ViewData` in the `OnGet(`) handler and then being bound to the `items` attribute:

```
public void OnGet()
{
    ViewData["Numbers"] = Enumerable.Range(1,5)
        .Select(n => new SelectListItem {
            Value = n.ToString(),
            Text = n.ToString()
        }).ToList();
}

```

```
<select asp-for="Number" asp-items="@((List<SelectListItem>)ViewData["Numbers"])">
    <option value="">Pick one</option>
</select>

```

The `ViewData` approach requires casting to the correct type, so the advice is to add the collection as a property of the PageModel:

```
public List<SelectListItem> Numbers => Enumerable.Range(1, 5)
    .Select(n => new SelectListItem {
        Value = n.ToString(),
        Text = n.ToString()
    }).ToList();

```

```
<select asp-for="Number" asp-items="Model.Numbers">
    <option value="">Pick one</option>
</select>

```

Here's another approach that shows how to use a generic `List` as a source for a `SelectList`:

```
[BindProperty]
public int Person Person { get; set; }
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

```
<select asp-for="Person" asp-items="@(new SelectList(Model.People, "Id", "Name"))">
    <option value="">Pick one</option>
</select>

```

### Setting Selected Item

The `SelectListItem` has a property named `Selected`. In previous versions of ASP.NET MVC and Web Pages, you could set this to `true` for the an item and it would be rendered as `selected`. This is also true for the `SelectList` constructor where you can specify the selected option. Thevalue of this property is ignored by the select tag helper. The way to set the selected item is to provide a value to the property that is assigned to the `for` attribute that matches the selected item's `value`:

```
[BindProperty]
public int Person Person { get; set; } = 3;
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

```
<select asp-for="Person" asp-items="Model.People">
    <option value="">Pick one</option>
</select>

```

The resulting HTML:

```
<select data-val="true" data-val-required="The Person field is required." id="Person" name="Person">
    <option value="">Pick one</option>
    <option value="1">Mike</option>
    <option value="2">Pete</option>
    <option selected="selected" value="3">Katy</option>
    <option value="4">Carl</option>
</select>

```

### Enumerations

The `Html.GetEnumSelectList` method makes it easy to use an enumeration as the data source for a select list. This next example shows how to use the [System.DayOfWeek](https://msdn.microsoft.com/en-us/library/system.dayofweek(v=vs.110).aspx) enumeration to present the days of the week as option values, and assumes that the PageModel has a property of the correct type called DayOfWeek:

```
public DayOfWeek DayOfWeek { get; set; }

```

```
<select asp-for="DayOfWeek" asp-items="Html.GetEnumSelectList<DayOfWeek>()">
    <option value="">Pick one</option>
</select>

```

The resulting HTML looks like this:

```
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

In this example, the first option is selected. This is because it matches the default value of `DayOfWeek`. If you do not want the default value to be pre-selected, you can make your model property nullable:

```
public DayOfWeek? DayOfWeek { get; set; }

```

### OptGroups

The `SelectListGroup` class represents an HTML `optgroup` element. If you want to use optgroups, you can create `SelectListGroup` instances as required, and then apply them to individual `SelectListItem`s:

```
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

The following shows the rendered HTML (indented for clarity):

```
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