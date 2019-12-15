# Model Binding

Model Binding in Razor Pages is the process that takes values from HTTP requests and maps them to [handler method](/razor-pages/handler-methods) parameters or [PageModel](/razor-pages/pagemodel) properties. Model binding reduces the need for the developer to manually extract values from the request and then assign them, one by one, to variables or properties for later processing. This work is repetitive, tedious and error prone, mainly because request values are usually only exposed via string-based indexes.

## The Problem

To illustrate the role that model binding plays, create a new Razor page with a page model and name it _ModelBinding.cshtml_. Change the code in the content page to the following:

```
@page 
@model ModelBindingModel
@{
}

<h3>@ViewData["confirmation"]</h3>
<form class="form-horizontal" method="post">
    <div class="form-group">
        <label for="Name" class="col-sm-2 control-label">Name</label>
        <div class="col-sm-10">
            <input type="text" class="form-control" name="Name">
        </div>
    </div>
    <div class="form-group">
        <label for="Email" class="col-sm-2 control-label">Email</label>
        <div class="col-sm-10">
            <input type="text" class="form-control" name="Email">
        </div>
    </div>
    <div class="form-group">
        <div class="col-sm-offset-2 col-sm-10">
            <button type="submit" class="btn btn-default">Register</button>
        </div>
    </div>
</form>

```

It represents a standard HTML form accepting a name and a email address (such as might be used to capture requests for information, for example), with a confirmation message at the top of the page. When the form is completed and submitted, the values are sent in the request body in name/value pairs, where the "name" represents the `name` attribute of the input, and the value is what was supplied by the user. You can see this in the Network tab of your preferred browser if it supports developer tools:

![Posted values](/images/2017-07-22_16-52-57.png)

Add the following handler method code to the PageModel class in _ModelBinding.cshtml.cs_:

```
public void OnPost()
{
    var name = Request.Form["Name"];
    var email = Request.Form["Email"];
    ViewData["confirmation"] = $"{name}, information will be sent to {email}";
}

```

This represents the traditional way that values in server-side code are processed in many web frameworks. The appropriate `Request` collection is accessed by string-based index and then values from the collection are assigned to local variables for further processing.

Launch the page in a browser and enter some values into the form. Then submit it and you should see those values included in the confirmation message:

This approach is sustainable for small forms, but if you are dealing with large forms, such as one representing an order for multiple items complete with shipping details, for example, the assignment code can become very tedious. And, because development tools provide no code-completion or Intellisense support for string indices, it is fairly easy to mistype `Request.Form["Email"]` as `Request.Form["Emial"]`, thereby introducing a subtle but damaging bug that may be difficult to track down among 30 or 40 other form fields.

## Binding Posted Form Values To Handler Method Parameters

Razor Pages provides two approaches to leveraging model binding. The first approach involves adding parameters to the handler method. The parameters are named after the form fields, and given an appropriate type for the expected data. To see this approach in action, remove the assignment code in the `OnPost` handler method and add two parameters to the method:

```
public void OnPost(string name, string email)
{
     ViewData["confirmation"] = $"{name}, information will be sent to {email}";
}

```

When the form is posted, the Razor Pages framework calls the `OnPost` method and sees that it has two parameters. It extracts posted form values that match the names of the parameters and automatically assigns the values from the form to the parameters. There is no need for any assignment code.

## Binding Posted Form Values to PageModel Properties

The previous approach is suitable when the values are not needed outside of the handler method to which the parameters belong. The second approach is more suitable if you need to access the values outside of the handler method (for display on the page, perhaps). This approach involves adding public properties to the [PageModel](/razor-pages/pagemodel) (or to a `@functions` block if you don't want to use the PageModel approach) and then decorating them with the `BindProperty` attribute. To try this out, alter the PageModel code as follows:

```
public class ModelBindingModel : PageModel
{
    [BindProperty]
    public string Name { get; set; }
    [BindProperty]
    public string Email { get; set; }
    public void OnGet()
    {
    }

    public void OnPost()
    {
        ViewData["confirmation"] = $"{Name}, information will be sent to {Email}";
    }
}

```

Note that the case of the variables in the format string has altered to refer to the public property names. Model binding itself is not case sensitive. Now when you run the page, the result is exactly the same as before:

![Model binding to page model properties](/images/2017-07-22_17-18-09.png)

## Binding Data From GET Requests

The same options apply if you want to bind data from `GET` requests (which is appended to the URL as a query string). Binding to handler method parameters is automatic and requires no additional configuration. However, by default, only values that form part of a `POST` request are considered for model binding when you use the `BindProperty` attribute. The `BindProperty` attribute has a property called `SupportsGet`, which is `false` by default. You have to set this to `true` to opt in to model binding to PageModel properties on `GET` requests:

```
public class ModelBindingModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Email { get; set; }
    [[BindProperty(SupportsGet = true)]
    public string Password { get; set; }
    public void OnGet()
    {
        ViewData["welcome"] = $"Welcome {Email}";
    }
}

```

<div class="alert alert-warning">

Note: Obviously it is not a good idea to create a login form that supports `GET`. Form values will appear in the URL, which could well be a security breach.

</div>

## Binding Route Data

So far the examples have featured how model binding works with form values. It also works with [Route Data](/razor-pages/routing#route-data), which is part of the routing system that Razor Pages uses. To test his, alter the code in _ModelBinding.cshtml_ as follows:

```
@page "{id}"
@model ModelBindingModel
@{
}

<h3>Id = @ViewData["id"]</h3>

```

A route parameter named `id` has been added and the content of the `h3` heading has been altered to print the value of a `ViewData` entry named `id`.

Next, remove the public properties from the PageModel and add a parameter named `id` of type `int` to the `OnGet` handler method, and in the body of the method, assign its value to `ViewData`:

```
public class ModelBindingModel : PageModel
{
    public void OnGet(int id)
    {
        ViewData["id"] = id;
    }
}

```

Once again, model binding takes care of assigning the value in the route to the handler method parameter. This also works for public properties on the PageModel in exactly the same way as for form values:

```
public class ModelBindingModel : PageModel
{
    [BindProperty]
    public int Id { get; set; }

    public void OnGet()
    {
        ViewData["id"] = Id;
    }

    public void OnPost()
    {

    }
}

```

## Binding Complex Objects

So far, you have seen how to use model binding to populate simple properties. As the number of form fields grows, the PageModel class will start to creak with either a long list of properties, all decorated with the `BindProperty` attribute, or a large number of parameters applied to a handler method. Fortunately model binding also works with complex objects. So the properties to be bound can be wrapped in an object that can be exposed as a property of the PageModel or a parameter for the handler method. Here's a class named `Login` that represents the form fields from the previous examples:

```
public class Login
{
    public string Email { get; set; }
    public string Password { get; set; }
}

```

Now this can be added as a property on the PageModel class:

```
public class ModelBindingModel : PageModel
{
    [BindProperty]
    public Login Login { get; set; }
    public void OnGet()
    {
    }

    public void OnPost()
    {
        ViewData["welcome"] = $"Welcome {Login.Email}";
    }
}

```

Or it can be applied as a parameter to the `OnPost` method depending on your usage needs:

```
public class ModelBindingModel : PageModel
{
    public void OnGet()
    {
    }

    public void OnPost(Login Login)
    {
        ViewData["welcome"] = $"Welcome {Login.Email}";
    }
}

```

## Binding Simple Collections

The next code example shows a form where the user is allowed to select more than one option. In this case, the user is invited to specify which film categories they like:

```
<form class="form-horizontal" method="post">
    <div class="form-group">
        <label for="CategoryId" class="col-sm-2 control-label">Which types of film do you like? (Tick all that apply)</label>
        <div class="col-sm-10">
            <input type="checkbox" name="CategoryId" value="1"> Factual<br />
            <input type="checkbox" name="CategoryId" value="2"> Horror<br />
            <input type="checkbox" name="CategoryId" value="3"> Historical<br />
            <input type="checkbox" name="CategoryId" value="4"> SciFi<br />
            <input type="checkbox" name="CategoryId" value="5"> Comedy<br />
            <input type="checkbox" name="CategoryId" value="6"> Fantasy<br />
        </div>
    </div>
    <div class="form-group">
        <div class="col-sm-offset-2 col-sm-10">
            <button type="submit" class="btn btn-default">Submit</button>
        </div>
    </div>
</form>

```

The form includes mutiple checkboxes, each with the same `name`attribute values: `CategoryId`. The correct type for capturing the posted values is a collection of integers - an array, `List<int>`, `ICollection<int>` etc. The code for binding to a handler method parameter and passing the posted values to `ViewData` looks like this:

```
public void OnPost(int[] categoryId)
{
    ViewData["categoryId"] = categoryId;
}

```

If there are no values posted, `categoryId` will be `null` as will `ViewData["categoryId"]`. Therefore must test for `null` (as well as casting to the relevant type):

```
@if (ViewData["categoryId"] != null)
{
<h3>You selected the following categories: @string.Join(",", (int[])ViewData["categoryId"])</h3>
}

```

If you choose to bind to a PageModel property, you can initiate the collection as part of its declaration:

```
public class ModelBindingModel : PageModel
{
    [BindProperty]
    public int[] CategoryId { get; set; } = new int[0];

    public void OnPost()
    {

    }
}

```

Then you can use `Any()` to check whether the collection has been populated:

```
@if (Model.CategoryId.Any())
{
<h3>You selected the following categories: @string.Join(",", Model.CategoryId)</h3>
}

```

## Binding Complex Collections

The model binder also supports binding to collections of complex objects. The following class represents a contact in an address book, perhaps:

```
public class Contact
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}

```

You might want to provide a form that enables a user to submit multiple contacts in one go. As in previous examples, you can do this via a handler method parameter:

```
public void OnPost(List<Contact> contacts)
{
    // process the contacts
}

```

Here is how the form might be designed to cater for this scenario:

```
<form class="form-horizontal" method="post">
    <table class="table table-striped">
            <tr>
                <th>First Name</th>
                <th>Last Name</th>
                <th>Email</th>
            </tr>
        @for (var i = 0; i < 5; i++)
        {
            <tr>
                <td>
                    <input type="text" name="[@i].FirstName" />
                </td>
                <td>
                    <input type="text" name="[@i].LastName" />
                </td>
                <td>
                    <input type="text" name="[@i].Email" />
                </td>
            </tr>
        }
    </table>
    <div class="form-group">
        <div class="col-sm-offset-2 col-sm-10">
            <button type="submit" class="btn btn-default">Submit</button>
        </div>
    </div>
</form>

```

![Multiple contacts](/images/2017-07-25_08-17-46.png)

The key to complex object binding is the sequential index in square brackets that is added to the form field's `name` attribute e.g `[0].FirstName`. The code below shows the generated HTML for the first 3 rows in the form:

```
<tr>
    <td>
        <input type="text" name="[0].FirstName" />
    </td>
    <td>
        <input type="text" name="[0].LastName" />
    </td>
    <td>
        <input type="text" name="[0].Email" />
    </td>
</tr>
<tr>
    <td>
        <input type="text" name="[1].FirstName" />
    </td>
    <td>
        <input type="text" name="[1].LastName" />
    </td>
    <td>
        <input type="text" name="[1].Email" />
    </td>
</tr>
<tr>
    <td>
        <input type="text" name="[2].FirstName" />
    </td>
    <td>
        <input type="text" name="[2].LastName" />
    </td>
    <td>
        <input type="text" name="[2].Email" />
    </td>
</tr>

```

In this example, for format that is used for the form field names is `[index].propertyname`. The process also works with `parametername[index].propertyname` if you prefer e.g.:

```
@for (var i = 0; i < 5; i++)
{
    <tr>
        <td>
            <input type="text" name="Contacts[@i].FirstName" />
        </td>
        <td>
            <input type="text" name="Contacts[@i].LastName" />
        </td>
        <td>
            <input type="text" name="Contacts[@i].Email" />
        </td>
    </tr>
}

```

When the form is submitted, a collection of five (in this example) `Contact` objects is instantiated and populated with the posted values. If the user only provides values for the first three contacts, the final two will have their properties set to the default for the type - `null` for strings.

The same approach works when you bind to a PageModel property. However, you can also use the `asp-for` attribute of an [input tag helper](/razor-pages/tag-helpers/input-tag-helper):

```
@for (var i = 0; i < 5; i++)
{
    <tr>
        <td>
            <input type="text" asp-for="Contacts[i].FirstName" />
        </td>
        <td>
            <input type="text" asp-for="Contacts[i].LastName" />
        </td>
        <td>
            <input type="text" asp-for="Contacts[i].Email" />
        </td>
    </tr>
}

```

```
public class ModelBindingModel : PageModel
{
    [BindProperty]
    public List<Contact> Contacts { get; set; } 

    public void OnPost()
    {
        // process the contacts
    }
}

```

## Preventing Overposting or Mass Assignment attacks

When you add the `BindProperty` attribute to a class, all properties in that class are automatically included in model binding. That may not be what is needed, particuarly when working with Entity Framework model classes.

For example, you might choose to have an `IsDeleted` property on your entities to allow "soft deletes" (i.e. a flag that specifies the status of a record rather than the permanent - and irrevocable - removal of the record from the database). Only admins are allowed to set this property so you don't include an _IsDeleted_ field in the edit form for normal users:

```
<form class="form-horizontal" method="post">
    <input type="hidden" asp-for="ContactId">
    <div class="form-group">
        <label asp-for="Name" class="col-sm-2 control-label"></label>
        <div class="col-sm-10">
            <input type="text" class="form-control" asp-for="Name">
        </div>
    </div>
    <div class="form-group">
        <label asp-for="Email" class="col-sm-2 control-label"></label>
        <div class="col-sm-10">
            <input type="text" class="form-control" asp-for="Email">
        </div>
    </div>
    <div class="form-group">
        <div class="col-sm-offset-2 col-sm-10">
            <button type="submit" class="btn btn-default">Edit</button>
        </div>
    </div>
</form>

```

However, it is trivial for a reasonably competent HTML-savvy user to manipulate the form (using the standard browser developer tools, for example) to include an `IsDeleted` property and to submit that to the server. The value will be processed as part of a legitimate edit operation. This is known as a mass assignment or an over posting attack.

For this reason, you are advised to be careful when using model binding with complex types. If they include properties that should not be set by an unauth orised user, you should only include the properties that can be set, either as individual properties on the PageModel, or wrapped in a ViewModel class.