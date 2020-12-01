using MelonLoader;
using System.Reflection;

namespace TimingAssist
{
    public static class Config
    {
        public const string Category = "TimingAssist";

        public static bool Enabled;

        public static float TimingAssistAmount;


        public static void RegisterConfig()
        {
            MelonPrefs.RegisterBool(Category, nameof(Enabled), true, "Enables the mod.");

            MelonPrefs.RegisterFloat(Category, nameof(TimingAssistAmount), 1.0f, "Limits the timing window [0,1,0.05,1] {P}");

            OnModSettingsApplied();
        }

        public static void OnModSettingsApplied()
        {
            foreach (var fieldInfo in typeof(Config).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (fieldInfo.FieldType == typeof(bool))
                    fieldInfo.SetValue(null, MelonPrefs.GetBool(Category, fieldInfo.Name));
                
                if (fieldInfo.FieldType == typeof(float))
                    fieldInfo.SetValue(null, MelonPrefs.GetFloat(Category, fieldInfo.Name));
            }
        }
    }
}
