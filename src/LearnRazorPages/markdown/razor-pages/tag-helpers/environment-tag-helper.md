# Environment 标签助手


Environment 标签助手根据应用程序的 [ASPNETCORE_ENVIRONMENT环境变量](/miscellaneous/environments) 的当前值来呈现不同的内容。


| 属性 | 描述 |
| --- | --- |
| `names` | 呈现内容环境的名称 |
| `include` |  呈现内容包含的环境名称列表 |
| `exclude` | 不应呈现内容包含的环境名称列表 |

## 备注

Environment 标签助手最常见的用法是在应用程序处于开发阶段时包含完整版本的CSS、JavaScript文件，在应用程序处于其它环境时压缩与打包的CSS、JavaScript文件。
```html
<environment names="Development">            
    <link rel="stylesheet" href="~/css/style1.css" />
    <link rel="stylesheet" href="~/css/style2.css" />
</environment>
<environment names="Staging, Test, Production">
    <link rel="stylesheet" href="~/css/style.min.css" />
</environment>
```

如果环境名称是`exclude`列表的一部分，则在该环境的任何情况下都不会呈现内容。`exclude`列表会覆盖`include`列表、 `names`列表。