using RimWorld;
using HarmonyLib;
using Verse;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace BellCurve
{
    public class CharacterizationDef : Def
    {
        public TraitDef traitDef;

        public List<StatCharacterizationImpactement> statImpact;
        public List<CharacteristicCharacterizationImpactement> characImpact;
        public List<TraitCharacterizationImpactement> traitImpact;


        public List<DeviatiableCharacteristic> deviatable;

        public float degreeActivation;

        public void TryActivate(Pawn pawn)
        {
            if (pawn.story?.traits?.allTraits == null) return;

            float value;
            float weigth = 0;
            if (!statImpact.NullOrEmpty())
            {
                for (int i = 0; i < statImpact.Count; i++)
                {
                    value = pawn.Characteristic().GetStatOffset(statImpact[i].stat);
                    if (statImpact[i].factorStep) value /= CharacteristicUtility.GetStatOrigin(pawn, statImpact[i].stat);
                    value = ((int)(value / statImpact[i].step)) * statImpact[i].weigth;
                    if (statImpact[i].shouldReachWeigth != null && statImpact[i].shouldReachWeigth > value) return;
                    weigth += value;
                }
            }
            if (!characImpact.NullOrEmpty())
            {
                for (int i = 0; i < characImpact.Count; i++)
                {
                    value = pawn.Characteristic().characteristics[characImpact[i].characteristic];
                    value = ((int)(value / characImpact[i].step)) * characImpact[i].weigth;
                    if (characImpact[i].shouldReachWeigth != null && characImpact[i].shouldReachWeigth > value) return;
                    weigth += value;
                }
            }
            if (!traitImpact.NullOrEmpty())
            {
                for (int i = 0; i < traitImpact.Count; i++)
                {
                    if(pawn.story.traits.HasTrait(traitImpact[i].trait)) weigth += traitImpact[i].weigth;
                }
            }

            if(traitDef.degreeDatas.Count == 1)
            {
                if (weigth >= degreeActivation) pawn.story.traits.GainTrait(new Trait(traitDef,forced: true));
            }
            else
            {
                List<int> degrees = new List<int>();
                if (weigth > 0)
                {
                    for (int i = 0; i < traitDef.degreeDatas.Count; i++)
                    {
                        if (traitDef.degreeDatas[i].degree > 0) degrees.Add(traitDef.degreeDatas[i].degree);
                    }
                    if (degrees.Count == 0) return;
                    degrees.Sort();
                    for (int i = degrees.Count - 1; i >= 0; i--)
                    {
                        if (weigth >= degrees[i] * degreeActivation)
                        {
                            pawn.story.traits.GainTrait(new Trait(traitDef, degree: degrees[i], forced: true));
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < traitDef.degreeDatas.Count; i++)
                    {
                        if (traitDef.degreeDatas[i].degree < 0) degrees.Add(traitDef.degreeDatas[i].degree);
                    }
                    if (degrees.Count == 0) return;
                    degrees.Sort();
                    for (int i = 0; i < degrees.Count; i++)
                    {
                        if (weigth <= degrees[i] * degreeActivation) 
                        {
                            pawn.story.traits.GainTrait(new Trait(traitDef, degree: degrees[i], forced: true));
                            break;
                        }
                    }
                }
            }

        }
    }
}
