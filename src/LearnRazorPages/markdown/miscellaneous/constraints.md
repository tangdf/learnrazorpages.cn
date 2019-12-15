# 路由约束

下表列出了路由约束中数据类型和范围配置：

| 约束 | 描述 | 示例 |
| --- | --- | --- |
| `alpha` | 匹配大、小写拉丁字母字符 (a-z, A-Z) |	`{title:alpha}`|
|`bool`|	匹配一个布尔值。|	`{isActive:bool}`|
|`int`	|匹配一个32位整数值。|	`{id:int}`|
|`datetime`|匹配日期时间值。|	`{startdate:datetime}`|
|`decimal`	|匹配一个十进制值。|	`{cost:decimal}`|
|`double`|	匹配一个64位浮点值。|	`{latitude:double}`|
|`float`|	匹配一个32位的浮点值。|	`{x:float}`|
|`long`	|	匹配一个64位的整数值。|	`{x:long}`|
|`guid`	|	匹配一个GUID值。|	`{id:guid}`|
|`length`|	匹配指定长度的字符串或指定的长度范围内的字符串。|	`{key:length(8)} {postcode:length(6,8)}`|
|`min`	|	匹配一个整数的最小值。	|`{age:min(18)}`|
|`max`	|	匹配一个整数的最大值。	|`{height:max(10)}`|
|`minlength`|匹配一个最小长度的字符串。|`{title:minlength(2)}`|
|`maxlength`	|匹配一个最大长度的字符串。|	`{postcode:maxlength(8)}`|
|`range`|	匹配值范围内的整数。	|`{month:range(1,12)}`|
|`regex`	|	匹配一个正则表达式。	|`{postcode:regex(^[A-Z]{2}\d\s?\d[A-Z]{2}$)}`|





