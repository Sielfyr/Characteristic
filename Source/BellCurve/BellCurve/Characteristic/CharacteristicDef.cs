using RimWorld;
using HarmonyLib;
using Verse;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;

namespace BellCurve
{
    public class CharacteristicDef : Def
    {

        public List<StatModifier> statOffset;
        public List<StatModifier> statFactor;
        public List<StatModifier> statFactorExp;

        public int displayPriority = 0;

        public int generationRolls;
        public int heredityRolls;

        public float generationDeviation;
        public float heredityDeviation;

        public string Explanation(Pawn pawn, float deviation)
        {

            StringBuilder stringBuilder = new StringBuilder();
            Dictionary<StatDef, string> statExplanation = new Dictionary<StatDef, string>();

            if (!statOffset.NullOrEmpty())
            {
                for (int i = 0; i < statOffset.Count; i++)
                {
                    statExplanation.Add(statOffset[i].stat, "( Offset : " + (statOffset[i].value * deviation).ToString("F2") + " <= " + statOffset[i].value.ToString());
                }
            }
            if (!statFactor.NullOrEmpty())
            {
                for (int i = 0; i < statFactor.Count; i++)
                {
                    if (statExplanation.ContainsKey(statFactor[i].stat)) statExplanation[statFactor[i].stat] += " / Factor : " + (statFactor[i].value * deviation).ToString("F2") + " <= " + statFactor[i].value.ToString();
                    else statExplanation.Add(statFactor[i].stat, "( Factor : " + (statFactor[i].value * deviation).ToString("F2") + " <= " + statFactor[i].value.ToString());
                }
            }
            if (!statFactorExp.NullOrEmpty())
            {
                for (int i = 0; i < statFactorExp.Count; i++)
                {
                    if (statExplanation.ContainsKey(statFactorExp[i].stat)) statExplanation[statFactorExp[i].stat] += " / Exp : " + (statFactorExp[i].value * deviation).ToString("F2") + " <= " + statFactorExp[i].value.ToString();
                    else statExplanation.Add(statFactorExp[i].stat, "( Exp : " + (statFactorExp[i].value * deviation).ToString("F2") + " <= " + statFactorExp[i].value.ToString());
                }
            }

            stringBuilder.AppendLine(description);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Deviation : " + deviation.ToString("F2"));
            stringBuilder.AppendLine();

            foreach (var item in statExplanation)
            {
                stringBuilder.AppendLine("    " + item.Key.LabelCap + " : " /*+ pawn.Characteristic().GetStatOffset(item.Key).ToString("F2") + " " */+ item.Value + " )");
            }

            return stringBuilder.ToString();
        }

        private string StatDesc(StatDef stat)
        {
            return "    " + stat.LabelCap + " : ";
        }
    }
}
