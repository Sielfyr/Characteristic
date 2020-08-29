using RimWorld;
using HarmonyLib;
using Verse;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using UnityEngine;
using System.Reflection;

namespace BellCurve
{

    [HarmonyPatch(typeof(Pawn_InteractionsTracker), "SocialFightChance")]
    public static class Patch_SocialFightChance
    {
        private const float figthChanceRedress = 4;
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            CodeInstruction getPawnInstruction = new CodeInstruction(OpCodes.Ldfld);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldfld && codes[i].operand.ToString().Contains("pawn"))
                {
                    getPawnInstruction = codes[i].Clone();
                    break;
                }
            }

            int index = codes.Count - 1 - 2;
            MethodInfo m_MyExtraMethod = SymbolExtensions.GetMethodInfo(() => MyMethod(0, null));
            codes.Insert(index, new CodeInstruction(OpCodes.Ldloc_0));
            codes.Insert(++index, new CodeInstruction(OpCodes.Ldarg_0));
            codes.Insert(++index, getPawnInstruction);
            codes.Insert(++index, new CodeInstruction(OpCodes.Call, m_MyExtraMethod));
            codes.Insert(++index, new CodeInstruction(OpCodes.Stloc_0));

            return codes.AsEnumerable();
        }
        static float MyMethod(float socialFightChance, Pawn pawn)
        {
            float chance = pawn.GetStatValue(BCStatsDefOf.SocialFightFactor);
            if (chance > 1) chance = 1 + (chance - 1) * figthChanceRedress;
            return socialFightChance * chance;
        }
    }


    [HarmonyPatch(typeof(InteractionWorker_KindWords), "RandomSelectionWeight")]
    public class Patch_KindWordsChance
    {
        public static bool Prefix(ref float __result, Pawn initiator)
        {
            __result = Mathf.Clamp01(initiator.GetStatValue(BCStatsDefOf.KindWordChance));
            return false;
        }
    }


    [HarmonyPatch(typeof(NegativeInteractionUtility), "NegativeInteractionChanceFactor")]
    public static class Patch_NegativeInteractionChance
    {
        public static void Postfix(ref float __result, Pawn initiator)
        {
            __result *= Mathf.Clamp(initiator.GetStatValue(BCStatsDefOf.NegativeInteractionFactor), 0, float.MaxValue);
        }
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldsfld && codes[i].operand.ToString().Contains("Kind"))
                {
                    for (int j = i; j >= 0; j--) 
                    {
                        if (codes[j].opcode == OpCodes.Ldarg_0)
                        {
                            for (int k = i; k < codes.Count; k++)
                            {
                                if (codes[j].opcode == OpCodes.Ret)
                                {
                                    codes.RemoveRange(j, k - j + 1);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldsfld && codes[i].operand.ToString().Contains("Abrasive"))
                {
                    for (int j = i; j >= 0; j--)
                    {
                        if (codes[j].opcode == OpCodes.Ldarg_0)
                        {
                            for (int k = i; k < codes.Count; k++)
                            {
                                if (codes[j].opcode == OpCodes.Stloc_0)
                                {
                                    codes.RemoveRange(j, k - j + 1);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return codes.AsEnumerable();
        }
    }
}
