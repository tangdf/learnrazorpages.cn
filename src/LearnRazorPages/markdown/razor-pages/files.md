# 不同作用的 Razor 文件

所有 Razor 文件以 _.cshtml_ 结尾。 大部分Razor文件都是可浏览的，并且包含客户端和服务器端代码的混合，这些代码在处理时会导致HTML被发送到浏览器,这些页面通常被称为“[内容页面](/razor-pages#内容页面)”。其它 Razor 文件的名称中以下划线（`_`）开头， 这些文件不能被浏览。 下划线开头的通常被称为[局部页面](/razor-pages/partial-pages)，以这种方式命名的三个文件在Razor页面应用程序中具有特定的功能。

### _Layout.cshtml

[_\_Layout.cshtml_](/razor-pages/files/layout) 文件作为其它内容页面的模板。网站设计的公共部分可以在_\_Layout.cshtml_文件中声明， 可以包括页眉，页脚，网站导航等。 通常，_\_Layout.cshtml_文件还包含页面的`<head>`部分，因此它们还引用常见的 CSS 样式和 JavaScript 文件，包括网站分析服务文件。 如果您想对网站的整体设计进行更改，通常只需对 _\_Layout.cshtml_ 文件的内容进行调整即可。

### _ViewStart.cshtml

 _\_ViewStart.cshtml_ 文件是在当前文件夹或子文件夹中的内容页面代码之前执行的代码，提供了一个便捷的方式来指定受其影响的所有内容页面的布局文件，您通常看到的 _\_ViewStart.cshtml_ 文件内容，都来自Razor 页面（或MVC）项目默认模板中。

### _ViewImports.cshtml

[_\_ViewImports.cshtml_](/razor-pages/files/viewimports)文件的目的是提供一种机制，将指令符应用到全局的 Razor 页面，不必单独添加到每一个页面中。

默认的 Razor 页面项目模板在 _Pages_（Razor页面的根文件夹）文件夹中包含一个 _\_ViewImports.cshtml_ 文件。 在文件夹层次结构中的 _\_ViewImports.cshtml_ 文件中设置的指令会影响所有 Razor 页面。