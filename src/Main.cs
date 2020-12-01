using System;
using MelonLoader;
using UnityEngine;
using Harmony;
using TimingAssist;


[assembly: MelonGame("Harmonix Music Systems, Inc.", "Audica")]
[assembly: MelonInfo(typeof(TimingAssistMod), "Timing Assist", "1.1.2", "octo & Alternity", "https://github.com/octoberU/TimingAssist")]

public class TimingAssistMod : MelonMod
{
    public override void OnApplicationStart()
    {
        Config.RegisterConfig();
    }

    public override void OnModSettingsApplied()
    {
        Config.OnModSettingsApplied();
    }

    private static float percent => Config.TimingAssistAmount;

    [HarmonyPatch(typeof(AudioDriver), "StartPlaying", new Type[0])]
    private static class SetCueTimingWindow
    {
        private static void Postfix(AudioDriver __instance)
        {
            if (!Config.Enabled) return;
            SongCues.Cue[] cues = SongCues.I.GetCues();
            SongList.SongData song = SongList.I.GetSong(SongDataHolder.I.songData.songID);
            SongList.SongData.TempoChange[] tempos = song.tempos;

            for (int i = 0; i < tempos.Length; i++)
            {
                float timingWindowMs = 200 * Mathf.Lerp(0.07f, 1.0f, percent);

                float ticks = timingWindowMs / (60000 / (tempos[i].tempo * 480));
                float halfTicks = ticks / 2;

                for (int j = 0; j < cues.Length; j++)
                {
                    if (cues[j].behavior != Target.TargetBehavior.Chain && cues[j].behavior != Target.TargetBehavior.Dodge && cues[j].behavior != Target.TargetBehavior.Melee)
                    {
                        void UpdateTarget(SongCues.Cue cue)
                        {
                            cue.slopAfterTicks = halfTicks;
                            cue.slopBeforeTicks = halfTicks;
                        }
                        if (cues[j].tick >= tempos[i].tick)
                        {
                            if (tempos.Length >= tempos.Length + 1 && cues[j].tick < tempos[i + 1].tick)
                            {
                                UpdateTarget(cues[j]);
                            }
                            else if (tempos.Length < tempos.Length + 1)
                            {
                                UpdateTarget(cues[j]);
                            }
                        }
                    }
                }
            }
        }
    }
}
