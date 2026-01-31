namespace Adeptstack_App.Utils
{
    class MarkdownStyle
    {
        static string css_dark = "html {display: flex; justify-content: center; padding: 20px;} " +
                              "body { background-color: #000000; color: white; max-width: 800px; font-family: 'Calibri', serif;} " +
                              "blockquote { Background: #222222; padding: 5px 20px; border-radius: 10px; }" +
                              "img { border-radius: 10px; }" +
                              "a { text-decoration: none; color: #00a8ff; }" +
                              "p { word-wrap: break-word; }" +
                              "pre { background: #262626; padding: 5px 10px; border-radius: 10px; }  div { border-radius: 11px; }" +
                              "code { background: #262626; padding: 2px 5px; border-radius: 5px; }";

        static string css_light = "body { background-color: white; color: black; font-family: 'Cascadia Mono', serif;} " +
                                   "blockquote { Background: lightgray; padding: 5px 20px; border-radius: 10px; }" +
                                   "img { border-radius: 10px; }" +
                                   "a { text-decoration: none; color: #00a8ff; }" +
                                   "p { word-wrap: break-word; }" +
                                   "pre { background: darkgray; padding: 5px 10px; border-radius: 10px; } div { border-radius: 11px; }" +
                                   "code { background: darkgray; padding: 2px 5px; border-radius: 5px; }";

        public static string GetFullHTML(string css, string content)
        {
            return "<html><head><style>" + css + "</style></head><body>" + content + "</body></html>";
        }

        public static string CSS()
        {
            return css_dark;
        }

        public static string CSSLight()
        {
            return css_light;
        }
    }
}
