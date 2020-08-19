using Harmony;
using System.Reflection;

namespace AudicaModding
{
    internal static class Hooks
    {
        public static void ApplyHooks(HarmonyInstance instance)
        {
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(SongSelectItem), "OnSelect")]
        private static class PatchSongOnSelect
        {
            private static void Postfix(SongSelectItem __instance)
            {
                AudicaMod.OnSelect(__instance);
            }
        }

        [HarmonyPatch(typeof(LaunchPanel), "Play")]
        private static class PatchPlay
        {
            private static void Postfix(LaunchPanel __instance)
            {
                AudicaMod.PlaySong();
            }
        }

        [HarmonyPatch(typeof(InGameUI), "Restart")]
        private static class PatchRestart
        {
            private static void Postfix(InGameUI __instance)
            {
                AudicaMod.PlaySong();
            }
        }

    }
}