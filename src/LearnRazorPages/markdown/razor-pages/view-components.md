# Razor 页面视图组件


视图组件与[局部页面](/razor-pages/partial-pages)有着类似的作用，它们代表可重复使用的 UI 片段，可以帮助分解和简化复杂的布局，并且可以在多个页面中重使。如果需要一定的逻辑来获取包含在HTML片段中的数据，比如调用外部数据源（如数据库或Web服务），建议使用视图组件而不是局部页面。视图组件的用例包括数据驱动的菜单、标签云和购物车小部件。

## 视图组件元素

视图组件由类文件和  _.cshtml_ 视图文件组成。类文件包含用于生成模型的逻辑，而视图文件包含用于生成将插入到调用页面的HTML模板。

类文件必须符合以下规则：

*   必须继承自`ViewComponent` 类。
*   必须以`ViewComponent`作为类名称的后缀，或者必须用`[ViewComponentAttribute]`（或继承自`ViewComponentAttribute`）标记修饰。
*   必须实现返回类型为`IViewComponentResult`的`InvokeAsync`方法。通常，通过`return View(...)`方法来实现。

默认情况下，视图文件名为 _default.cshtml_ 。您可以通过将视图文件传递给`return View(...)`语句来指定替换的名称。视图文件在应用程序文件结构中的位置很重要，因为框架预定义了搜索的位置：

`/Pages/Components/<component name>/Default.cshtml`
`/Views/Shared/Components/<component name>/Default.cshtmlll`

组件名称是去掉了类名的`ViewComponent`后缀（如果使用了该规则）。对于 只Razor 页面框架的站点，视图组件视图的推荐位置是 _/Pages/Components/_ 目录。只有在创建 Razor 页面和 MVC 混合应用程序时，才能推荐使用以 _/Views_ 开头的路径。

## 演示

以下将演示创建两个视图组件的示例。一个会调用外部的 Web 服务来获取人员名单，并显示他们的名字。另外通过代表一个人的ID参数，从 Web 服务中获取这个人的详细信息，然后显示在小部件中。

本示例中使用的服务API托管在  [JSONPlaceholder](https://jsonplaceholder.typicode.com) 上，它可以为开发和测试提供免费的 JSON API。

视图组件不负责调用外部API。这个任务将在一个单独的服务类中执行，该类将通过内置的 [依赖注入](/advanced/dependency-injection) 框架注入到视图组件中。

1. 使用Visual Studio或 [命令行](/first-look) 创建一个新的叫 _RazorPages_ 的 Razor 页面网站。

2. 在应用程序的根目录中添加 _Domain.cs_ C#类文件，并文件中所有内容替换为下面内容：

    ```csharp
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
    
    这些类映射到由本示例使用API返回的JSON所表示对象结构。

3.  在根目标添加 _Services_ 文件夹：

    ![Services folder](/images/2017-08-05_21-19-06.png)

4. 在 _Services_ 文件夹中添加 _IUserService.cs_ 类。将内容替换为如下内容：

    ```csharp
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
    
    这是指定由服务提供操作的接口。

5.  在 _Services_ 文件夹中添加 _UserService.cs_ 类，并使用以下内容进行替换：

    ```csharp
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

    这个类是`IUserService`接口的一个实现。

6. 在应用程序的根目录中增加 _ViewComponents_ 文件夹，然后将新目录增加 _UsersViewComponent.cs_ 类，并使用以下内容替换原有内容 ：
     ```csharp
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

     这是视图组件的代码部分。它利用内置的依赖注入来解析`IUserService`接口的实现，并注入到视图组件类的构造函数中的。`InvokeAsync`方法将从服务中获取`List<User>`并将其传递给视图。

7. 在 _Pages_ 文件夹中创建 _Components_ 文件夹，然后将 _Users_ 文件夹添加到新创建的 _Components_ 文件夹中。在 _Users_ 文件夹添加 _default.cshtml_ 文件。生成的文件夹和文件层次结构应如下所示：

     ![ViewComponents view file](/images/2017-08-06_20-06-21.png)

8. 将 _default.cshtml_ 中的代码替换为以下内容：

     ```html
     @model List<RazorPages.User>
     <h3>Users</h3>
     <ul>
         @foreach (var user in Model)
         {
             <li>@user.Name</li>
         }
     </ul>
     ```

     这是视图，并完成视图组件。请注意，视图通过`@model`指令将接受的模型类型指定为`List<User>`，它表示从`InvokeAsync`方法传递给视图的类型。

8. 打开 _Startup.cs_ 文件并添加 `using RazorPages.Services;` 命名空间引用，然后修改`ConfigureServices`方法代码为如下所示：

     ```csharp
     public void ConfigureServices(IServiceCollection services)
     {
         services.AddMvc();
         services.AddTransient<IUserService, UserService>();
     }
     ```

     这一步注册`IUserService`到依赖注入容器中，并指定`UserService`为使用的实现。

9. 打开 _\_Layout.cshtml_ 文件，找到`<nav>`部分和`<environment>`标签助手之间的内容，当前内容为如下：

   ```html
   <div class="container body-content">

       @RenderBody()
       <hr />
       <footer>
           <p>&copy; 2017 - RazorPages</p>
       </footer>
   </div>
   ```

   将内容修改为如下：

   ```html
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

   这会将网站的布局转换为两列，其中页面内容显示在左侧栏中，用户部件显示在右侧栏中。它使用`Component.InvokeAsync`方法将视图组件的输出呈现给页面。传递给该方法的字符串表示视图组件的名称（去掉“ViewComponent”后缀后的类名）。

11.  运行该网站以确保一切正常。用户列表应显示在每个页面的右侧：

    ![User View Component](/images/07-08-2017-08-24-53.png)

## 标签助手和参数传递


第二个例子将演示如何使用标签助手而不是调用`Component.InvokeAsync`方法，同时将演示传递参数给视图组件。

1. 在 _ViewComponents_ 文件夹中增加 `UserViewComponent.cs` 文件，将内容替换为如下内容：

    ```csharp
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

    这是视图组件的代码部分。这个组件和前一个组件的唯一区别是该`InvokeAsync`方法需要传递一个`int`类型的参数，然后传递给服务方法。

2. 在 _/Pages/Components_ 文件夹中添加`User`文件夹。将 Razor 文件 _default.cshtml_ 添加到该文件夹。 _Components_ 文件夹现在的结构应该如下所示：

    ![user view component](/images/07-08-2017-08-50-21.png)


3. 将新的 _default.cshtml_ 文件中内容替换为如下内容：

    ```html
    @model RazorPages.User

    <h3>Featured User</h3>
    <div>
        <strong>@Model.Name</strong><br />
        @Model.Website<br />
        @Model.Company.Name<br />
    </div>
    ```

4. 打开 _ViewImports.cshml_ 文件，并在现有代码中添加下行代码： 

    ```html
    @addTagHelper *, RazorPages
    ```

    这将注册视图组件标记。

5. 用以下代码替换`@await Component.InvokeAync("Users")`布局文件中的调用代码：

    ```html
    <vc:user id="new Random().Next(1, 10)"></vc:user>
    ```
    视图组件的名称是在标签助手中指定的，以及`InvokeAsync`方法的参数。在这种情况下，每次调用组件时都会使用`Random`类生成从1-10随机数字，导致每次显示页面时都会随机选择一个用户。

6. 运行应用程序以测试组件是否正常工作，并刷新几次以查看显示的不同用户的详细信息：

     ![user view component](/images/07-08-2017-09-07-45.png)

     如果您喜欢使用`Component.InvokeAsync`方法，则`id`参数将作为第二个参数传递：
     
     ```html
     @await Component.InvokeAsync("User", new Random().Next(1, 10))
     ```