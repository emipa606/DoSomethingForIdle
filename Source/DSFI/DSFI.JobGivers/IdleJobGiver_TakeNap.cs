using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.JobGivers;

public class IdleJobGiver_TakeNap : IdleJobGiver<IdleJobGiverDef_TakeNap>
{
    public override float GetWeight(Pawn pawn, Trait traitIndustriousness)
    {
        if (modSettings.noNapping)
        {
            return 0;
        }

        if (pawn.story.traits.HasTrait(TraitDefOf.Undergrounder) ||
            pawn.story.traits.HasTrait(MoreTraitDefOf.QuickSleeper))
        {
            return base.GetWeight(pawn, traitIndustriousness) * 1.5f;
        }

        return base.GetWeight(pawn, traitIndustriousness);
    }

    public override Job TryGiveJob(Pawn pawn)
    {
        var rest = pawn.needs.rest;
        if (rest == null || rest.CurLevelPercentage > def.restRequirement)
        {
            return null;
        }

        var num = GenLocalDate.DayPercent(pawn.Map);
        if (num is < 0.42f or > 0.7f)
        {
            return null;
        }

        var building_Bed = RestUtility.FindBedFor(pawn);
        if (building_Bed != null)
        {
            return new Job(IdleJobDefOf.IdleJob_TakeNap, building_Bed)
            {
                locomotionUrgency = modSettings.wanderMovePolicy
            };
        }

        return null;
    }
}
