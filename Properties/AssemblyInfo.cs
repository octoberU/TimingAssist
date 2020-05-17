using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;
using AudicaModding;

[assembly: AssemblyTitle(AudicaModding.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(AudicaModding.BuildInfo.Company)]
[assembly: AssemblyProduct(AudicaModding.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + AudicaModding.BuildInfo.Author)]
[assembly: AssemblyTrademark(AudicaModding.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(AudicaModding.BuildInfo.Version)]
[assembly: AssemblyFileVersion(AudicaModding.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonModInfo(typeof(AudicaModding.TimingAssist), AudicaModding.BuildInfo.Name, AudicaModding.BuildInfo.Version, AudicaModding.BuildInfo.Author, AudicaModding.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonModGame(null, null)]