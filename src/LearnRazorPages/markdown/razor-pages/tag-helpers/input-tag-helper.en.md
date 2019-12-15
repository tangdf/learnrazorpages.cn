# The Input Tag Helper

The Input tag helper generates appropriate `name` and `id` attribute values based on the `PageModel` property that is assigned to it. It will also generate an appropriate value for the `type` attribute, based on the property's meta data. The tag helper will also emit attributes that provide support for unobtrusive client-side validation.

The input tag helper has one attribute:

| Attribute | Description |
| --- | --- |
| `for` | An expression to be evaluated against the current PageModel, usually a PageModel property name |

## Notes

Although it only has one attribute, the input tag helper is quite powerful. It examines the meta data of the type that is passed to the `for` attribute, including any Data Annotations that have been applied to the property and emits HTML accordingly.

Here is a class with various property types and data annotation attributes applied to it:

```
public class Member
{
    public int PersonId { get; set; }
    public string Name { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [DataType(DataType.PhoneNumber)]
    public string Telephone { get; set; }
    [Display(Name="Date of Birth")]
    public DateTime DateOfBirth { get; set; }
    public decimal Salary { get; set; }
    [Url]
    public string Website { get; set; }
    [Display(Name="Send spam to me")]
    public bool SendSpam { get; set; }
    public int? NumberOfCats { get; set; }
}

```

This is then applied as a property to a [PageModel](/razor-pages/pagemodel) for a page called `Register.cshtml`:

```
public class RegisterModel : PageModel
{
    [BindProperty]
    public Member Member { get; set; }

    public void OnGet()
    {
    }
}

```

The properties of the model are applied to various input tag helpers in the Razor file:

```
<form method="post">
    <input asp-for="Member.PersonId" /><br />
    <input asp-for="Member.Name" /><br />
    <input asp-for="Member.Email" /><br />
    <input asp-for="Member.Password" /><br />
    <input asp-for="Member.Telephone" /><br />
    <input asp-for="Member.Website" /><br />
    <input asp-for="Member.DateOfBirth" /><br />
    <input asp-for="Member.Salary" /><br />
    <input asp-for="Member.SendSpam" /><br />
    <input asp-for="Member.NumberOfCats" /><br />
    <button>Submit</button>
</form>

```

And this generates the following HTML:

```
<form method="post">
    <input type="number" data-val="true" data-val-required="The PersonId field is required." id="Member_PersonId" name="Member.PersonId" value="" /><br />
    <input type="text" id="Member_Name" name="Member.Name" value="" /><br />
    <input type="email" data-val="true" data-val-email="The Email field is not a valid e-mail address." id="Member_Email" name="Member.Email" value="" /><br />
    <input type="password" id="Member_Password" name="Member.Password" /><br />
    <input type="tel" id="Member_Telephone" name="Member.Telephone" value="" /><br />
    <input type="url" data-val="true" data-val-url="The Website field is not a valid fully-qualified http, https, or ftp URL." id="Member_Website" name="Member.Website" value="" /><br />
    <input type="datetime-local" data-val="true" data-val-required="The Date of Birth field is required." id="Member_DateOfBirth" name="Member.DateOfBirth" value="" /><br />
    <input type="text" data-val="true" data-val-number="The field Salary must be a number." data-val-required="The Salary field is required." id="Member_Salary" name="Member.Salary" value="" /><br />
    <input data-val="true" data-val-required="The Send spam to me field is required." id="Member_SendSpam" name="Member.SendSpam" type="checkbox" value="true" /><br />
    <input type="number" id="Member_NumberOfCats" name="Member.NumberOfCats" value="" /><br />
    <button>Submit</button>
    <input name="__RequestVerificationToken" type="hidden" value="CfDJ8I..." />
    <input name="Member.SendSpam" type="hidden" value="false" />
</form>

```

### Type attribute based on data type

The data type of the property is taken into account when the input tag helper determines the `type` attribute's value to apply. HTML5 types are used wherever possible to take advantage of features provided by supporting browsers. They behave as `type="text"` in browsers that don't support the rendered HTML5 type:

| .NET Type | Input type |
| --- | --- |
| bool | `checkbox` |
| byte, short, int, long | `number` |
| decimal, double, float | `text`<sup>1</sup> |
| string | `text` |
| DateTime | `datetime-local` |

1.  Despite the fact that the input type is set to`text` for these data types, they will still be validated for numeric values.

### Type attribute based on data annotations

Data annotation attributes applied to properties are also a determining factor for the selection of the input tag helper's `type` attribute. The following table provides the `DataType` enumeration values, with equivalent annotations in parentheses if applicable:

| Annotation | Input type |
| --- | --- |
| EmailAddress (EmailAddress) | `email` |
| PhoneNumber (Phone) | `tel` |
| Password | `password` |
| Url (Url) | `url` |
| Date, Time, DateTime, Duration | `datetime-local` |
| HiddenInput<sup>1</sup> | `hidden` |

All other `DataType` enumeration values (`CreditCard`, `Currency`, `Html`, `ImageUrl`, `MultilineText`, `PostCode` and `Upload`) result in `type="text"` being applied.

1.  The `HiddenInput` attribute requires a reference to `Microsoft.AspNetCore.Mvc`. All other attributes reside in the `System.ComponentModel.DataAnnotations` namespace.

### Validation support

The input tag helper also emits `data` attributes that work with ASP.NET's Unobtrusive Client Validation framework (an extension to jQuery Validation). The validation rules are specified in `data-val-*` attributes and are calcuated from the data types and any data annotation attributes that have been applied to model properties.

The following attributes are designed for validation purposes and will result in appropriate `data-val` error messages and other attributes being generated:

*   Compare
*   MaxLength
*   MinLength
*   Range
*   Required <sup>1</sup>
*   StringLength

1.  Non-nullable properties are treated as `Required`.

In addition, the following annotation attributes generate `data-val` attributes:

*   EmailAddress / DataType.EmailAddress
*   Phone / DataType.PhoneNumer
*   Url / DataType.Url

For further information on the validation attributes, see [Validation](/razor-pages/validation).