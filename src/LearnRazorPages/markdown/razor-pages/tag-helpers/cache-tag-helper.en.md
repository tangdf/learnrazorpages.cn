# The Cache Tag Helper

The cache tag helper enables you to cache regions of a Razor page in memory on the server. Typical uses for this helper are for View Components or partial views that require relatively expensive database or web service calls, but where the content doesn't change very often. This type of caching its primarily used to improve a website's performance.

Unlike most other tag helpers, the Cache tag helper doesn't target a standard HTML element. It targets the `<cache>` tag, which is not rendered to the browser and doesn't appear in the HTML source either.

<div class="alert alert-warning">

The cache tag helper uses an in-memory-based store for content. This is volatile and can be cleared unexpectedly for any number of reasons, including app pool recycling, server restarts, low memory pressure and so on. The cache tag helper is not intended to be used for reliable long term storage.

</div>

| Attribute | Description |
| --- | --- |
| `enabled`<sup>1</sup> | Used to specify if this tag is enabled or not |
| `expires-after`<sup>2</sup> | Specifies the period of time after which the cached item should expire |
| `expires-on`<sup>3</sup> | Specifies the time at which the cached entry should expire |
| `expires-sliding`<sup>4</sup> | The period of time after the last access that the item should expire |
| `priority`<sup>5</sup> | Specifies the `CacheItemPriority` value of the cached item |
| `vary-by`<sup>6</sup> | Used to specify the parameters that determine whether different versions of the same content should be cached |

## Notes

1.  The cache tag helper is enabled by default. You might want to conditionally disable it under certain circumstances. The `enabled` attribute facilitates this. The cached item in this example is disabled on Sundays:

    ```
    <cache enabled="DateTime.Now.DayOfWeek != DayOfWeek.Sunday">@DateTime.Now</cache>

    ```

2.  If you don't provide values for any of the `expires-*` attributes, your item will be cached without an expiration date, meaning that it will only expire when the memory store is cleared. The `expires-after` attribute takes a `TimeSpan` value that represents the period of time that the item should be stored in the cache. This example shows the item being stored for 1 hour:

    ```
    <cache expires-after="TimeSpan.FromHours(1)">@DateTimeNow</cache>

    ```

3.  The `expires-on` attribute takes a `DateTimeOffset` value that specifies the absolute expiry time of an item. This item is set to expire at 8:15 am on June 14th 2017 UTC:

    ```
    <cache expires-on="new DateTimeOffset(new DateTime(2017, 6, 14, 8, 15, 0))">@DateTime.Now</cache>

    ```

4.  An item can be set to expire after a specified period of inactivity. This is known as "sliding expiration". The `expires-sliding` attribute takes a `TimeSpan` value. In this example, the item is set to expire 20 minutes after the last time the page was requested:

    ```
    <cache expires-sliding="TimeSpan.FromMinutes(20)">@DateTime.Now</cache>

    ```

    As each request is made for the cached item, the expiration time is reset to 20 minutes after the request.

5.  Items in the cache can have a priority applied to them, determining how they are dealt with when memory pressure results in items being cleared from the cache before their pre-ordained expiry time. The `priority` attribute takes a `CacheItemPriority` enum value that specifies the relative priority of items in the cache. The available values are

    *   High
    *   Low
    *   Normal
    *   NeverRemove

    Items with `High` priority will be the last to be removed. Items set as `NeverRemove` will remain in the cache. You should exercise caution when applying `NeverRemove` as there is a danger of overflowing the cache with more data than it can handle. This is how to set an item with `High` priority:

    ```
    <cache priority="CacheItemPriority.High">@DateTime.Now</cache>

    ```

6.  You can cache multiple versions of the same content based on different criteria. The `vary-by` attribute provides a mechanism for you to specify the criteria to be used to determine whether to store another version of existing cached content.

    The attribute has a number of preset options:

    *   `vary-by-cookie`
    *   `vary-by-header`
    *   `vary-by-query`
    *   `vary-by-route`
    *   `vary-by-user`

    #### Cookie

    The values of cookies can be taken into consideration when caching different versions of content. The `vary-by-cookie` option takes a comma-separated string representing the names of the cookies:

    ```
    <cache vary-by-cookie="AppCookie1, AppCookie2">@DateTime.Now</cache>

    ```

    #### Header

    You can store multiple versions of content based on differing request header values. Multiple headers can be applied as a comma-separated string. This example stores different versions of the cached content based on the user's preferred language:

    ```
    <cache vary-by-header="Accept-Language">@DateTime.Now</cache>

    ```

    #### Query

    The `vary-by-query` option enables the use of query string parameters as the criteria for caching different versions. Query string parameter names are supplied in a comma-separated list:

    ```
    <cache vary-by-query="id">@DateTime.Now</cache>

    ```

    #### Route

    You can have route data parameters taken into account by using the `vary-by-route` option. This attribute also accepts a comma-separated string of route parameter names as a value:

    ```
    <cache vary-by-route="key1, key2">@DateTime.Now</cache>

    ```

    #### User

    The final built-in option enables you to specify that each user gets their own version of cached content. The `vary-by-user` attribute requires a `bool`, which should be set to `true`:

    ```
    <cache vary-by-user="true">@DateTime.Now</cache>

    ```

    #### String

    Finally, you can provide an arbitrary string value as a parameter if you want to use a value that isn't exposed by one of the preset options:

    ```
    <cache vary-by="@Model.Id">@DateTime.Now</cache>

    ```

    You can use any combination of the `vary-by` attributes that you need.