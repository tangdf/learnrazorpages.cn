# The Validation Summary Tag Helper

The validation summary tag helper targets the HTML `div` element, and is used to render a summary of all form validation error messages.

| Attribute | Description |
| --- | --- |
| `validation-summary` | A `ValidationSummary` enumeration that specifies what validation errors to display. |

Possible `ValidationSummary` enumeration values are:

*   `None`, providing no validation summary
*   `ModelOnly`, summarising only model validation errors
*   `All`, summarising model _and_ property validation errors

For more information on the various types of error (model or property), see the [Validation topic](/razor-pages/validation).

## Notes

The validation summary tag helper is normally placed at the top of the form. Individual items that form the summary are displayed in an unordered list:

```
<div class="validation-summary-errors" data-valmsg-summary="true">
    <ul>
        <li>The FirstName field is required.</li>
        <li>The LastName field is required.</li>
        <li>The DateOfBirth field is required.</li>
    </ul>
</div>

```

You can include additional content to appear before the summary list by adding it to the content of the validation summary tag helper:

```
<div asp-validation-summary="All">
    <span>Please correct the following errors</span>
</div>

```

The additional content will be visible by default. If you don't want users to see the content unless the page is invalid, change the `validation-summary-valid` CSS class (which is injected into the `div` by the tag helper when the page is valid) so that it hides the `div` or its content:

```
.validation-summary-valid { display: none; }

```

or, suitable for the example above where the additional content is in a `span`:

```
.validation-summary-valid span { display: none; }

```

If you specify `None` as the value for the `validation-summary` attribute, an empty `div` is rendered.