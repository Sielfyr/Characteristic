using RimWorld;
using HarmonyLib;
using Verse;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;

namespace BellCurve
{
    public class Pawn_CharacteristicTracker : IExposable
    {
        public Pawn pawn;

        public Dictionary<CharacteristicDef, float> characteristics;

        private Dictionary<StatDef, float> statsOffset;

        public Pawn_CharacteristicTracker(Pawn pawn)
        {
            this.pawn = pawn;
        }
        public void ExposeData()
        {
            Scribe_Collections.Look(ref characteristics, "characteristics", LookMode.Def, LookMode.Value);
            Scribe_Collections.Look(ref statsOffset, "statsOffset", LookMode.Def, LookMode.Value);
        }

        public float GetStatOffset(StatDef stat)
        {
            if (statsOffset.ContainsKey(stat)) return statsOffset[stat];
            return 0;
        }

        public void Init()
        {
            GenerateCharacteristics();
            CalculateStatsOffset();
        }
        private void GenerateCharacteristics()
        {
            characteristics = new Dictionary<CharacteristicDef, float>();

            List<CharacteristicDef> allCharacteristic = DefDatabase<CharacteristicDef>.AllDefsListForReading;
            float deviation;
            float origin;
            int originCount;
            Pawn_CharacteristicTracker familyCharac;

            List<Pawn> pawnFamily = new List<Pawn>();
            if (pawn.relations?.Children != null) pawnFamily.AddRange(pawn.relations.Children);
            if (pawn.GetFather() != null) pawnFamily.Add(pawn.GetFather());
            if (pawn.GetMother() != null) pawnFamily.Add(pawn.GetMother());

            Dictionary<CharacteristicDef, FloatRange> storyDeviation = DeviationStory();

            List<int> specialDeviation = new List<int>();
            List<int> possibleIndex = new List<int>();

            for (int i = 0; i < allCharacteristic.Count; i++)
            {
                if (storyDeviation == null || !storyDeviation.ContainsKey(allCharacteristic[i])) possibleIndex.Add(i);
            }
            for (int s = 0; s < BCSpecialGenerationCharacDefOf.SpecialGenerationCharac.number - (storyDeviation?.Count ?? 0); s++)
            {
                specialDeviation.Add(possibleIndex[Rand.RangeInclusive(0, possibleIndex.Count - 1)]);
                possibleIndex.Remove(specialDeviation.Last());
            }
            for (int i = 0; i < allCharacteristic.Count; i++)
            {
                deviation = 0;
                originCount = 0;
                origin = 0;

                if (pawnFamily.Count > 0)
                {
                    for (int j = 0; j < allCharacteristic[i].heredityRolls; j++)
                    {
                        deviation += Rand.Value;
                    }
                    deviation = ((deviation / allCharacteristic[i].heredityRolls) * 2 - 1) * allCharacteristic[i].heredityDeviation;

                    for (int k = 0; k < pawnFamily.Count; k++)
                    {
                        familyCharac = pawnFamily[k].Characteristic();
                        if (familyCharac == null)
                        {
                            if (pawn.relations?.Children?.Contains(pawnFamily[k]) ?? true) continue;
                            else familyCharac = pawnFamily[k].CreateCharacteristicTracker();
                        }
                        originCount++;
                        origin += familyCharac.characteristics[allCharacteristic[i]];
                    }
                    if (originCount > 0) origin /= originCount;
                }
                else
                {
                    if (specialDeviation.Contains(i))
                    {
                        for (int j = 0; j < BCSpecialGenerationCharacDefOf.SpecialGenerationCharac.rolls; j++)
                        {
                            deviation += Rand.Value;
                        }
                        deviation = ((deviation / BCSpecialGenerationCharacDefOf.SpecialGenerationCharac.rolls) * 2 - 1) * BCSpecialGenerationCharacDefOf.SpecialGenerationCharac.deviation;
                    }
                    else
                    {
                        for (int j = 0; j < allCharacteristic[i].generationRolls; j++)
                        {
                            deviation += Rand.Value;
                        }
                        deviation = ((deviation / allCharacteristic[i].generationRolls) * 2 - 1) * allCharacteristic[i].generationDeviation;
                    }
                }
                if (storyDeviation != null && storyDeviation.ContainsKey(allCharacteristic[i]))
                {
                    float closerToZero = 0;
                    float newmiddle;
                    if (!storyDeviation[allCharacteristic[i]].Includes(0))
                    {
                        closerToZero = storyDeviation[allCharacteristic[i]].min > 0
                            ? storyDeviation[allCharacteristic[i]].min
                            : storyDeviation[allCharacteristic[i]].max;
                    }
                    newmiddle = storyDeviation[allCharacteristic[i]].Average - ((storyDeviation[allCharacteristic[i]].Average - closerToZero) * 2f / 3f);
                    if(deviation >= 0)
                    {
                        deviation = Mathf.Lerp(newmiddle, storyDeviation[allCharacteristic[i]].max, deviation / allCharacteristic[i].generationDeviation);
                    }
                    else
                    {
                        deviation = Mathf.Lerp(newmiddle, storyDeviation[allCharacteristic[i]].min, -deviation / allCharacteristic[i].generationDeviation);
                    }
                    origin = 0;
                }
                characteristics.Add(allCharacteristic[i], origin + deviation);
            }
        }


        private Dictionary<CharacteristicDef, FloatRange> DeviationStory()
        {
            List<TraitEntry> forcedTrait = new List<TraitEntry>();
            if (pawn.story?.childhood?.forcedTraits != null)
            {
                forcedTrait.AddRange(pawn.story.childhood.forcedTraits);
            }
            if (pawn.story?.adulthood?.forcedTraits != null)
            {
                forcedTrait.AddRange(pawn.story.adulthood.forcedTraits);
            }
            if (forcedTrait.NullOrEmpty()) return null;

            CharacterizationDef characterization;
            DeviatiableCharacteristic deviatable;
            Dictionary<CharacteristicDef, FloatRange> deviation = new Dictionary<CharacteristicDef, FloatRange>();
            for (int i = 0; i < forcedTrait.Count; i++)
            {
                characterization = DefDatabase<CharacterizationDef>.AllDefsListForReading.Find(ch => ch.traitDef == forcedTrait[i].def);
                if (characterization == null) continue;
                if (characterization.deviatable == null) Log.Error("BellCurve : no deviatable for : " + characterization.defName);
                for (int d = 0; d < characterization.deviatable.Count; d++)
                {
                    FloatRange range;
                    deviatable = characterization.deviatable[d];
                    if (!deviation.ContainsKey(deviatable.characteristic))
                    {
                        range = new FloatRange(-deviatable.characteristic.generationDeviation, deviatable.characteristic.generationDeviation);
                    }
                    else range = deviation[deviatable.characteristic];

                    int degreeFactor = forcedTrait[i].degree == 0 ? 1 : forcedTrait[i].degree;
                    if ((forcedTrait[i].degree >= 0 && !deviatable.inverse) ||(forcedTrait[i].degree < 0 && deviatable.inverse))
                    {
                        if (range.min < deviatable.value * degreeFactor) range.min = deviatable.value * degreeFactor;
                    }
                    else if (range.max > deviatable.value * degreeFactor) range.max = deviatable.value * degreeFactor;
                    
                    if (!deviation.ContainsKey(deviatable.characteristic)) deviation.Add(deviatable.characteristic, range);
                    else deviation[deviatable.characteristic] = range;
                }
            }
            return deviation;
        }


        private void CalculateStatsOffset()
        {
            statsOffset = new Dictionary<StatDef, float>();

            List<StatDef> statDefs = DefDatabase<StatDef>.AllDefsListForReading;
            StatModifier modifier;
            float factorValue;
            float expValue;
            float origine;
            for (int i = 0; i < statDefs.Count; i++)
            {
                factorValue = 0;
                expValue = 0;
                origine = 0;
                statsOffset.Add(statDefs[i], 0);
                foreach (var charac in characteristics)
                {
                    modifier = charac.Key.statOffset?.Find(stat => stat.stat == statDefs[i]) ?? null;
                    if (modifier != null)
                    {
                        statsOffset[statDefs[i]] += modifier.value * charac.Value;
                    }
                    modifier = charac.Key.statFactor?.Find(stat => stat.stat == statDefs[i]) ?? null;
                    if (modifier != null)
                    {
                        origine = CharacteristicUtility.GetStatOrigin(pawn, statDefs[i]);
                        factorValue += charac.Value * modifier.value;
                    }
                    modifier = charac.Key.statFactorExp?.Find(stat => stat.stat == statDefs[i]) ?? null;
                    if (modifier != null)
                    {
                        origine = CharacteristicUtility.GetStatOrigin(pawn, statDefs[i]);
                        expValue += charac.Value * modifier.value;
                    }
                }
                if (factorValue != 0) statsOffset[statDefs[i]] += origine * (1f + (factorValue)) - origine;
                if (expValue != 0) statsOffset[statDefs[i]] += origine * Mathf.Exp(expValue) - origine;
                if (statsOffset[statDefs[i]] == 0) statsOffset.Remove(statDefs[i]);
            }
        }


    }
}
