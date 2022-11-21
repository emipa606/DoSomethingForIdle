using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.JobGivers;

public class IdleJobGiver_WatchDoing : IdleJobGiver<IdleJobGiverDef_WatchDoing>
{
    public override float GetWeight(Pawn pawn, Trait traitIndustriousness)
    {
        if (pawn.story.traits.HasTrait(TraitDefOf.Bloodlust))
        {
            return 0f;
        }

        if (pawn.story.traits.HasTrait(TraitDefOf.Kind))
        {
            return base.GetWeight(pawn, traitIndustriousness) * 1.8f;
        }

        return base.GetWeight(pawn, traitIndustriousness);
    }

    public override Job TryGiveJob(Pawn pawn)
    {
        if (GenClosest.ClosestThingReachable(pawn.Position, pawn.Map,
                ThingRequest.ForGroup(ThingRequestGroup.BuildingFrame), PathEndMode.Touch,
                TraverseParms.For(pawn, Danger.None), def.searchDistance, delegate(Thing x)
                {
                    var pawn3 = x as Pawn;
                    return pawn3?.CurJob != null && !(pawn3.CurJobDef.driverClass != def.targetJobDriver);
                }) is Pawn pawn2)
        {
            return new Job(def.jobDef, pawn2)
            {
                locomotionUrgency = modSettings.wanderMovePolicy
            };
        }

        return null;
    }
}
