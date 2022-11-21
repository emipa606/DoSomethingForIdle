using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.Jobs;

public class IdleJobDriver_LookAroundRoom : IdleJobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var p = pawn;
        var target = job.GetTarget(TargetIndex.B);
        var localJob = job;
        return p.Reserve(target, localJob, 1, -1, null, errorOnFailed);
    }

    public override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
        yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
        var looking = Toils_General.Wait(1500);
        looking.FailOn(() => TargetB.Cell.IsForbidden(pawn));
        looking.tickAction = delegate
        {
            looking.handlingFacing = true;
            if (!pawn.IsHashIntervalTick(250))
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
        looking.AddFinishAction(delegate
        {
            var recipient = TargetA.Thing as Pawn;
            pawn.interactions.TryInteractWith(recipient, InteractionDefOf.Chitchat);
        });
        yield return looking;
    }
}
