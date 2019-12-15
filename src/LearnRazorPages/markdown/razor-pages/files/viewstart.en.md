# The ViewStart file

The Razor Pages __ViewStart.cshtml_ file contains code that is executed at the start of each Razor Page's execution. The ViewStart file affects all Razor Pages located in the same folder as the ViewStart file or any of its subfolders. ViewStart files are hierarchical. Those located in subfolders will be executed after those located higher up the file system hierarchy.

The most common use for the ViewStart file is to set the layout page for each Razor Page. Since the ViewStart file is a Razor Page, server-side code must be located in a Razor code block:

```
@{
    Layout = "_Layout";
}

```