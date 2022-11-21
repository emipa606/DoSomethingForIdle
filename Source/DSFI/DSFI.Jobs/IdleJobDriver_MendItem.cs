using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.Jobs;

public class IdleJobDriver_MendItem : IdleJobDriver
{
    private const float mendingWorks = 1800f;
    private float workDone;

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
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        var toil = new Toil().FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
        toil.defaultCompleteMode = ToilCompleteMode.Never;
        toil.socialMode = RandomSocialMode.SuperActive;
        toil.WithProgressBar(TargetIndex.A, () => workDone / 1800f);
        toil.PlaySustainerOrSound(job.targetA.Thing.def.recipeMaker.soundWorking);
        toil.tickAction = delegate
        {
            workDone += 1f;
            if (!(workDone >= 1800f))
            {
                return;
            }

            var thing = job.targetA.Thing;
            var max = Math.Max(1, pawn.skills.GetSkill(SkillDefOf.Crafting).Level) / 20f * 0.1f;
            var num = Rand.Range(0.0050000004f, max);
            thing.HitPoints = Math.Min(thing.MaxHitPoints, thing.HitPoints + (int)(thing.MaxHitPoints * num));
            ReadyForNextToil();
        };
        yield return toil;
    }
}
