# Different types of Razor files

All Razor files end with _.cshtml_. Most Razor files are intended to be browsable and contain a mixture of client-side and server-side code, which, when processed, results in HTML being sent to the browser. These pages are usually referred to as "[content pages](/razor-pages#content-pages)". Other Razor files have a leading underscore (`_`) in their file name. These files are not intended to be browsable. The leading underscore is often used for naming [partial pages](/razor-pages/partial-pages), but three files named in this way have a particular function within a Razor Pages application.

### _Layout.cshtml

The [__Layout.cshtml_](/razor-pages/files/layout) file acts a template for all content pages that reference it. Consistent part of a site's design are declared in the __Layout.cshtml_ file. These can include the header, footer, site navigation and so on. Typically, the __Layout.cshtml_ file also includes the `<head>` section of the page, so they also reference the common CSS style sheet files and JavaScript files including analytics srvice's files. If you want to make changes to the overall design of the site, you often only need to make adjustments to the content of the __Layout.cshtml_ file.

### _ViewStart.cshtml

The __ViewStart.cshtml_ file contains code that executes after the code in any content page in the same folder or any child folders. It provides a convenient location to specify the layout file for all content pages that are affected by it, and that is typically what you see in the __ViewStart.cshtml_ file that comes with any Razor Pages (or MVC) template.

### _ViewImports.cshtml

The purpose of the [__ViewImports.cshtml_](/razor-pages/files/viewimports) file is to provide a mechanism to make directives available to Razor pages globally so that you don't have to add them to pages individually.

The default Razor Pages template includes a __ViewImports.cshtml_ file in the _Pages_ folder - the root folder for Razor pages. All Razor pages in the folder heirarchy will be affected by the directives set in the __ViewImports.cshtml_ file.