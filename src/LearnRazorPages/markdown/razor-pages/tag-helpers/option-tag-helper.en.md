# The Options Tag Helper

The option tag helper is designed to work with the [select tag helper](/razor-pages/tag-helpers/select-tag-helper). It has no custom attributes. It has two main uses:

1.  It enables you to manually add items to a list of options to be rendered such as a default option.
2.  If any of the option values that you provide manually match the value of the select tag helper's `for` attribute, they will be set as `selected`.

The first example shows a default option manually added to the select tag helper. First, here is a simple [PageModel](/razor-pages/pagemodel) with a property called `Items` which is a collection of `SelectListItem` to be bound to a select tag helper. The options are just the numbers 1 to 3\. The PageModel also has a property named `Number` which represents the selected item:

```
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

Next is the select tag helper with a single option tag helper that has no value applied to it, and instructional text:

```
<select asp-for="Number" asp-items="Model.Items">
    <option value="">Pick one</option>
</select>

```

This results in the following rendered HTML:

```
<select data-val="true" data-val-required="The Number field is required." id="Number" name="Number">
    <option value="">Pick one</option>
    <option value="1">1</option>
    <option value="2">2</option>
    <option value="3">3</option>
</select>
```The second example shows a number of options manually added to the select tag helper, one of which matches the value of the `for` attribute property. Ths is similar to the PageModel in the previous example, except that in this instance, the options are not present and the `Number` property has a default value of 2 applied to it:

```csharp
public class TaghelpersModel : PageModel
{
    public int Number { get; set; } = 2;
    public void OnGet()
    {

    }
}

```

Here all of the options for the select tag helper have been added manually:

```
<select asp-for="Number">
    <option value="">Pick one</option>
    <option>1</option>
    <option>2</option>
    <option>3</option>
</select>

```

The rendered HTML shows that the option with a value of `2` has been set as `selected`, despite the fact that the `value` attributes on the option tag helper have not been explicitly set:

```
<select data-val="true" data-val-required="The Number field is required." id="Number" name="Number">
    <option value="">Pick one</option>
    <option>1</option>
    <option selected="selected">2</option>
    <option>3</option>
</select>

```