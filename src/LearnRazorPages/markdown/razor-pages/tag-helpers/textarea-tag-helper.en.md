# The Textarea Tag Helper

The role of the textarea tag helper is to render and HMTL `textarea` element for capturing multiline text.

The textarea tag helper has one attribute:

| Attribute | Description |
| --- | --- |
| `for` | An expression to be evaluated against the current page model |

## Notes

The textarea tag helper renders `id` and `name` attributes based on the name of the model property passed to the `asp-for` attribute. It also renders any associated `data` attributes required for property validation.

The `MainText` property below has a maximum length of 300 applied to it:

```
[BindProperty, MaxLength(300)]
public string MainText { get; set; }

```

This is passed to the value of the `asp-for` attribute of the tag helper:

```
<textarea asp-for="MainText"></textarea>

```

The resulting HTML includes the validation attributes for unobtrusive validation as well as the appropriate `name` attribute value for model binding:

```
<textarea 
    data-val="true" 
    data-val-maxlength="The field MainText must be a string or array type with a maximum length of &#x27;300&#x27;." 
    data-val-maxlength-max="300" 
    id="MainText" 
    name="MainText">

```