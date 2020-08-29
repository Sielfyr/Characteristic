using RimWorld;
using HarmonyLib;
using Verse;
using System;
using Verse.AI;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;

namespace BellCurve
{

    [HarmonyPatch(typeof(Pawn), "get_BodySize")]
    public static class Patch_BodySize
    {
        public static void Postfix(ref float __result, Pawn __instance)
        {
            if (__instance.Characteristic() != null)
            {
                __result += __instance.ageTracker.CurLifeStage.bodySizeFactor * __instance.Characteristic().GetStatOffset(BCStatsDefOf.BodySizeOffset);
            }
        }
    }

    [HarmonyPatch(typeof(Pawn), "get_HealthScale")]
    public static class Patch_HealthScale
    {
        public static void Postfix(ref float __result, Pawn __instance)
        {
            
            if (__instance.Characteristic() != null)
            {
                __result += __instance.ageTracker.CurLifeStage.healthScaleFactor * __instance.Characteristic().GetStatOffset(BCStatsDefOf.HealthScaleOffset);
            }
        }
    }


    [HarmonyPatch(typeof(ThinkNode_ChancePerHour_Nuzzle), "MtbHours")]
    public static class Patch_NuzzleMtbHours
    {
        public static void Postfix(ref float __result, Pawn pawn)
        {
            __result += pawn.GetStatValue(BCStatsDefOf.NuzzleMtbHoursOffset);
        }
    }


    [HarmonyPatch(typeof(PawnUtility), "GetManhunterOnDamageChance", new Type[] { typeof(Pawn), typeof(Thing) })]
    public static class Patch_ManhunterOnDamageChancePT
    {
        public static void Postfix(ref float __result, Pawn pawn, Thing instigator)
        {
            if (instigator == null) __result += Find.Storyteller.difficulty.manhunterChanceOnDamageFactor * pawn.GetStatValue(BCStatsDefOf.ManhunterOnDamageOffset);
        }
    }
    [HarmonyPatch(typeof(PawnUtility), "GetManhunterOnDamageChance", new Type[] { typeof(Pawn), typeof(float), typeof(Thing) })]
    public static class Patch_ManhunterOnDamageChancePFT
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Stloc_0)
                {
                    MethodInfo m_MyExtraMethod = SymbolExtensions.GetMethodInfo(() => MyMethod(null));
                    codes.Insert(++i, new CodeInstruction(OpCodes.Ldarg_0));
                    codes.Insert(++i, new CodeInstruction(OpCodes.Call, m_MyExtraMethod));
                    codes.Insert(++i, new CodeInstruction(OpCodes.Ldloc_0));
                    codes.Insert(++i, new CodeInstruction(OpCodes.Add));
                    codes.Insert(++i, new CodeInstruction(OpCodes.Stloc_0));
                    break;
                }
            }
            return codes.AsEnumerable();
        }
        static float MyMethod(Pawn pawn) 
        {
            return Find.Storyteller.difficulty.manhunterChanceOnDamageFactor * pawn.GetStatValue(BCStatsDefOf.ManhunterOnDamageOffset);
        }
    }


    [HarmonyPatch(typeof(Pawn_MindState), "CheckStartMentalStateBecauseRecruitAttempted")]
    public static class Patch_ManhunterOnTameFail
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldfld && codes[i].operand.ToString().Contains("manhunterOnTameFailChance"))
                {
                    MethodInfo m_MyExtraMethod = SymbolExtensions.GetMethodInfo(() => MyMethod(0, null));
                    codes.Insert(i + 1, codes[i - 3].Clone());
                    codes.Insert(i + 2, codes[i - 2].Clone());
                    codes.Insert(i + 3, new CodeInstruction(OpCodes.Call, m_MyExtraMethod));
                    break;
                }
            }
            return codes.AsEnumerable();
        }
        static float MyMethod(float baseChance, Pawn pawn)
        {
            return baseChance + pawn.GetStatValue(BCStatsDefOf.ManhunterOnTameFailOffset);
        }
    }
}
