# The Label Tag Helper

The label tag helper generates appropriate `for` attribute values and content based on the PageModel property that is assigned to it. It has just one attribute:

| Attribute | Description |
| --- | --- |
| `for` | An expression to be evaluated against the current page model |

## Notes

The label tag helper is intended to work alongside the [Input tag helper](/razor-pages/tag-helpers/input-tag-helper). It takes a property of the PageModel as a parameter to the `asp-for` attribute and renders the name of the property as a value for the label's `for` attribute and as the content of the label tag. Assuming that the PageModel has a property named "Email:

```
<label asp-for="Email"></label>

```

This renders as

```
<label for="Email">Email</label>

```

You can use the Data Annotations `Display` attribute to change the content that is rendered:

```
[Display(Name="Email Address")]
public string EmailAddress { get; set; }

```

```
<label for="EmailAddress">Email Address</label>

```