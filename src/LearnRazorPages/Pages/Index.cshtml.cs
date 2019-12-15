using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearnRazorPages.Web;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Helpers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;


namespace LearnRazorPages.Pages
{
    [Cahce.ResponseCache(Duration = 60 * 6)]
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly List<PageInfo> _pageInfos;

        public IndexModel(IWebHostEnvironment webHostEnvironment, IOptions<List<PageInfo>> configuration)
        {
            this._webHostEnvironment = webHostEnvironment;
            this._pageInfos = configuration.Value;
        }

        public string HtmlContent { get; private set; }

        public List<HeadingLink> HeadingLinks { get; private set; }

        public PageInfo CurrentPageInfo { get; private set; }

        public DateTime? LastUpdatedtime { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
   

            InitPageInfo();

            if (this.CurrentPageInfo == null)
                return NotFound();

            var filePath = GetMdFilePath();

            if (System.IO.File.Exists(filePath) == false) {
                return NotFound();
            }

            this.LastUpdatedtime = System.IO.File.GetLastWriteTime(filePath);

            await TransformMarkdown(filePath);

            return Page();
        }

        private string GetMdFilePath()
        {
            var url = this.CurrentPageInfo.Url;
            if (url.StartsWith('/'))
                url = url.Substring(1);
            if (url.EndsWith('/'))
                url = url.Substring(0, url.Length - 1);
            if (string.IsNullOrEmpty(url))
                url = "home";

            var rootPath = this._webHostEnvironment.ContentRootPath;

            var path = Path.Combine(rootPath, "markdown", url + ".md");

            return System.IO.Path.GetFullPath(path);
         
        }

        private void InitPageInfo()
        {
         
            if (string.Equals(this.RouteData.Values["errorcode"] as string, "404", StringComparison.OrdinalIgnoreCase)) {
                this.CurrentPageInfo = PageInfo.NotFoundPageInfo;
                return;
            }
            var path = this.Request.Path.Value;

            if (path.Length>1 && path.EndsWith('/'))
                path = path.Substring(0, path.Length - 1);

            foreach (PageInfo pageInfo in this._pageInfos) {
                if (string.Equals(pageInfo.Url, path, StringComparison.OrdinalIgnoreCase)) {
                    this.CurrentPageInfo = pageInfo;
                    break;
                }

                if (pageInfo.Childs != null && pageInfo.Childs.Count > 0) {
                    foreach (PageInfo pageInfoChild in pageInfo.Childs) {
                        if (string.Equals(pageInfoChild.Url, path, StringComparison.OrdinalIgnoreCase)) {
                            this.CurrentPageInfo = pageInfoChild;
                            break;
                        }
                    }

                    if (this.CurrentPageInfo != null)
                        break;
                }
            }

            if (this.CurrentPageInfo == null
                && string.Equals(this.RouteData.Values["errorcode"] as string, "404", StringComparison.OrdinalIgnoreCase)) {
                this.CurrentPageInfo = PageInfo.NotFoundPageInfo;
            }
        }

        private async Task TransformMarkdown(string filePath)
        {
            string text;

            using (StreamReader streamReader = new StreamReader(filePath)) {
                text = await streamReader.ReadToEndAsync();
            }


            MarkdownPipelineBuilder markdownPipelineBuilder = new MarkdownPipelineBuilder().UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
                .UseAdvancedExtensions().UseBootstrap();

            var pipeline = markdownPipelineBuilder.Build();

            using (StringWriter stringWriter = new StringWriter()) {
                MarkdownDocument markdownDocument = Markdown.ToHtml(text, stringWriter, pipeline);

                HeadingLinks = new List<HeadingLink>();

                StringBuilder stringBuilder = new StringBuilder();

                foreach (HeadingBlock headingBlock in markdownDocument.Descendants<HeadingBlock>()) {
                    var htmlAttributes = headingBlock.TryGetAttributes();
                    if (htmlAttributes != null) {
                        if (stringBuilder.Length > 0)
                            stringBuilder.Length = 0;
                        GetContainerInlineText(headingBlock.Inline);
                        HeadingLinks.Add(new HeadingLink(htmlAttributes.Id, stringBuilder.ToString()));
                    }
                }

                void GetContainerInlineText(ContainerInline containerInline)
                {
                    foreach (Inline inline in containerInline) {
                        if (inline is ContainerInline childInline) {
                            GetContainerInlineText(childInline);
                        }
                        else if (inline is LiteralInline stringLine) {
                            stringBuilder.Append(stringLine.ToString());
                        }
                        else {
                            throw new NotSupportedException();
                        }
                    }
                }

                this.HtmlContent = stringWriter.ToString();
            }
        }
    }
}