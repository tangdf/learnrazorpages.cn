# Razor 页面配置

ASP.NET Core 提供了用于管理应用程序所需配置的 API 设置，其中包含许多用于解析各种不同格式数据的提供程序。

在`Program.cs`中通过调用`WebHost.CreateDefaultBuilder`方法来设置配置，这是应用程序的入口点。默认情况下，将下列各种键/值存储添加到配置中:

*   _appsettings.json_ （另外使用当前环境命名的版本，例如 _appsettings.Development.json_）
*   用户秘密（如果是开发环境）
*   环境变量
*   命令行参数

如果需要，可以添加其它存储方式，如：XML文件、 _.ini_ 文件等。配置被添加到依赖注入系统，在整个应用程序中可以通过 _IConfiguration_ 对象访问。

## AppSettings.json

绝大多数应用程序可能只会使用 _appsettings.json_ 文件来满足配置需求。每个配置设置都存储在其对应的节点。 _appsettings.json_ 文件默认情况下包含配置应用程序日志记录的部分：

```json
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  }
}

```

下面的示例将日志记录和链接字符串配置为SQLite数据库：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "DataSource=app.db"
  },
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```

## 使用自定义设置

自定义设置提供了扩展应用程序配置的最简单方法。使用 JSON 作为存储格式可以让您轻松存储复杂的信息。

您可以提供任何您喜欢的名字来定制 _appsettings.json_ 文件的节点。在下面的例子中，一些值被存储在自定义的 _AppSettings_ 的节点中：

```json
"AppSettings":{
  "First" : "Value 1",
  "Second" : "Value 2",
  "Car": {
      "NumberOfDoors" : 5,
      "RegistrationDate" : "2017-01-01T00:00:00.000Z",
      "Color" : "Black"
    }
}
```

## 访问配置设置

一旦将`IConfiguration`对象注入到 PageModel 的构造函数中，该对象将允许您以多种方式访问配置设置。您需要为 PageModel 类文件添加`using`指令`Microsoft.Extensions.Configuration`。第一个例子展示了如何使用基于字符串的方法来检索一个值。指定的部分使用冒号（`:`）分隔后续的属性。

```csharp
private readonly IConfiguration _configuration;

public IndexModel(IConfiguration configuration){
    _configuration = configuration;
}

public void OnGet()
{
    ViewData["config"] = _configuration["AppSettings:First"];
}
```

与所有依赖字符串的API一样，这种方法很容易出错。运行时可能因为拼写错误会导致`NullReferenceException`错误。

## 强类型AppSettings

通过使用配置系统的内置功能将设置绑定到 C# 对象，可以实现更健壮的方式。以下代码是上面JSON中表示对象 C# 表示形式：

```csharp
public class AppSettings
{
    public string First { get; set; }
    public string Second { get; set; }
    public Car Car { get; set; }
}

public class Car
{
    public int NumberOfDoors { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string Color { get; set; }
}

```

下面是如何使用`IConfiguration.GetSection`方法将 _appsettings.json_ 的内容绑定到`AppSettings`实例的方法：

```csharp
private readonly IConfiguration _configuration;

public IndexModel(IConfiguration configuration){
    _configuration = configuration;
}

public void OnGet()
{
    var settings = _configuration.GetSection("AppSettings").Get<AppSettings>();
    ViewData["RegistrationDate"] = settings.Car.RegistrationDate;
}
```