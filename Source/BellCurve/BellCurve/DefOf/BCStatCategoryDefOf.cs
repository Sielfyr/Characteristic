using RimWorld;
using Verse;

namespace BellCurve
{
    [DefOf]
    public static class BCStatCategoryDefOf
    {
        public static StatCategoryDef PawnCharacteristic;

        static BCStatCategoryDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BCStatCategoryDefOf));
        }
    }
}
