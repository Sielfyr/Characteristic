using RimWorld;
using HarmonyLib;
using Verse;
using System.Linq;
using System.Collections.Generic;

namespace BellCurve
{
    public static class CharacteristicUtility
    {
        private static Dictionary<Pawn, Pawn_CharacteristicTracker> pawnCharacteristicTracker = new Dictionary<Pawn, Pawn_CharacteristicTracker>();
        /*
        [DebugAction("General", "Characteristic Update All Pawn", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.Playing)]
        private static void UpdateAllCharacteristic()
        {
            List<Pawn> allPawn = new List<Pawn>();
            allPawn.AddRange(Find.WorldPawns.AllPawnsAliveOrDead);
            for (int i = 0; i < Find.Maps.Count; i++)
            {
                allPawn.AddRange(Find.Maps[i].mapPawns.AllPawns);
            }
            for (int i = 0; i < allPawn.Count; i++)
            {
                RedressTrait(allPawn[i]);
            }
        }*/
        public static Pawn_CharacteristicTracker Characteristic(this Pawn pawn)
        {
            if (PawnTrackerSetted(pawn)) return pawnCharacteristicTracker[pawn];
            return null;
        }
        public static Pawn_CharacteristicTracker CreateCharacteristicTracker(this Pawn pawn)
        {
            if (PawnTrackerSetted(pawn))
            {
                Log.Warning(pawn.Name.ToStringFull + "already have a Pawn_CharacteristicTracker");
                return pawnCharacteristicTracker[pawn];
            }
            Pawn_CharacteristicTracker tracker = new Pawn_CharacteristicTracker(pawn);
            pawnCharacteristicTracker.Add(pawn, tracker);
            tracker.Init();
            RedressTrait(pawn);
            return tracker;
        }
        public static Pawn_CharacteristicTracker CreateCharacteristicTracker(this Pawn pawn, Pawn_CharacteristicTracker tracker)
        {
            if (PawnTrackerSetted(pawn))
            {
                Log.Message(pawn.Name.ToStringFull + "already have a Pawn_CharacteristicTracker, are you sure you should replace it?");
                RemoveCharacteristicTracker(pawn);
            }
            pawnCharacteristicTracker.Add(pawn, tracker);
            RedressTrait(pawn);
            return tracker;
        }
        public static void RemoveCharacteristicTracker(this Pawn pawn)
        {
            if (PawnTrackerSetted(pawn))
            {
                pawnCharacteristicTracker.Remove(pawn);
            }
        }

        private static bool PawnTrackerSetted(Pawn pawn)
        {
            return (pawnCharacteristicTracker.ContainsKey(pawn));
        }


        private static void RedressTrait(Pawn pawn)
        {
            if (pawn.story?.traits?.allTraits == null) return;
            List<CharacterizationDef> characterization = DefDatabase<CharacterizationDef>.AllDefsListForReading;
            pawn.story.traits.allTraits.RemoveAll((Trait tr) => characterization.Any((CharacterizationDef tc) => tc.traitDef == tr.def));

            List<CharacterizationDef> needCharacterization = characterization.ListFullCopy();
            int index = 0;
            while(needCharacterization.Count > 0)
            {
                for (int i = needCharacterization.Count - 1; i >= 0; i--)
                {
                    if (needCharacterization[i].traitImpact == null
                        || !needCharacterization[i].traitImpact.Any(tci => needCharacterization.Any((CharacterizationDef c) => tci.trait == c.traitDef)))
                    {
                        needCharacterization[i].TryActivate(pawn);
                        needCharacterization.RemoveAt(i);
                    }
                }
                if (++index > 100)
                {
                    Log.Error("BellCurve : Some CharacterizationDef are interdependant cannot resolve Characterization order");
                    break;
                }
            }
        }

        public static float GetStatOrigin(Pawn pawn, StatDef stat)
        {
            if (stat is RaceStatDef)
            {
                return (float)pawn.RaceProps.GetType().GetField((stat as RaceStatDef).valueName).GetValue(pawn.RaceProps);
            }
            return pawn.def.GetStatValueAbstract(stat);
        }

    }
}
