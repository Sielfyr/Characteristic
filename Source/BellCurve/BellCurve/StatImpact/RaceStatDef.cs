using RimWorld;
using HarmonyLib;
using Verse;
using System.Collections.Generic;

namespace BellCurve
{
    public class RaceStatDef : StatDef
    {

        public string valueName;

        public override IEnumerable<string> ConfigErrors()
        {
            if (valueName == null || valueName == "") yield return "RaceStatDef valueName not set";
            base.ConfigErrors();
        }
    }
}
