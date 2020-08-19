using System;
using System.Collections;
using MelonLoader;
using UnityEngine;
using Harmony;

namespace AudicaModding
{
    public class AudicaMod : MelonMod
    {
        private static float percent = 1.0f;
        private static string selectedSong;
        private bool sliderExists = false;
        public static class BuildInfo
        {
            public const string Name = "TimingAssist"; // Name of the Mod.  (MUST BE SET)
            public const string Author = "octo & alternity"; // Author of the Mod.  (Set as null if none)
            public const string Company = null; // Company that made the Mod.  (Set as null if none)
            public const string Version = "1.1.0"; // Version of the Mod.  (MUST BE SET)
            public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
        }

        public OptionsMenuSlider TimingAssistSlider = new OptionsMenuSlider();

        public static void OnSelect(SongSelectItem button)
        {
            string songID = button.mSongData.songID;
            selectedSong = songID;
        }

        public static void PlaySong()
        {
            MelonCoroutines.Start(ChangeTimingWindow());
        }

        public void UpdateSlider()
        {
            if (this.TimingAssistSlider == null)
            {
                return;
            }
            else
            {
                if(percent > 1.0f)
                {
                    percent = 1.0f;
                }
                if (percent < 0.0f)
                {
                    percent = 0.0f;
                }
                this.TimingAssistSlider.label.text = "Timing Assist " + (percent * 100).ToString("F") + "%";
            }
        }
        static IEnumerator ChangeTimingWindow()
        {
            if (percent < 1.0f)
            {
                yield return new WaitForSeconds(5);

                SongCues.Cue[] cues = SongCues.I.GetCues();
                SongList.SongData song = SongList.I.GetSong(selectedSong);
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
        public override void OnUpdate()
        {
            if(OptionsMenu.I != null)
            {
                if(!sliderExists & OptionsMenu.I.mPage == OptionsMenu.Page.Gameplay)
                {
                    {
                        OptionsMenu.I.AddHeader(0, "TimingAssist");

                        this.TimingAssistSlider = OptionsMenu.I.AddSlider(0, "Timing Assist", "P", new Action<float>((float n) => { percent += (n * 0.05f); MelonModLogger.Log(percent.ToString()); UpdateSlider(); }), null, null, "Controls the size of the timing window");
                        this.TimingAssistSlider.label.text = "Timing Assist " + (percent * 100).ToString("F") + "%";

                        sliderExists = true;
                    }
                }
                if (sliderExists & OptionsMenu.I.mPage != OptionsMenu.Page.Gameplay)
                {
                    sliderExists = false;
                }
            }      
        }
    }
}
