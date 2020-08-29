using RimWorld;
using HarmonyLib;
using Verse;
using System;
using System.Collections.Generic;

namespace BellCurve
{

    [HarmonyPatch(typeof(Pawn), "SpecialDisplayStats")]
    public class Patch_SpecialDisplayStats
    {
        public static IEnumerable<StatDrawEntry> Postfix(IEnumerable<StatDrawEntry> statDrawEnumaration, Pawn __instance)
        {
            foreach (var entry in statDrawEnumaration)
            {
                yield return entry;
            }
            if(__instance.Characteristic() != null)
            {
                foreach (var charac in __instance.Characteristic().characteristics)
                {
                    yield return new StatDrawEntry(BCStatCategoryDefOf.PawnCharacteristic, charac.Key.label, charac.Value.ToString("F2"), charac.Key.Explanation(__instance, charac.Value), charac.Key.displayPriority);
                }
            }
        }
    }
}
