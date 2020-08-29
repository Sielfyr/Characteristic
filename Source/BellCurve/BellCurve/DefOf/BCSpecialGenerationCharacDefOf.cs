using RimWorld;
using Verse;

namespace BellCurve
{

    [DefOf]
    public static class BCSpecialGenerationCharacDefOf
    {
        public static SpecialGenerationCharacDef SpecialGenerationCharac;

        static BCSpecialGenerationCharacDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BCSpecialGenerationCharacDefOf));
        }
    }
}
