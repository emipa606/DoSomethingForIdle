using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.Toils;

public static class DSFIToils_Moving
{
    public static Toil GotoNearTarget(TargetIndex target, Danger danger, float moveDistance, float lookDistance)
    {
        var toil = new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Never
        };
        toil.initAction = delegate
        {
            var actor2 = toil.actor;
            var pawn2 = actor2.CurJob.GetTarget(target).Thing as Pawn;
            if (pawn2 != null && actor2.Position.InHorDistOf(pawn2.Position, lookDistance))
            {
                actor2.jobs.curDriver.ReadyForNextToil();
            }
            else
            {
                actor2.pather.StartPath(pawn2, PathEndMode.OnCell);
            }
        };
        toil.tickAction = delegate
        {
            var actor = toil.actor;
            var pawn = actor.CurJob.GetTarget(target).Thing as Pawn;
            var map = actor.Map;
            if (pawn != null && actor.Position.InHorDistOf(pawn.Position, lookDistance))
            {
                actor.pather.StopDead();
                actor.jobs.curDriver.ReadyForNextToil();
            }
            else if (!actor.pather.Moving)
            {
                var intVec = IntVec3.Invalid;
                foreach (var item in GenRadial.RadialPatternInRadius(moveDistance))
                {
                    if (pawn == null)
                    {
                        continue;
                    }

                    var intVec2 = pawn.Position + item;
                    if (intVec2.InBounds(map) && intVec2.Walkable(map) && intVec2 != actor.Position &&
                        !intVec2.IsForbidden(actor) && actor.CanReach(intVec2, PathEndMode.OnCell, danger) &&
                        actor.CanSee(pawn) && (!intVec.IsValid || actor.Position.DistanceToSquared(intVec2) >=
                            actor.Position.DistanceToSquared(intVec)))
                    {
                        intVec = intVec2;
                    }
                }

                if (intVec.IsValid)
                {
                    actor.pather.StartPath(intVec, PathEndMode.OnCell);
                }
                else
                {
                    actor.jobs.curDriver.EndJobWith(JobCondition.Incompletable);
                }
            }
        };
        return toil;
    }

    public static Toil GotoNearTargetAndWait(TargetIndex target, Danger danger, float moveDistance, float lookDistance)
    {
        var toil = new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Never
        };
        toil.initAction = delegate
        {
            var actor2 = toil.actor;
            var pawn2 = actor2.CurJob.GetTarget(target).Thing as Pawn;
            if (pawn2 != null && actor2.Position.InHorDistOf(pawn2.Position, lookDistance))
            {
                actor2.jobs.curDriver.ReadyForNextToil();
            }
            else
            {
                actor2.pather.StartPath(pawn2, PathEndMode.OnCell);
            }
        };
        toil.tickAction = delegate
        {
            var actor = toil.actor;
            var pawn = actor.CurJob.GetTarget(target).Thing as Pawn;
            var map = actor.Map;
            if (pawn != null && actor.Position.InHorDistOf(pawn.Position, moveDistance))
            {
                actor.pather.StopDead();
                actor.jobs.curDriver.ReadyForNextToil();
            }
            else if (!actor.pather.Moving)
            {
                var intVec = IntVec3.Invalid;
                foreach (var item in GenRadial.RadialPatternInRadius(moveDistance))
                {
                    if (pawn == null)
                    {
                        continue;
                    }

                    var intVec2 = pawn.Position + item;
                    if (intVec2.InBounds(map) && intVec2.Walkable(map) && intVec2 != actor.Position &&
                        !intVec2.IsForbidden(actor) && actor.CanReach(intVec2, PathEndMode.OnCell, danger) &&
                        actor.CanSee(pawn) && (!intVec.IsValid || actor.Position.DistanceToSquared(intVec2) >=
                            actor.Position.DistanceToSquared(intVec)))
                    {
                        intVec = intVec2;
                    }
                }

                if (intVec.IsValid)
                {
                    actor.pather.StartPath(intVec, PathEndMode.OnCell);
                }
                else
                {
                    actor.jobs.curDriver.EndJobWith(JobCondition.Incompletable);
                }
            }
        };
        return toil;
    }
}
