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

        private static readonly (string Name, string Key)[] ThemeVariables =
        {
            ("page", "PageBackground"),
            ("surface", "Surface"),
            ("surface-alt", "SurfaceAlt"),
            ("border", "Border"),
            ("stripe", "TableStripe"),
            ("text", "TextPrimary"),
            ("text-body", "TextBody"),
            ("text-secondary", "TextSecondary"),
            ("text-muted", "TextMuted"),
            ("accent", "Accent")
        };

        /// <summary>
        /// Farben des aktuellen Themes als CSS-Variablen für <see cref="CSS"/>. Auf dem UI-Thread aufrufen.
        /// </summary>
        public static string ThemeCSS()
        {
            string variables = string.Join(" ", ThemeVariables.Select(v => $"--{v.Name}: {AppTheming.Css(v.Key)};"));
            return $":root {{ color-scheme: {(AppTheming.IsDark ? "dark" : "light")}; {variables} }}";
        }

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
                                          "html { min-height: 100%; margin: 0 15px; padding: 0; } " +
                                          "body { background-color: var(--page); color: var(--text-body); max-width: 800px; font-family: 'Nunito', sans-serif; line-height: 1.6; min-height: 100%; margin: 0 auto; padding: 10px 10px 100px 10px; box-sizing: border-box; } " +
                                          "h1, h2, h3, h4, h5, h6 { color: var(--text); margin-top: 24px; margin-bottom: 12px; } " +
                                          "blockquote { background: var(--surface); padding: 10px 20px; border-radius: 8px; border-left: 4px solid var(--accent); margin: 16px 0; }" +
                                          "img { border-radius: 10px; max-width: 100%; margin: 16px 0; }" +
                                          "a { text-decoration: none; color: var(--accent); font-weight: bold; }" +
                                          "p { word-wrap: break-word; }" +
                                          "pre { background: var(--surface); padding: 12px 16px; border-radius: 10px; overflow-x: auto; border: 1px solid var(--border); margin: 16px 0; }  div { border-radius: 11px; }" +
                                          "code { background: var(--surface-alt); color: var(--text); padding: 3px 6px; border-radius: 5px; font-family: monospace; }" +
                                          "table { display: block; width: max-content; max-width: 100%; overflow-x: auto; border-collapse: separate; border-spacing: 0; border: 1px solid var(--border); border-radius: 10px; margin: 16px 0; font-size: 0.875rem; }" +
                                          "th, td { border-right: 1px solid var(--border); border-bottom: 1px solid var(--border); padding: 8px 12px; text-align: left; vertical-align: top; }" +
                                          "th:last-child, td:last-child { border-right: none; }" +
                                          "tbody tr:last-child td { border-bottom: none; }" +
                                          "th { background: var(--surface); color: var(--text); font-weight: bold; white-space: nowrap; }" +
                                          "tbody tr:nth-child(even) td { background: var(--stripe); }";

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
