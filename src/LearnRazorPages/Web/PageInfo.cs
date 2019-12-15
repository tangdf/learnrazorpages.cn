using System.Collections.Generic;

namespace LearnRazorPages.Web
{
    public class PageInfo
    {
        public string Title { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }

        public bool Menu { get; set; } = true;

        public List<PageInfo> Childs { get; set; }


        public static PageInfo NotFoundPageInfo = new PageInfo {
            Url = "notfound",
            Title = "That page doesn't exist",
            Description = "",
            Name = string.Empty
        };

    }
}
