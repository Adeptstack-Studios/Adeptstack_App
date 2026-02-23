namespace Adeptstack_App.Utils
{
    class MarkdownStyle
    {
        static string css_dark = "html {display: flex; justify-content: center; padding: 20px;} " +
                              "body { background-color: #000000; color: white; max-width: 800px; font-family: 'Nunito', serif; } " +
                              "blockquote { Background: #222222; padding: 5px 20px; border-radius: 10px; }" +
                              "img { border-radius: 10px; max-width: 100%; }" +
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
            if (string.IsNullOrEmpty(css))
            {
                css = css_dark;
            }

            return "<html><head><style>" + css + "</style></head><body>" + content + "</body></html>";
        }

        public static string CSS()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                string resourcePath = "Adeptstack_App.Resources.Raw.Nunito.ttf";

                using (var stream = assembly.GetManifestResourceStream(resourcePath))
                {
                    if (stream == null)
                    {
                        System.Diagnostics.Debug.WriteLine("FEHLER: Ressource nicht gefunden!");
                        return "";
                    }

                    using (var reader = new MemoryStream())
                    {
                        stream.CopyTo(reader);
                        byte[] fontBytes = reader.ToArray();
                        string base64Font = Convert.ToBase64String(fontBytes);

                        string css_dark = "@font-face { font-family: 'Nunito'; src: url('data:font/ttf;base64," + base64Font + "') format('truetype'); }" +
                              "html {min-height: 100%; margin: 0; padding: 0; } " +
                              "body { background-color: #000000; color: white; max-width: 800px; font-family: 'Nunito', serif; min-height: 100%; margin: 0 auto; padding: 10px 10px 100px 10px; box-sizing: border-box;  } " +
                              "blockquote { Background: #222222; padding: 5px 20px; border-radius: 10px; }" +
                              "img { border-radius: 10px; max-width: 100%; }" +
                              "a { text-decoration: none; color: #00a8ff; }" +
                              "p { word-wrap: break-word; }" +
                              "pre { background: #262626; padding: 5px 10px; border-radius: 10px; overflow-x: auto; }  div { border-radius: 11px; }" +
                              "code { background: #262626; padding: 2px 5px; border-radius: 5px; }";

                        return css_dark;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fehler: {ex.Message}");
            }
            return "";
        }

        public static string CSSLight()
        {
            return css_light;
        }
    }
}
