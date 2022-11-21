using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.JobGivers;

internal class IdleJobGiver_Wander : IdleJobGiver<IdleJobGiverDef>
{
    public override float GetWeight(Pawn pawn, Trait traitIndustriousness)
    {
        return base.GetWeight(pawn, traitIndustriousness) * modSettings.wanderMultiplier;
    }

    public override Job TryGiveJob(Pawn pawn)
    {
        return null;
    }
}
