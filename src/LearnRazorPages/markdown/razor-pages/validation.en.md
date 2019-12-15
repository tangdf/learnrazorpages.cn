# Validating User Input in Razor Pages

When you allow users to provide values that you then process, you need to ensure that the incoming values are of the expected data type, that they are within the permitted range and that required values are present. This process is known as input validation.

You can validate user input in two places in a web application: in the browser using client-side script or the browser's in-built data type validation; and on the server. However, you should only ever view client-side validation as a courtesy to the user because it is easily circumnavigated.

The MVC framework, on which Razor Pages is built, includes a robust validation framework that works against inbound model properties on the client-side **and** on the server.

The key players in the validation framework are:

*   DataAnnotation Attributes
*   Tag Helpers
*   jQuery Unobtrusive Validation
*   ModelState

## DataAnnotation Attributes

The primary building block of the validation framework is a set of attributes that inherit from `ValidationAttribute`. Most of these attributes reside in the `System.ComponentModel.DataAnnotations` namespace.

| Attribute | Description |
| --- | --- |
| `Compare` | Used to specify another form field that the value should be compared to for equality |
| `MaxLength` | Sets the maximum number of characters/bytes/items that can be accepted |
| `MinLength` | Sets the minimum number of characters/bytes/items that can be accepted |
| `Range` | Sets the minimum and maximum values of a range |
| `RegularExpression` | Checks the value against the specified regular expression |
| `Remote` | Enables client-side validation against a server-side resource, such as a database check to see if a username is already in use |
| `Required` | Specifies that a value must be provided for this property. Note that non-nullable value types such as `DateTime` and numeric values are treated as required by default and do not need this attribute applied to them |
| `StringLength` | Sets the maximum number of string characters allowed |

Apart from the `Remote` attribute, all the other attributes cause validation to occur on both the client and the server. The `Remote` attribute also differs from the others in that it doesn't belong to the `DataAnnotations` namespace. It is found in the `Microsoft.AspNetCore.Mvc` namespace.

<div class="alert-info" alert="">

There are a number of other attributes in the `DataAnnotations` namespace that inherit from `DataTypeAttribute`. These include `Phone`, `EmailAddress`, `Url`, `CreditCard` and so on. These do not form part of the validation framework. However, these attributes affect the `type` of the rendered `input` associated with the model property to which they have been applied. The [input tag helper](/razor-pages/tag-helpers/input-tag-helper) will render an appropriate HTML5 type based on the data type of the property to take advantage of any supporting features provided by the browser _which can include browser-specific type validation_. This validation is not consistent across browsers and cannot be relied upon. If you want to validate for a data type, you should implement your own custom solution.

</div>

Attributes are applied to properties on the inbound model - typically a PageModel or ViewModel:

```
public class UserModel : PageModel
{
    [BindProperty]
    [Required]
    [MinLength(6)]
    public string UserName { get; set; }

    [BindProperty]
    [Required, MinLength(6)]
    public string Password { get; set; }

    [BindProperty, Required, Compare(nameof(Password))]
    public string Password2 { get; set; }

    ...

```

Each attribute can be declared separately, or as a comma separated list, or a mixture of both.

## Client side validation

Client-side validation support is provided by the jQuery Unobtrusive Validation library, developed by Microsoft. It works with special HTML5 `data-*` attributes emitted by tag helpers. To see how that works, here is a simple tag helper-based form featuring the properties above:

```
<form method="post">
    <div>
        <input asp-for="UserName" />
        <span asp-validation-for="UserName"></span>
    </div>
    <div>
        <input asp-for="Password" />
        <span asp-validation-for="Password"></span>
    </div>
    <div>
        <input asp-for="Password2" />
        <span asp-validation-for="Password2"></span>
    </div>
    <div>
        <input type="submit" />
    </div>
</form>

```

This is how it renders as HTML:

```
<div>
    <input type="text" data-val="true" data-val-minlength="The field UserName must be a string or array type with a minimum length of &#x27;6&#x27;." data-val-minlength-min="6" data-val-required="The UserName field is required." id="UserName" name="UserName" value="" />
    <span class="field-validation-valid" data-valmsg-for="UserName" data-valmsg-replace="true"></span>
<div>
    <input type="text" data-val="true" data-val-minlength="The field Password must be a string or array type with a minimum length of &#x27;6&#x27;." data-val-minlength-min="6" data-val-required="The Password field is required." id="Password" name="Password" value="" />
    <span class="field-validation-valid" data-valmsg-for="Password" data-valmsg-replace="true"></span>
</div>
<div>
    <input type="text" data-val="true" data-val-equalto="&#x27;Password2&#x27; and &#x27;Password&#x27; do not match." data-val-equalto-other="*.Password" data-val-required="The Password2 field is required." id="Password2" name="Password2" value="" />
    <span class="field-validation-valid" data-valmsg-for="Password2" data-valmsg-replace="true"></span>
</div>
<div>
    <input type="submit" />
</div>

```

Validation is activated by the inclusion of the `data-val` attribute with a value of `true`. Various other `data-val-*` attributes are added as part of the tag helper rendering to specify the type of validation required and the error message, which can be customised as part of the attribute declaration:

```
[Compare(nameof(Password)), ErrorMessage ="Make sure both passwords are the same")]
public string Password2 { get; set; }

```

```
<input type="text" 
    data-val="true" 
    data-val-equalto="Make sure both passwords are the same" 
    data-val-equalto-other="*.Password" 
    data-val-required="The Password2 field is required." 
    id="Password2" name="Password2" value="" />

```

You must also include the jQuery Unobtrusive Validation library within the page containing the form for client side validation to work. This is most easily accomplished by the inclusion of the __ValidationScriptsPartial.cshtml_ file within the page:

```
@section scripts{
    @await Html.PartialAsync("_ValidationScriptsPartial")
}

```

Finally, validation message or summary tag helpers are required to provide somewhere for the error message to be displayed. Without these, any attempt to submit a form that fails validation will not succeed, but without any visual clues as to why, and will leave the user confused.

## Server side validation

Client side validation will not take place if you don't include __ValidationScriptsPartial.cshtml_ in your form, or if you don't use tag helpers to generate the HTML for your form controls.

There are a number of other ways to circumvent client-side validation:

*   Use the browser's developer tools to change `data-val="true"` to `data-val="false"`
*   Save a copy of the form to your desktop and remove the validation scripts
*   Use Postman, Fiddler or Curl to post the form values directly
*   etc...

Because it is so easy to circumvent client-side validation, server-side validation is included as part of the validation framework. Once property values have been bound, the framework looks for all validation attributes on those properties and executes them. Any failures result in an entry being added to a `ModelStateDictionary` - a dictionary-like structure where validation errors are stored. This is made available in the PageModel class via `ModelState`, which has a property named `IsValid` that returns `false` if any of the validation tests fail:

```
public IActionResult  OnPost()
{
    if (ModelState.IsValid)
    {
        // do something
        return RedirectToPage("Contact"));
    }
    else
    {
        return Page();
    }
}

```

The snippet above illustrates the most common pattern for dealing with validation in an `OnPost` handler - query `ModelState`'s `IsValid` property, and if it returns `true`, process the form otherwise redisplay the form, and let the framework take care of extracting error messages from `ModelState` and passing them to the validation message and/or summary tag helpers.