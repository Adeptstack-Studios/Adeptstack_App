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
#endif
        }

        public static bool WantsChangelogs()
        {
#if ANDROID || IOS
            IDictionary<string, string> tags = OneSignalSDK.DotNet.OneSignal.User.GetTags();

            if (tags != null && tags.TryGetValue(ChangelogTag, out string tagValue))
            {
                return tagValue.ToLower() == "true";
            }
#endif
            return true;
        }

        public static void SetWantsChangelogs(bool wanted)
        {
#if ANDROID || IOS
            if (wanted)
            {
                OneSignalSDK.DotNet.OneSignal.User.AddTag(ChangelogTag, "true");
            }
            else
            {
                OneSignalSDK.DotNet.OneSignal.User.RemoveTag(ChangelogTag);
            }
#endif
        }
    }
}
