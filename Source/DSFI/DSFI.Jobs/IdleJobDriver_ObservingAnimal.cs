using System.Collections.Generic;
using DSFI.Toils;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DSFI.Jobs;

public class IdleJobDriver_ObservingAnimal : IdleJobDriver
{
    private const float moveDistance = 2f;

    private const float lookDistance = 6f;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    public override IEnumerable<Toil> MakeNewToils()
    {
        if (TargetA.Thing is not Pawn target)
        {
            yield break;
        }

        Mathf.Pow(5f, target.RaceProps.wildness);
        this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
        yield return DSFIToils_Moving.GotoNearTarget(TargetIndex.A, Danger.None, 2f, 6f);
        var toil = Toils_General.Wait(1500).FailOnDestroyedOrNull(TargetIndex.A);
        toil.socialMode = RandomSocialMode.SuperActive;
        toil.handlingFacing = true;
        toil.FailOn(() => !JoyUtility.EnjoyableOutsideNow(pawn) || pawn.Position.IsForbidden(pawn));
        toil.tickAction = delegate
        {
            if (!pawn.IsHashIntervalTick(20))
            {
                return;
            }

            pawn.skills.Learn(SkillDefOf.Animals, 5f);
            pawn.rotationTracker.FaceTarget(target);
            if (!pawn.CanSee(target) || !pawn.Position.InHorDistOf(target.Position, 6f))
            {
                ReadyForNextToil();
            }
        };
        yield return toil;
    }
}
