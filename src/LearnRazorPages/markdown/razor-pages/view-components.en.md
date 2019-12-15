# View Components in Razor Pages

View Components perform a similar role to [Partial Pages](/razor-pages/partial-pages). They represent reusable snippets of UI that can help break up and simplify complex layouts, or that can be used in multiple pages. View components are recommended instead of partial pages whenever any form of logic is required to obtain data for inclusion in the resulting HTML snippet, specifically calls to an external data source such as a database or web service. Examples of use cases for view components include data-driven menus, tag clouds and shopping basket widgets.

## Elements of a view component

View components are comprised of a class file and a _.cshtml_ view file. The class file contains the logic for generating the model, and the view file contains the template that's used to generate the HTML to be plugged in to the calling page.

The class file must conform to the following rules:

*   It must derive from the `ViewComponent` class
*   It must have "ViewComponent" as a suffix to the class name or it must be decorated with the `[ViewComponent]` attribute (or derive from a class that's decorated with the `[ViewComponent]` attribute)
*   It must implement a method named `InvokeAsync` which must have a return type of `IViewComponentResult`. Typically, this is satisfied by a `return View(...)` statement in the method.

By default, the view file is named _default.cshtml_. You can specify an alternative name for the view file by passing it to the `return View(...)` statement. The view file's placement within the application's file structure is important, because the framework searches pre-defined locations for it:

`/Pages/Components/<component name>/Default.cshtml`
`/Views/Shared/Components/<component name>/Default.cshtml`

The component name is the name of the view component class without the _ViewComponent_ suffix (if it is applied). For a Razor Pages only site, the recommended location for view component views is the _/Pages/Components/_ directory. The path that begins with _/Views_ should only really be used if you are creating a hybrid Razor Pages/MVC application.

## Walkthrough

The following walkthrough will result in two example view components being created. One will call into an external web service to obtain a list of people, and will display their names. The other will take a parameter representing the ID of one person whose details will be obtained from an external web service and then displayed in a widget.

The service APIs used in this example are hosted at [JSONPlaceholder](https://jsonplaceholder.typicode.com) which provides free JSON APIs for development and testing purposes.

The view components will not be responsible for making calls to the external APIs. This task will be performed in a separate service class which will be injected into the view components via the built-in [Dependency Injection](/advanced/dependency-injection) framework.

1.  Create a new Razor Pages site named _RazorPages_ using Visual Studio or the [command line](/first-look#try-it-out-creating-your-first-razor-pages-application).

2.  Add a new C# class file named _Domain.cs_ to the root folder of the application and replace any existing content with the following:

    ```
    namespace RazorPages
    {
        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Website { get; set; }
            public Address Address { get; set; }
            public Company Company { get; set; }
        }

        public class Address
        {
            public string Street { get; set; }
            public string Suite { get; set; }
            public string City { get; set; }
            public string Zipcode { get; set; }
            public Geo Geo { get; set; }
        }
        public class Company
        {
            public string Name { get; set; }
            public string Catchphrase { get; set; }
            public string Bs { get; set; }
        }
        public class Geo
        {
            public float Lat { get; set;}
            public float Lng { get; set; }
        }
    }

    ```

    These classes map to the structure of the objects represented by the JSON provided by the API being used for this example.

3.  Add a new folder named _Services_ to the root folder
    ![Services folder](/images/2017-08-05_21-19-06.png)

4.  Add a new C# class file named _IUserService.cs_ to the _Services_ folder. Replace any existing content with the following code:

    ```
    using System.Collections.Generic;
    using System.Threading.Tasks;

    namespace RazorPages.Services
    {
        public interface IUserService
        {
            Task<List<User>> GetUsersAsync();
            Task<User> GetUserAsync(int id);
        }
    }

    ```

    This is the interface that specifies the operations offered by the service

5.  Add another new C# class file named _UserService.cs_ to the _Services_ folder and replace any existing content with the following:

    ```
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    namespace RazorPages.Services
    {
        public class UserService : IUserService
        {
            public async Task<List<User>> GetUsersAsync()
            {
                using (var client = new HttpClient())
                {
                    var endPoint = "https://jsonplaceholder.typicode.com/users";
                    var json = await client.GetStringAsync(endPoint);
                    return JsonConvert.DeserializeObject<List<User>>(json);
                }
            }

            public async Task<User> GetUserAsync(int id)
            {
                using (var client = new HttpClient())
                {
                    var endPoint = $"https://jsonplaceholder.typicode.com/users/{id}";
                    var json = await client.GetStringAsync(endPoint);
                    return JsonConvert.DeserializeObject<User>(json);
                }
            }
        }
    }

    ```

    This class represents an implementation of the `IUserService` interface.

6.  Add a folder names _ViewComponents_ to the root of the application. Then add a new file to that folder, name it _UsersViewComponent.cs_ and replace any existing content with the following:

    ```
    using Microsoft.AspNetCore.Mvc;
    using RazorPages.Services;
    using System.Threading.Tasks;

    namespace RazorPages.ViewComponents
    {
        public class UsersViewComponent : ViewComponent
        {
            private IUserService _userService;

            public UsersViewComponent(IUserService userService)
            {
                _userService = userService;
            }

            public async Task<IViewComponentResult> InvokeAsync()
            {
                var users = await _userService.GetUsersAsync();
                return View(users);
            }
        }
    }

    ```

    This is the code part of the view component. It makes use of the built-in dependency injection system to resolve the implementation of `IUserService` which is injected into the constructor of the view component class. The `InvokeAsync` method obtains a `List<User>` from the service and passes it to the view.

7.  Create a folder named _Components_ in the _Pages_ folder and then add another folder named _Users_ to the newly created _Components_ folder. Add a new file named _default.cshtml_ to the _Users_ folder. The resulting folder and file hierarchy should look like this:

    ![ViewComponents view file](/images/2017-08-06_20-06-21.png)

8.  Replace the code in _default.cshtml_ with the following:

    ```
    @model List<RazorPages.User>
    <h3>Users</h3>
    <ul>
        @foreach (var user in Model)
        {
            <li>@user.Name</li>
        }
    </ul>

    ```

    This is the view, and completes the view component. Notice that the view accepts a model of type `List<User>` via the `@model` directive, which is type passed to the view from the `InvokeAsync` method.

9.  Open the _Startup.cs_ file and add `using RazorPages.Services;` to the `using` directives at the top of the file. Then amend the `ConfigureServices` method so that the code looks like this:

    ```
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();
        services.AddTransient<IUserService, UserService>();
    }

    ```

    This step registers the `IUserService` with the dependency injection system, and specifies that `UserService` is the actual implementation to use.

10.  Open the _Layout.cshtml_ file and locate the content between the `<nav>` section and the `<environment>` tag helper that currently looks like this:

    ```
    <div class="container body-content">

        @RenderBody()
        <hr />
        <footer>
            <p>&copy; 2017 - RazorPages</p>
        </footer>
    </div>

    ```

    Change the content as follows:

    ```
    <div class="container body-content">
        <div class="col-md-9">
            @RenderBody()
        </div>
        <div class="col-md-3">
            @await  Component.InvokeAsync("Users")
        </div>
    </div>
    <hr />
    <footer class="container">
        <p>&copy; 2017 - RazorPages</p>
    </footer>

    ```

    This converts the layout for the site to 2 columns, with page content displayed in the left hand column and a Users widget displayed in the right hand column. It uses the `Component.InvokeAsync` method to render the output of the view component to the page. The string which is passed to the method represents the name of the view component (the class name without the "ViewComponent" suffix).

11.  Run the site to ensure that all is working. The list of users should appear on the right hand side of every page:

    ![User View Component](/images/07-08-2017-08-24-53.png)

## Taghelper and passing parameters

The second example will demonstrate the use of a tag helper instead of calling the `Component.InvokeAsync` method. It will also demonstrate passing parameters to the view component.

1.  Add a new C# class file to the _ViewComponents_ folder and name it _UserViewComponent.cs_. Replace any existing content with the following:

    ```
    using Microsoft.AspNetCore.Mvc;
    using RazorPages.Services;
    using System.Threading.Tasks;

    namespace RazorPages.ViewComponents
    {
        public class UserViewComponent : ViewComponent
        {
            private IUserService _userService;

            public UserViewComponent(IUserService userService)
            {
                _userService = userService;
            }

            public async Task<IViewComponentResult> InvokeAsync(int id)
            {
                var user = await _userService.GetUserAsync(id);
                return View(user);
            }
        }
    }

    ```

    This is the code part of the view component. The only difference between this component and the previous one is that the `InvokeAsync` method expects a parameter of type `int` to be passed, which is then passed to the service method.

2.  Add a new folder to the _/Pages/Components_ folder named _User_. Add a Razor file to the folder named _default.cshtml_. The structure of the _Components_ folder should now look like this:

    ![user view component](/images/07-08-2017-08-50-21.png)

3.  Replace any existing content in the new _default.cshtml_ file with the following:

    ```
    @model RazorPages.User

    <h3>Featured User</h3>
    <div>
        <strong>@Model.Name</strong><br />
        @Model.Website<br />
        @Model.Company.Name<br />
    </div>

    ```

4.  Open the _ViewImports.cshml_ file and add the following line to the existing code:

    ```
    @addTagHelper *, RazorPages

    ```

    This registers the view component tag.

5.  Replace the call to `@await Component.InvokeAync("Users")` in the layout file with the following:

    ```
    <vc:user id="new Random().Next(1, 10)"></vc:user>

    ```

    The name of the view component is specified in the tag helper, along with the parameter for the `InvokeAsync` method. In this case, the `Random` class is used to return any number from 1-10 each time the component is invoked, resulting in a user being selected randomly each time the page is displayed.

6.  Run the application to test that the component is working, and refresh a few times to see different users' details being displayed:

    ![user view component](/images/07-08-2017-09-07-45.png)

If you prefer to use the `Component.InvokeAsync` method, parameters are passed as a second argument:

```
@await Component.InvokeAsync("User", new Random().Next(1, 10))

```