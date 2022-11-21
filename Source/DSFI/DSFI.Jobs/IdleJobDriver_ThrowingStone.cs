using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.Jobs;

public class IdleJobDriver_ThrowingStone : IdleJobDriver
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
        yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
        var toil = Toils_General.Wait(2000, TargetIndex.A);
        toil.socialMode = RandomSocialMode.Normal;
        toil.FailOn(() => !JoyUtility.EnjoyableOutsideNow(pawn) || TargetB.Cell.IsForbidden(pawn));
        toil.handlingFacing = true;
        toil.initAction = delegate { pawn.rotationTracker.FaceCell(TargetLocA); };
        toil.tickAction = delegate
        {
            if (!pawn.IsHashIntervalTick(400))
            {
                return;
            }

            pawn.skills.Learn(SkillDefOf.Shooting, 5f);
            FleckMaker.ThrowStone(pawn, TargetLocA);
        };
        yield return toil;
    }
}
