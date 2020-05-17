using System;
using System.Collections;
using MelonLoader;
using UnityEngine;
using NET_SDK;
using NET_SDK.Harmony;
using NET_SDK.Reflection;


namespace AudicaModding
{
    public static class BuildInfo
    {
        public const string Name = "TimingAssist"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "octo & alternity"; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.3"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class TimingAssist : MelonMod
    {
        public static Patch SongSelectItem_OnSelect;
        public static Patch LaunchPlay;
        public static Patch Restart;

        public static IL2CPP_Class SongSelectClass;

        private static float percent = 1.0f;
        private static string selectedSong;
        private bool sliderExists = false;

        public OptionsMenuSlider TimingAssistSlider = new OptionsMenuSlider();

        public override void OnApplicationStart()
        {
            Instance instance = Manager.CreateInstance("TimingAssist");
            TimingAssist.LaunchPlay = instance.Patch(SDK.GetClass("LaunchPanel").GetMethod("Play"), typeof(TimingAssist).GetMethod("PlaySong"));
            TimingAssist.Restart = instance.Patch(SDK.GetClass("InGameUI").GetMethod("Restart"), typeof(TimingAssist).GetMethod("RestartSong"));
            TimingAssist.SongSelectItem_OnSelect = instance.Patch(SDK.GetClass("SongSelectItem").GetMethod("OnSelect"), typeof(TimingAssist).GetMethod("OnSelect"));
        }

        public static void OnSelect(IntPtr @this)
        {
            TimingAssist.SongSelectItem_OnSelect.InvokeOriginal(@this);
            SongSelectItem button = new SongSelectItem(@this);
            string songID = button.mSongData.songID;
            selectedSong = songID;
        }
        public static void RestartSong(IntPtr @this)
        {
            TimingAssist.Restart.InvokeOriginal(@this);
            MelonCoroutines.Start(DisableParticles());
        }

        public static void PlaySong(IntPtr @this)
        {
            TimingAssist.LaunchPlay.InvokeOriginal(@this);
            MelonCoroutines.Start(DisableParticles());
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
        static IEnumerator DisableParticles()
        {
            if (percent < 1.0f)
            {
                yield return new WaitForSeconds(5);

                SongCues.Cue[] cues = SongCues.I.GetCues();
                SongList.SongData song = SongList.I.GetSong(selectedSong);
                SongList.SongData.TempoChange[] tempos = song.tempos;

                for (int i = 0; i < tempos.Length; i++)
                {
                    //MelonModLogger.Log("Tick: " + tempos[i].tick.ToString());
                    //MelonModLogger.Log("Tempo: " + tempos[i].tempo.ToString());

                    float timingWindowMs = 200 * Mathf.Lerp(0.07f, 1.0f, percent);

                    float ticks = timingWindowMs / (60000 / (tempos[i].tempo * 480));
                    float halfTicks = ticks / 2;

                    for (int j = 0; j < cues.Length; j++)
                    {
                        if (cues[j].behavior != Target.TargetBehavior.Chain && cues[j].behavior != Target.TargetBehavior.Dodge && cues[j].behavior != Target.TargetBehavior.Melee)
                        {
                            void UpdateTarget(SongCues.Cue cue)
                            {
                                cues[j].slopAfterTicks = halfTicks;
                                cues[j].slopBeforeTicks = halfTicks;
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

                for (int i = 0; i < cues.Length; i++)
                {
                    if (cues[i].behavior == Target.TargetBehavior.Standard | cues[i].behavior == Target.TargetBehavior.ChainStart | cues[i].behavior == Target.TargetBehavior.Horizontal | cues[i].behavior == Target.TargetBehavior.Vertical | cues[i].behavior == Target.TargetBehavior.Hold)
                    {
                        //cues[i].slopAfterTicks = slopEarly;
                        //cues[i].slopBeforeTicks = slopBefore;

                    }
                    cues[i].particleReductionScale = 0.0f;
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
