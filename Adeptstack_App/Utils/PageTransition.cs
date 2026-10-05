namespace Adeptstack_App.Utils
{
    /// <summary>
    /// Schwere UI-Arbeit (viele Karten mit Bildern, WebView) während der Einschub-Animation blockiert den UI-Thread:
    /// die Seite bleibt auf halbem Weg stehen, bis alles aufgebaut ist. Seiten laden deshalb sofort,
    /// rendern aber erst, wenn die Animation durch ist.
    /// </summary>
    static class PageTransition
    {
        // Etwas länger als die Push-Animation von Android und iOS
        private const int AnimationDuration = 350;

        /// <summary>
        /// Im Konstruktor aufrufen, damit das Loaded-Event nicht verpasst wird.
        /// </summary>
        public static async Task WaitAsync(Page page)
        {
            if (!page.IsLoaded)
            {
                var loaded = new TaskCompletionSource();
                void OnLoaded(object sender, EventArgs e)
                {
                    page.Loaded -= OnLoaded;
                    loaded.TrySetResult();
                }

                page.Loaded += OnLoaded;
                await loaded.Task;
            }

            await Task.Delay(AnimationDuration);
        }
    }
}
