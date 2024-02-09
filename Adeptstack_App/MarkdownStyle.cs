namespace Adeptstack_App
{
    class MarkdownStyle
    {
        static string css_dark = "body { background-color: #303030; color: white; font-family: 'Cascadia Mono', serif;} " +
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
