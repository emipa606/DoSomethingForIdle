using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.Jobs;

public class IdleJobDriver_MessingAround : IdleJobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var p = pawn;
        var target = job.GetTarget(TargetIndex.A);
        var localJob = job;
        return p.Reserve(target, localJob, 1, -1, null, errorOnFailed);
    }

    public override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
        _ = TargetA.Thing;
        yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
        pawn.Rotation = Rot4.Random;
        var waiting = Toils_General.Wait(2000);
        waiting.socialMode = RandomSocialMode.SuperActive;
        waiting.tickAction = delegate
        {
            waiting.handlingFacing = true;
            if (!pawn.IsHashIntervalTick(150))
            {
                return;
            }

            if (pawn.Rotation == Rot4.North)
            {
                pawn.Rotation = Rot4.East;
            }
            else if (pawn.Rotation == Rot4.East)
            {
                pawn.Rotation = Rot4.South;
            }
            else if (pawn.Rotation == Rot4.South)
            {
                pawn.Rotation = Rot4.West;
            }
            else
            {
                pawn.Rotation = Rot4.North;
            }
        };
        yield return waiting;
    }
}
