# Razor 页面表单

表单用于将数据从浏览器传输到Web服务器进行进一步处理，例如将数据保存到数据库、构建电子邮件、或者简单地对数据进行某种算法处理，然后显示结果。

## HTML表单元素

HTML `<form>`元素用于在网页上创建表单。`form`元素具有多个属性，最常用的是`method`和`action`。`method`属性确定提交表单时使用的 HTTP 谓词。默认情况下，使用`GET`谓词，表单值作为查询字符串值附加到接收页面的URL。如果`action` 属性被省略，表单将被提交给当前的URL，即表单所在的页面。

通常，使用POST谓词提交表单，不在 URL中追回表单的值，并允许在请求中发送更多的数据，因为大多数浏览器都限制了查询字符串的长度。因此，必须提供`method`属性，其值为`post`：

```html
<form method="post">
...
</form>
```

## 收集用户输入

表单的主要作用是收集用户提供的输入，以便传输到Web服务器。表单控件集合，如`input`、`select`和`textarea`等元素，用于接受用户提交的输入内容。

`input`元素的显示和行为是由`type`属性控制。如果省略，则`type`默认为`text`，并且控件呈现为单行文本框：

<input>

有一系列其它的`input`类型，其行为和外观根据`type`值来显示：

| 类型               | 示例                                    | 描述                              |
| ---------------- | ------------------------------------- | ---------------------------------------- |
| `checkbox`       | <input type="checkbox">               | 呈现为复选框                  |
| `color`          | <input type="color">                  | 呈现一个颜色选择器                   |
| `date`           | <input type="date">                   | 呈现日期控件                   |
| `dateTime`       | <input type="datetime">               | 过时，使用`datetime-local`取而代之   |
| `datetime-local` | <input type="datetime-local">         | 创建一个控件，以浏览器的本地格式接受日期和时间 |
| `email`          | <input type="email">                  | 仅接受有效电子邮件地址的文本框，验证由浏览器执行 |
| `file`           | <input type="file">                   | 呈现文件选择器   |
| `hidden`         | <input type="hidden">                 | 没有呈现任何内容，用于传递不需要显示的表单值 |
| `image`          | <input type="image">                  | 使用指定的图像呈现提交按钮 |
| `month`          | <input type="month">                  | 渲染一个接受月和年的控件
 |
| `number`         | <input type="number">                 | 有些浏览器会渲染成一个`spinner`控件，并拒绝接受非数值。 |
| `password`       | <input type="password">               | 出于安全目的，用户输入的值被遮蔽 |
| `radio`          | <input type="radio">                  | 呈现为单选按钮                |
| `range`          | <input type="range" min="1" max="10"> | 呈现滑块控件        |
| `search`         | <input type="search">                 | 	设计为接受搜索字词的文本框，某些浏览器可能会提供其它功能，例如内容重置图标 |
| `submit`         | <input type="submit" value="提交查询内容">  | 呈现标准的提交按钮，文本内容是`提交` |
| `tel`            | <input type="tel">                    | 用于接受电话号码的文本框，浏览器不会验证任何特定的格式 |
| `time`           | <input type="time">                   | 以 hh：mm 格式接受时间值的控件 |
| `url`            | <input type="url">                    | 验证URL的文本输入框    |
| `week`           | <input type="week">                   | 接受星期和年的输入。 |


收集用户输入的另外两个最常用的元素是`textarea`，呈现多行文本框，以及`select`用于封装多个`option`元素的元素，为用户提供用于选择一个或多个固定选项列表的机制。

## 访问用户输入

如果表单控件提交了`name`属性的值，则用户输入的值会提交到服务端。有几种方法可以检索表单提交的值：

*   通过基于字符串的索引访问`Request.Form`集合，使用表单控件的`name`属性作为索引值。
*   利用 [模型绑定 ](/razor-pages/model-binding) 将表单字段映射到 [处理方法](/razor-pages/handler-methods) 参数。
*   利用模型绑定将表单字段映射到 [PageModel](/razor-pages/pagemodel) 的公共属性。

## Request.Form

<div class="alert alert-warning">

不建议使用这种方法，但是对于从其它框架（如PHP，传统 ASP 或 ASP.NET Web 页面）迁移过来的开发人员来说，`Request.Form`这是一种熟悉的方式。

</div>

通过基于字符串的索引可访问`Request.Form`集合中的项目，字符串的值映射到相关表单字段的`name`属性。下面的表单有一个`emailaddress`的输入框架：

```html
<form method="post">
    <input type="email" name="emailaddress"> 
    <input type="submit">
</form>
```

可以在`OnPost`处理方法中通过如下方式访问值：

```csharp
public void OnPost()
{
    var emailAddress = Request.Form["emailaddress"];
    // do something with emailAddress
}
```

字符串索引不区分大小写，但必须与输入的名称匹配。从`Request.Form`集合返回的值总是字符串。

## 利用模型绑定

推荐访问表单值的方法是使用模型绑定。模型绑定是将表单值自动映射到服务器端代码的过程，从`Request.Form`集合中的字符串转换为由服务器端表示的类型。目标可以是 PageModel 上的处理方法参数或公共属性。

###  处理方法参数

以下示例展示如何修改`OnPost`处理方法，将`emailAddress`输入值绑定到参数：

```csharp
public void OnPost(string emailAddress)
{
    // do something with emailAddress
}
```

### PageModel 公共属性

以下是如何修改处理方法代码以使用公共属性：

```csharp
[BindProperty]
public string EmailAddress { get; set; }

public void OnPost()
{
    // do something with EmailAddress
}
```

在模型绑定中包含的属性必须用`BindProperty`标记来修饰。

## 标签组手

`form`、`input`、`select`和`textarea`元素都存在对应的 [标签助手](/razor-pages/tag-helpers)，这些组件扩展 HTML 元素，提供用于控制 HTML 生成的自定义属性。