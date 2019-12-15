# The Validation Message Tag Helper

The validation message tag helper targets the HTML `span` element, and is used to render property-specific validation error messages.

| Attribute | Description |
| --- | --- |
| `validation-for` | An expression to be evaluated against the current PageModel, usually a PageModel property name |

## Notes

Validation message tag helpers display both client-side and server-side validation error messages. They apply a CSS class named `field-validation-valid` to the span, which is changed to `field-validation-error` in the event that the form value is invalid. These styles are added to any others that you specify via the `class` attribute.

```
<span asp-validation-for="FirstName" class="myclass"></span>

```

renders as

```
<span class="myclass field-validation-valid" data-valmsg-for="FirstName" data-valmsg-replace="true"></span>

```

It is customary to position the tag helper as close to the control that it refers to so that it is easier for users to relate the error message to the relevant form value.