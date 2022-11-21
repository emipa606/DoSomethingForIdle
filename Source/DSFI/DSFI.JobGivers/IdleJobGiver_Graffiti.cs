using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.JobGivers;

public class IdleJobGiver_Graffiti : IdleJobGiver<IdleJobGiverDef>
{
    public override float GetWeight(Pawn pawn, Trait traitIndustriousness)
    {
        return pawn.skills.GetSkill(SkillDefOf.Artistic).passion == Passion.None
            ? 0f
            : base.GetWeight(pawn, traitIndustriousness);
    }

    public override Job TryGiveJob(Pawn pawn)
    {
        if (!JoyUtility.EnjoyableOutsideNow(pawn.Map))
        {
            return null;
        }

        RCellFinder.TryFindRandomSpotJustOutsideColony(pawn.Position, pawn.Map, pawn, out var result,
            delegate(IntVec3 x)
            {
                if (!x.InBounds(pawn.Map) || !x.Walkable(pawn.Map))
                {
                    return false;
                }

                if (x.IsForbidden(pawn))
                {
                    return false;
                }

                if (x.GetTerrain(pawn.Map).fertility <= 0f)
                {
                    return false;
                }

                var num = x.DistanceToSquared(pawn.Position);
                return num is >= 9 and <= 225 && pawn.CanReserveAndReach(x, PathEndMode.Touch, Danger.None);
            });
        if (result.IsValid)
        {
            return new Job(IdleJobDefOf.IdleJob_Graffiti, result)
            {
                locomotionUrgency = modSettings.wanderMovePolicy
            };
        }

        return null;
    }
}
