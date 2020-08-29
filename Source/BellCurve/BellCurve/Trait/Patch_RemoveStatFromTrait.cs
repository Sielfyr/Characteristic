using RimWorld;
using HarmonyLib;
using Verse;
using System.Collections.Generic;

namespace BellCurve
{
    [HarmonyPatch(typeof(Page_ConfigureStartingPawns), "PreOpen")]
    public static class Patch_RemoveStatFromTraitInPawnPreGameUI
    {
        public static void Postfix()
        {
            List<CharacterizationDef> characterizations = DefDatabase<CharacterizationDef>.AllDefsListForReading;
            for (int i = 0; i < characterizations.Count; i++)
            {
                for (int j = 0; j < characterizations[i].traitDef.degreeDatas.Count; j++)
                {
                    characterizations[i].traitDef.degreeDatas[j].statFactors = null;
                    characterizations[i].traitDef.degreeDatas[j].statOffsets = null;
                }
            }
        }
    }
    [HarmonyPatch(typeof(Game), "FinalizeInit")]
    public static class Patch_RemoveStatFromTraitOnGameStart
    {
        public static void Postfix()
        {
            List<CharacterizationDef> characterizations = DefDatabase<CharacterizationDef>.AllDefsListForReading;
            for (int i = 0; i < characterizations.Count; i++)
            {
                for (int j = 0; j < characterizations[i].traitDef.degreeDatas.Count; j++)
                {
                    characterizations[i].traitDef.degreeDatas[j].statFactors = null;
                    characterizations[i].traitDef.degreeDatas[j].statOffsets = null;
                }
            }
        }
    }
}
