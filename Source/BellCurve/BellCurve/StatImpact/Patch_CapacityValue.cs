using RimWorld;
using HarmonyLib;
using Verse;
using System;

namespace BellCurve
{

    [HarmonyPatch(typeof(PawnCapacityWorker_Manipulation), "CalculateCapacityLevel")]
    public class Patch_Manipulation
    {
        public static void Postfix(ref float __result, HediffSet diffSet)
        {
            __result *= diffSet.pawn.GetStatValue(BCStatsDefOf.ManipulationFactor);
        }
    }
    [HarmonyPatch(typeof(PawnCapacityWorker_Moving), "CalculateCapacityLevel")]
    public class Patch_Moving
    {
        public static void Postfix(ref float __result, HediffSet diffSet)
        {
            __result *= diffSet.pawn.GetStatValue(BCStatsDefOf.MovingFactor);
        }
    }
    [HarmonyPatch(typeof(PawnCapacityWorker_Breathing), "CalculateCapacityLevel")]
    public class Patch_Breathing
    {
        public static void Postfix(ref float __result, HediffSet diffSet)
        {
            __result *= diffSet.pawn.GetStatValue(BCStatsDefOf.BreathingFactor);
        }
    }
    [HarmonyPatch(typeof(PawnCapacityWorker_BloodPumping), "CalculateCapacityLevel")]
    public class Patch_BloodPumping
    {
        public static void Postfix(ref float __result, HediffSet diffSet)
        {
            __result *= diffSet.pawn.GetStatValue(BCStatsDefOf.BloodPumpingFactor);
        }
    }
}
