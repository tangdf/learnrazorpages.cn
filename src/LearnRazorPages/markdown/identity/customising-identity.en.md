# Customising Identity in Razor Pages

The code for managing authentication in a Razor Pages application that is provided by the standard project template is a good starting point. However, chances are that you want to customise it to fit your own application needs. This article looks at the most common customisation requirements.

## Customising the Registration

A user in ASP.NET Identity is represented by the `ApplicationUser` class. It has very few properties by default - `UserName` and `Email`. The registration form in the template takes the value provided in the Email input and applies it to both the `UserName` and `Email` properties. The following steps illustrate how to enable a user to provide a different value for their username:

1.  Add a new property to the `InputModel` class in the _Register.cshtml.cs_ file for the user name:

    ```
    public class InputModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        ...

    ```

2.  Change the code in the `OnPostAsync` method so that the value for the user name is assigned from the new property:

    ```
    if (ModelState.IsValid)
    {
        var user = new ApplicationUser { UserName = Input.UserName, Email = Input.Email };
        ...

    ```

3.  Change the registration form in the _Register.cshtml_ file to accommodate the new property:

    ```
    <form asp-route-returnUrl="@Model.ReturnUrl" method="post">
        <h4>Create a new account.</h4>
        <hr />
        <div asp-validation-summary="All" class="text-danger"></div>
        <div class="form-group">
            <label asp-for="Input.UserName"></label>
            <input asp-for="Input.UserName" class="form-control" />
            <span asp-validation-for="Input.UserName" class="text-danger"></span>
        </div>
        ...

    ```

## Adding properties to the ApplicationUser

You probably want to capture more information from the user at the point of registration than just their email address and username. You do this by adding properties to the `ApplicationUser` class for storing the additional values, and then use [Entity Framework Core Migrations](http://www.learnentityframeworkcore.com/migrations) to apply the changes to the database so that the additional information can be stored. The following steps show how to add a first name, last name and date of birth fields:

1.  Add two `string` properties and a `DateTime` property to the `ApplicationUser` class:

    ```
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
    }

    ```

2.  Open the Package Manager Console and type the following command

    ```
    PM> add-migration AddedFirstNameLastNameBirthDate

    ```

    This will create a migration that, when applied, will modify the schema of the `AspNetUsers` table in the database to accommodate the additional data related to the properties that have been added.

3.  Apply the migration by typing the following command in the Package Manager Console:

    ```
    PM> update-database

    ```

4.  Add corresponding properties to the `InputModel` class (_Register.cshtml.cs_) together with appropriate annotations for display:

    ```
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
    [Display(Name = "Date of birth")]
    public DateTime BirthDate { get; set; }

    ```

5.  Change the code in the `OnPostAsync` handler method in the _Register.cshtml.cs_ file to assign values from the `Input` class to the new `ApplicationUser` properties:

    ```
    if (ModelState.IsValid)
    {
        var user = new ApplicationUser {
            UserName = Input.UserName,
            Email = Input.Email,
            FirstName = Input.FirstName,
            LastName = Input.LastName,
            BirthDate = Input.BirthDate
        };
        ...

    ```

6.  Finally, add appropriate additional fields to the form in _Register.cshtml_

    ```
    <div class="form-group">
        <label asp-for="Input.FirstName"></label>
        <input asp-for="Input.FirstName" class="form-control" />
        <span asp-validation-for="Input.FirstName" class="text-danger"></span>
    </div>
    <div class="form-group">
        <label asp-for="Input.LastName"></label>
        <input asp-for="Input.LastName" class="form-control" />
        <span asp-validation-for="Input.LastName" class="text-danger"></span>
    </div>
    <div class="form-group">
        <label asp-for="Input.BirthDate"></label>
        <input asp-for="Input.BirthDate" class="form-control" />
        <span asp-validation-for="Input.BirthDate" class="text-danger"></span>
    </div>

    ```

## Customising the Password Options

The following default password requirements may not suit your purposes:

*   The Password must be at least 6 and at max 100 characters long.
*   Passwords must have at least one non alphanumeric character.
*   Passwords must have at least one lowercase ('a'-'z').
*   Passwords must have at least one uppercase ('A'-'Z').

You can change these defaults via the Razor Pages options in the `ConfigureServices` method of the `Startup` class. Most of the options are booleans, requiring a `true` or `false` value. You can also specify a minimum length and a miunimum number of unique characters. The option names are self-explanatory:

```
services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredUniqueChars = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

```

## Customising the resources that require authentication

Finally, I look at how to protect resources from non-authenticated users. The default template prevents users from accessing the contents of the _Pages/Account/Manage_ folder, and the `Logout` action on the `AccountController`. These are protected by conventions established in the `ConfigureServices` method of the `Startup` class:

```
services.AddMvc()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AuthorizeFolder("/Account/Manage");
        options.Conventions.AuthorizePage("/Account/Logout");
    });

```

You can protect other resources by adding additional conventions using the `AuthorizeFolder` method to restrict access to a folder and all of its contents, or the `AuthorizePage` method to restrict access on a page-by-page basis. Alternatively, you can use the `AuthorizeAttribute` to protect a specific page. You do this by decorating the page model class for the page with `[Authorize]`:

```
[Authorize]
public class AboutModel : PageModel
{
...

```