
# Cache 标签助手


Cache 标签助手能够将 Razor 页面的部分区域缓存服务器上内存中的，典型用途是针对视图组件或部分视图，这些视图往往需要对昂贵数据库或 Web 服务进行调用，但内容不会经常更改。这种类型的缓存主要用于提高网站的性能。

与大多数其它标签助手不同，Cache标签助手的目标不是以标准的 HTML 元素，它的目标是`<cache>`标签，不会呈现给浏览器，也不会出现在HTML源代码中。

<div class="alert alert-warning">

Cache 标签助手使用基于内存的存储方式，这是不稳定的，可以由于很多原因意外清除，包括应用程序池回收、服务器重启、内存压力等等。Cache 标签助手不适用于持久化存储。

</div>

| 属性 | 描述 |
| --- | --- |
| `enabled`<sup>1</sup> | 指定是否启用此标记 |
| `expires-after`<sup>2</sup> | 指定缓存项过期的时间段 |
| `expires-on`<sup>3</sup> | 指定缓存的项过期的时间点 |
| `expires-sliding`<sup>4</sup> | 最后一次访问后过期的时间段 |
| `priority`<sup>5</sup> | 指定缓存项的`CacheItemPriority`值 |
| `vary-by`<sup>6</sup> | 指定是否缓存不同版本相同内容的参数 |


## 备注

1.	Cache 标签助手是默认启用的。在某些情况下，您可能需要禁用它，`enabled`属性可以轻松实现。下面这个例子中的缓存项将在周日禁用：

    ```html
    <cache enabled="DateTime.Now.DayOfWeek != DayOfWeek.Sunday">@DateTime.Now</cache>
    ```

2.	如果您没有提供任何`expires-*`属性的值，被缓存的项目没有过期时间，这意味着它只会在内存回收时才会过期。`expires-after`属性采用`TimeSpan`值，表示项目应该存储在缓存中的时间段。此示例演示项目将被缓存1个小时：

    ```html
    <cache expires-after="TimeSpan.FromHours(1)">@DateTimeNow</cache>
    ```

3.	`expires-on`属性采用`DateTimeOffset`值，指定项目的绝对过期时间。下面例子中的项目将于2017年6月14日上午8:15过期：

    ```html
    <cache expires-on="new DateTimeOffset(new DateTime(2017, 6, 14, 8, 15, 0))">@DateTime.Now</cache>
    ```


4.	可以将项目设置为在不使用的时段后过期，称为“滑动过期”。 `expires-sliding`属性需要一个`TimeSpan`值。 在此示例中，项目设置为在上次页面请求20分钟后过期：

    ```html
    <cache expires-sliding="TimeSpan.FromMinutes(20)">@DateTime.Now</cache>
    ```

	由于每个请求都会涉及到缓存项，所以在请求后过期时间重置为20分钟。

5.	我们可以给缓存中的项目设置一个优先级，设定当内存压力导致在预定过期时间之前从缓存中清除的项时如何处理它们。`priority`属性采用`CacheItemPriority`枚举值来指定缓存中项目的相对优先级。可用的值有：
    *   High
    *   Low
    *   Normal
    *   NeverRemove


	`High`项目将最后被清除。设置为`NeverRemove`优先级的项目将保留在缓存中。在应用`NeverRemove`时应该非常小心，因为超过内存处理能力时，存在数据溢出的危险。下面这是如何设置优先级别为`High`的例子：	

    ```html
    <cache priority="CacheItemPriority.High">@DateTime.Now</cache>
    ```

6.	可以根据不同的标准缓存同一内容的多个版本。`vary-by`属性为提供了一种机制，用于指定是否缓存现有内容的其它版本。  

	该属性有多个预设选项：
    *   `vary-by-cookie`
    *   `vary-by-header`
    *   `vary-by-query`
    *   `vary-by-route`
    *   `vary-by-user`

    #### Cookie

	根据Ccookie的值缓存多个同版本的内容。`vary-by-cookie`选项使用逗号分隔的字符串表示Cookie的名称：

    ```html
    <cache vary-by-cookie="AppCookie1, AppCookie2">@DateTime.Now</cache>    
    ```
    
    #### Header

	根据不同的请求头的值缓存多个版本的内容。多个标题可以使用逗号分隔的字符串表示。此示例根据用户的首选语言存储不同版本的缓存内容：

    ```html
    <cache vary-by-header="Accept-Language">@DateTime.Now</cache>    
    ```
    
    #### Query

	`vary-by-query`选项允使用查询字符串参数作为缓存不同版本的依据。查询字符串参数名称以逗号分隔：

    ```html
    <cache vary-by-query="id">@DateTime.Now</cache>    
    ```
    
    #### 路由
    
    使用`vary-by-route`选项依据路由参数。属性接受逗号分隔的路由参数名称作为值：

    ```html
    <cache vary-by-route="key1,key2">@DateTime.Now</cache>    
    ```
    
    #### 用户

	最后一个内置选项根据每个用户获取他们自己版本的缓存内容。`vary-by-user`属性需要一个`bool`值，它的设置为如下：

    ```html
    <cache vary-by-user="true">@DateTime.Now</cache>    
    ```
    
    #### String

	最后，如果您想要使用一个未被预先设置的选项，则可以提供任意字符串值作为参数:
	
    ```html
    <cache vary-by="@Model.Id">@DateTime.Now</cache>    
    ```

	您可以使用`vary-by`属性的任何组合。