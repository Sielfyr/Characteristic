using RimWorld;
using HarmonyLib;
using Verse;

namespace BellCurve
{

    [StaticConstructorOnStartup]
    static class Main
    {

        public const string Id = "Sielfyr.BellCurve";
        public const string ModName = "BellCurve";
        public const string Version = "0.1.1";
        static Main()
        {
            var harmony = new Harmony(Id);
            harmony.PatchAll();
            Log.Message("Initialized " + ModName + " v" + Version);

        }
    }
}
