namespace LearnRazorPages.Web
{
    public class HeadingLink
    {
        public HeadingLink(string id, string text)
        {
            Id = id;
            Text = text;
        }

        public string Id { get;  }

        public string Text { get;  }
    }
}
