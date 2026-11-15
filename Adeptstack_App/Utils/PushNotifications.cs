namespace Adeptstack_App.Utils
{
    /// <summary>
    /// OneSignal liefert nur Implementierungen für Android und iOS aus. Auf allen
    /// anderen Plattformen zieht NuGet den netstandard2.0-Stub, dessen statischer
    /// Konstruktor eine NotImplementedException wirft - schon das Berühren des
    /// OneSignal-Typs killt dort die App. Deshalb läuft alles über diesen Wrapper.
    /// </summary>
    static class PushNotifications
    {
        private const string ChangelogTag = "WantsChangelogs";

        public static bool IsSupported
        {
#if ANDROID || IOS
            get => true;
#else
            get => false;
#endif
        }

        public static void Initialize(string appId)
        {
#if ANDROID || IOS
            OneSignalSDK.DotNet.OneSignal.Initialize(appId);
            _ = OneSignalSDK.DotNet.OneSignal.Notifications.RequestPermissionAsync(true);
            EnsureChangelogTag();
#endif
        }

        /// <summary>
        /// Ob das System Benachrichtigungen erlaubt. Ohne Berechtigung kommt trotz Tag nichts an.
        /// </summary>
        public static bool HasPermission
        {
#if ANDROID || IOS
            get => OneSignalSDK.DotNet.OneSignal.Notifications.Permission;
#else
            get => false;
#endif
        }

        /// <summary>
        /// Lokale Kopie der Wahl, Standard "an". Direkt nach dem Start hat OneSignal die Tags
        /// eventuell noch nicht geladen, dann darf ein fehlendes Tag kein bewusstes "false" überschreiben.
        /// </summary>
        private static bool LocalWantsChangelogs
        {
            get => Preferences.Default.Get(ChangelogTag, true);
            set => Preferences.Default.Set(ChangelogTag, value);
        }

        public static bool WantsChangelogs()
        {
#if ANDROID || IOS
            string tagValue = GetChangelogTag();

            if (tagValue != null)
            {
                LocalWantsChangelogs = tagValue.ToLower() == "true";
            }
#endif
            return LocalWantsChangelogs;
        }

        /// <summary>
        /// Immer ausdrücklich "true" oder "false": Das Backend schickt Release-Pushes nur an "true",
        /// ein fehlendes Tag sieht dort aus wie "aus".
        /// </summary>
        public static void SetWantsChangelogs(bool wanted)
        {
            LocalWantsChangelogs = wanted;
#if ANDROID || IOS
            OneSignalSDK.DotNet.OneSignal.User.AddTag(ChangelogTag, wanted ? "true" : "false");
#endif
        }

        /// <summary>
        /// Fehlt das Tag (Neuinstallation, neues Gerät, frühere Versionen haben es beim Ausschalten gelöscht),
        /// wird es aus der lokalen Wahl nachgetragen. Sonst zeigt der Schalter "an", aber es kommt nichts.
        /// </summary>
        private static void EnsureChangelogTag()
        {
#if ANDROID || IOS
            if (GetChangelogTag() == null)
            {
                OneSignalSDK.DotNet.OneSignal.User.AddTag(ChangelogTag, LocalWantsChangelogs ? "true" : "false");
            }
#endif
        }

#if ANDROID || IOS
        private static string GetChangelogTag()
        {
            IDictionary<string, string> tags = OneSignalSDK.DotNet.OneSignal.User.GetTags();
            return tags != null && tags.TryGetValue(ChangelogTag, out string tagValue) ? tagValue : null;
        }
#endif
    }
}
