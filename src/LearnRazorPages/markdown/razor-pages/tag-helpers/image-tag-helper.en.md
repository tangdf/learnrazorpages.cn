# The Image Tag Helper

The image tag helper targets the `<img>` element and enables versioning of image files.

| Attribute | Description |
| --- | --- |
| `append-version` | A boolean value indicating whether to append the image URL with a file version |

## Notes

The query string named `v` is added to the image's URL if the `append-version` attribute is set to `true`. The value is calculated from the file contents, so if the file is amended, the value will differ. Browsers take query string values into account when determining whether a request can be satisfied from its cache. Therefore, if the query string value changes, the browser will retrieve the new version of the file from the server.

This usage example utilises one of the images provided with the Razor Pages (and MVC) template:

```
<img asp-append-version="true" src="~/images/banner1.svg" />

```

The rendered HTML is as follows:

```
<img src="/images/banner1.svg?v=GaE_EmkeBf-yBbrJ26lpkGd4jkOSh1eVKJaNOw9I4uk" />

```

The thing about this image is that it is an _.svg_ file, which makes it easy to edit using any text editor. The following shows the new version tag after changing just the first path's `fill` color value from `#56B4D9` to `#66B4D9`:

```
<img src="/images/banner1.svg?v=qp53a_aCPkTojNdLo1S2IvprtETqDiat_cWYbj1z8Z0" />

```