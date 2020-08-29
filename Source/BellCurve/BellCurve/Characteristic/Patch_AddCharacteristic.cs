using RimWorld;
using HarmonyLib;
using Verse;
using System.Collections.Generic;
using System;
using System.Reflection.Emit;
using System.Linq;
using System.Reflection;

namespace BellCurve
{

    [HarmonyPatch(typeof(PawnGenerator), "TryGenerateNewPawnInternal")]
    public static class Patch_GeneratePawn
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Contains("GenerateSkills"))
                {
                    MethodInfo m_MyExtraMethod = SymbolExtensions.GetMethodInfo(() => GenerateCharacteristic(null, new PawnGenerationRequest()));
                    CodeInstruction saved = codes[++i].Clone();
                    codes[i].opcode = OpCodes.Ldloc_0;
                    codes[i].operand = null;
                    codes.Insert(++i, new CodeInstruction(OpCodes.Ldarg_0));
                    codes.Insert(++i, new CodeInstruction(OpCodes.Ldobj, typeof(PawnGenerationRequest)));
                    codes.Insert(++i, new CodeInstruction(OpCodes.Call, m_MyExtraMethod));
                    codes.Insert(++i, saved);
                    break;
                }
            }
            return codes.AsEnumerable();

        }

        static void GenerateCharacteristic(Pawn pawn, PawnGenerationRequest request) 
        {
            if(!request.Newborn) CharacteristicUtility.CreateCharacteristicTracker(pawn);
        }
    }

    [HarmonyPatch(typeof(Hediff_Pregnant), "DoBirthSpawn")]
    public static class Patch_PregnantBirth
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldsfld && codes[i].operand.ToString().Contains("GaveBirth"))
                {
                    MethodInfo m_MyExtraMethod = SymbolExtensions.GetMethodInfo(() => GenerateCharacteristic(null));
                    CodeInstruction saved = codes[i].Clone();
                    codes[i].opcode = OpCodes.Ldloc_2;
                    codes[i].operand = null;
                    codes.Insert(++i, new CodeInstruction(OpCodes.Call, m_MyExtraMethod));
                    codes.Insert(++i, saved);
                    break;
                }
            }
            return codes.AsEnumerable();

        }

        static void GenerateCharacteristic(Pawn pawn)
        {
            CharacteristicUtility.CreateCharacteristicTracker(pawn);
        }
    }
}
