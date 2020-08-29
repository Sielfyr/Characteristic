using RimWorld;
using Verse;

namespace BellCurve
{

    [DefOf]
    public static class BCStatsDefOf
    {
        public static StatDef BodySizeOffset;

        public static StatDef HealthScaleOffset;

        public static StatDef MeleeDamageFactor;

        public static StatDef ManhunterOnDamageOffset;

        public static StatDef ManhunterOnTameFailOffset;

        public static StatDef NuzzleMtbHoursOffset;

        public static StatDef ManipulationFactor;

        public static StatDef MovingFactor;

        public static StatDef BloodPumpingFactor;

        public static StatDef BreathingFactor;

        public static StatDef SocialFightFactor;

        public static StatDef KindWordChance;

        public static StatDef NegativeInteractionFactor;

        static BCStatsDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BCStatsDefOf));
        }
    }
}
