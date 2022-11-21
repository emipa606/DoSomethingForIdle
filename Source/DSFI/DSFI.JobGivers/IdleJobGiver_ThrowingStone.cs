using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.JobGivers;

public class IdleJobGiver_ThrowingStone : IdleJobGiver<IdleJobGiverDef>
{
    public override float GetWeight(Pawn pawn, Trait traitIndustriousness)
    {
        var num = 1f;
        if (pawn.story.traits.HasTrait(TraitDefOf.ShootingAccuracy))
        {
            num *= 1.5f;
        }

        return base.GetWeight(pawn, traitIndustriousness) * num;
    }

    public override Job TryGiveJob(Pawn pawn)
    {
        if (!JoyUtility.EnjoyableOutsideNow(pawn.Map))
        {
            return null;
        }

        var intVec = AIUtility.FindRandomSpotOutsideColony(pawn, def.searchDistance);
        if (!intVec.IsValid)
        {
            return null;
        }

        if (!AIUtility.FindAroundSpotFromTarget(pawn, intVec, 4f, 3f, true, true, true)
                .TryRandomElement(out var result))
        {
            return null;
        }

        return new Job(IdleJobDefOf.IdleJob_ThrowingStone, intVec, result)
        {
            locomotionUrgency = modSettings.wanderMovePolicy
        };
    }
}
