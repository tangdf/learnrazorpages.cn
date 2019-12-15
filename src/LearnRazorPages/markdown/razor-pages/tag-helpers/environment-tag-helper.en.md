

# The Environment Tag Helper

The environment tag helper supports rendering different content dependent on the current value of the [ASPNETCORE_ENVIRONMENT environment variable](/miscellaneous/environments) for the application.

| Attribute | Description |
| --- | --- |
| `names` | The name(s) of the environment(s) for which the content should be rendered |
| `include` | The name(s) of the environment(s) for which the content should be rendered |
| `exclude` | The name(s) of the environment(s) for which the content should not be rendered |

## Notes

The most common use case for the environment tag helper is to include the full version of CSS or JavaScript files when the application is in a development stage, and the bundled and minified versions when the application is in other environments.

```
<environment names="Development">            
    <link rel="stylesheet" href="~/css/style1.css" />
    <link rel="stylesheet" href="~/css/style2.css" />
</environment>
<environment names="Staging, Test, Production">
    <link rel="stylesheet" href="~/css/style.min.css" />
</environment>

```

If an environment name is part of an `exclude` list, the content will not be rendered under any circumstances for that environment. Inclusion in the `exclude` list overrides inclusion in the `include` list or the `names` list.