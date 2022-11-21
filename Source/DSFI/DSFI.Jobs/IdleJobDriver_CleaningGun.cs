using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.Jobs;

public class IdleJobDriver_CleaningGun : IdleJobDriver
{
    private const float mendingWorks = 1800f;
    private float workDone;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    public override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
        var toil = new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Never,
            socialMode = RandomSocialMode.SuperActive
        };
        toil.WithProgressBar(TargetIndex.A, () => workDone / 1800f);
        toil.PlaySustainerOrSound(SoundDefOf.Interact_CleanFilth);
        toil.FailOn(() => pawn.Position.IsForbidden(pawn));
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
