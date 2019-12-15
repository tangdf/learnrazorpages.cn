# Configuration In Razor Pages

ASP.NET Core includes an API for managing configuration settings needed by the application which includes a number of providers for retrieving data in a variety of different formats.

Configuration is set up as part of the `WebHost.CreateDefaultBuilder` method called in _Program.cs_, the entry point to the application. Various key/value stores are added to configuration by default:

*   _appsettings.json_ (and another version named after the current environment e.g. _appsettings.Development.json_)
*   User Secrets (if the environment is _Development_)
*   Environment variables
*   Command line arguments

You can add other stores such as XML files, _.ini_ files and so on if required. Configuration is added to the Dependency Injection system and is accessible throughout the application via an `IConfiguration` object.

## AppSettings.json

The vast majority of applications are likely to only ever use an _appsettings.json_ file for their configuration needs. Each configuration setting is stored in its own section. The default _appsettings.json_ file includes a section that configures logging for the application:

```
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  }
}

```

The next example configures logging and a connection string to a SQLite database:

```
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

## Working with Custom Settings

Custom settings provide the easiest way to extend the application configuration. The use of JSON as a storage format enables you to store complex information easily.

You can provide any name you like to custom sections of the _appsettings.json_ file. In the example below, some values are stored in a section which has been creatively named _AppSettings_:

```
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

## Accessing Configuration Settings Programmatically

The `IConfiguration` object enables you to access configuration settings in a variety of ways once it has been injected into your PageModel's constructor. You need to add a `using` directive for `Microsoft.Extensions.Configuration` to the PageModel class file. The first example illustrates how to reference a value using a string-based approach. The section is specified and subsequent properties are referenced by separating them with colons (`:`)

```

private readonly IConfiguration _configuration;

public IndexModel(IConfiguration configuration){
    _configuration = configuration;
}

public void OnGet()
{
    ViewData["config"] = _configuration["AppSettings:First"];
}

```

This approach, as with all APIs that rely on strings is error-prone. You are a typing mistake away from a `NullReferenceException` at runtime.

## Strongly Typed AppSettings

A more robust approach can be achieved by using the Configuration system's built-in capability to bind settings to a C# object. The following code is a C# representation of the object represented in the JSON above:

```
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

And this is how you can use the `IConfiguration.GetSection` method to bind the content of _appsettings.json_ to and instance of `AppSettings`:

```
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