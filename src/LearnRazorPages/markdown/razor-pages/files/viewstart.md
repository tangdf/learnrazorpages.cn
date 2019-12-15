# ViewStart 文件

Razor 页面 _\_ViewStart.cshtml_ 文件包含在每个 Razor 页面执行开始时执行的代码，会影响位于当前文件夹及子文件夹的所有 Razor 页面；按文件系统结构层次执行，位于子文件夹中的文件会在父文件夹中的文件之后执行。

ViewStart文件最常见的用途是为每个 Razor 页面设置布局页面。由于ViewStart文件也是 Razor 页面，因此服务器端代码必须位于 Razor 代码块中：

```csharp
@{
    Layout = "_Layout";
}
```