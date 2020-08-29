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
    [HarmonyPatch(typeof(StatWorker), "GetValueUnfinalized")]
    public static class Patch_GetValueUnfinalized
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            CodeInstruction statFromWorker = new CodeInstruction(OpCodes.Ldfld);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldfld && codes[i].operand.ToString().Contains("StatDef") && codes[i - 1].opcode == OpCodes.Ldarg_0)
                {
                    statFromWorker = codes[i].Clone();
                    break;
                }
            }
            
            for (int i = 0; i < codes.Count; i++)
            {
                yield return codes[i];

                if (codes[i + 2].opcode == OpCodes.Ldfld && codes[i + 2].operand.ToString().Contains("skills"))
                {
                    MethodInfo m_MyExtraMethod = SymbolExtensions.GetMethodInfo(() => MyMethod(null, null));
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return statFromWorker;
                    yield return new CodeInstruction(OpCodes.Call, m_MyExtraMethod);
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Add);
                    yield return new CodeInstruction(OpCodes.Stloc_0);

                    for (int j = i + 1; j < codes.Count; j++) yield return codes[j];
                    break;
                }
            }
        }
        
        static float MyMethod(Pawn pawn, StatDef stat)
        {
            return pawn.Characteristic()?.GetStatOffset(stat) ?? 0;
        }
    }
}
