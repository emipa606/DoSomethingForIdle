using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI;

public static class AIUtility
{
    public static IntVec3 FindRandomSpotOutsideColony(Pawn pawn, float distance = -1f, bool canReach = false,
        bool canReserve = false)
    {
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

                if (distance > 0f && x.DistanceToSquared(pawn.Position) > distance * distance)
                {
                    return false;
                }

                if (canReach && !pawn.CanReach(x, PathEndMode.OnCell, Danger.None))
                {
                    return false;
                }

                return !canReserve || pawn.CanReserve(x);
            });
        return result;
    }

    public static IEnumerable<IntVec3> FindAroundSpotFromTarget(Pawn pawn, IntVec3 target, float maxRadius,
        float minRadius, bool canSee = true, bool canReach = false, bool canReserve = false)
    {
        return GenRadial.RadialCellsAround(target, maxRadius, true).Where(delegate(IntVec3 x)
        {
            if (!x.InBounds(pawn.Map) || !x.Walkable(pawn.Map))
            {
                return false;
            }

            if (x.IsForbidden(pawn))
            {
                return false;
            }

            if (x.DistanceToSquared(target) <= minRadius * minRadius)
            {
                return false;
            }

            if (canReach && !pawn.CanReach(x, PathEndMode.OnCell, Danger.None))
            {
                return false;
            }

            if (canReserve && !pawn.CanReserve(x))
            {
                return false;
            }

            return !(canSee & !GenSight.LineOfSight(target, x, pawn.Map));
        });
    }

    public static IntVec3 FindRandomSpotInZone(Pawn pawn, Zone zone, bool canReach = false, bool canReserve = false)
    {
        return zone.cells.Where(delegate(IntVec3 x)
        {
            if (!x.InBounds(pawn.Map) || !x.Walkable(pawn.Map))
            {
                return false;
            }

            if (x.IsForbidden(pawn))
            {
                return false;
            }

            if (canReach && !pawn.CanReach(x, PathEndMode.OnCell, Danger.None))
            {
                return false;
            }

            return !canReserve || pawn.CanReserve(x);
        }).TryRandomElement(out var result)
            ? result
            : IntVec3.Invalid;
    }
}
