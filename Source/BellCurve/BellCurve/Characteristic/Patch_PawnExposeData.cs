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
        [HarmonyPatch(typeof(Pawn), "ExposeData")]
        public static class Patch_PawnExposeData
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                bool found = false;
                for (int i = 0; i < codes.Count; i++)
                {
                    yield return codes[i];
                    if (codes[i].opcode == OpCodes.Ldstr && (codes[i].operand as string) == "skills") found = true;
                    if (found && codes[i].opcode == OpCodes.Call)
                    {
                        MethodInfo m_MyExtraMethod = SymbolExtensions.GetMethodInfo(() => ExposeData(null));
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call, m_MyExtraMethod);
                        for (int j = i + 1; j < codes.Count; j++) yield return codes[j];
                        break;
                    }
                }
            }
            static void ExposeData(Pawn pawn)
            {
                Pawn_CharacteristicTracker charac = pawn.Characteristic();
                bool shouldAddTracker = (charac == null) ? true : false;
                Scribe_Deep.Look(ref charac, "characteristics", pawn);
                if (charac != null && shouldAddTracker) pawn.CreateCharacteristicTracker(charac);
            }
        }
    }
