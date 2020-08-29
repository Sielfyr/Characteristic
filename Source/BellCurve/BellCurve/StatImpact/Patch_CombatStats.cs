using RimWorld;
using HarmonyLib;
using Verse;
using System;
using System.Management.Instrumentation;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

namespace BellCurve
{

    [HarmonyPatch(typeof(VerbProperties), "GetDamageFactorFor", new Type[] { typeof(Tool), typeof(Pawn), typeof(HediffComp_VerbGiver)})]
    public static class Patch_DamageFactor
    {

        public static void Postfix(ref float __result, Pawn attacker)
        {
            __result *= attacker.GetStatValue(BCStatsDefOf.MeleeDamageFactor);
        }
    }


}
