using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.Jobs;

public class IdleJobDriver_Graffiti : IdleJobDriver
{
    private float workLeft;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.GetTarget(TargetIndex.A), job, 1, -1, null, errorOnFailed);
    }

    public override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnForbidden(TargetIndex.A);
        yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch);
        var doWork = new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Never
        };
        doWork.FailOn(() => !JoyUtility.EnjoyableOutsideNow(pawn));
        doWork.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
        doWork.PlaySustainerOrSound(SoundDefOf.Interact_CleanFilth);
        doWork.initAction = delegate { workLeft = 600f; };
        doWork.tickAction = delegate
        {
            if (pawn.IsHashIntervalTick(50))
            {
                pawn.skills.Learn(SkillDefOf.Artistic, 0.5f);
            }

            workLeft -= doWork.actor.GetStatValue(StatDefOf.WorkSpeedGlobal);
            if (!(workLeft <= 0f))
            {
                return;
            }

            GenSpawn.Spawn(ThingMaker.MakeThing(DSFIThingDefOf.DSFI_Scribbling), TargetLocA, Map).Rotation =
                Rot4.Random;
            ReadyForNextToil();
        };
        yield return doWork;
    }
}
