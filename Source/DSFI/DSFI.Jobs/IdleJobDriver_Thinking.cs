using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.Jobs;

public class IdleJobDriver_Thinking : IdleJobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.GetTarget(TargetIndex.B), job, 1, -1, null, errorOnFailed);
    }

    public override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
        pawn.Rotation = Rot4.Random;
        var waiting = Toils_General.Wait(job.def.joyDuration);
        waiting.socialMode = RandomSocialMode.Quiet;
        waiting.FailOn(() =>
            !MeditationUtility.CanMeditateNow(pawn) ||
            !MeditationUtility.SafeEnvironmentalConditions(pawn, TargetB.Cell, Map));
        waiting.tickAction = delegate
        {
            pawn.GainComfortFromCellIfPossible();
            if (pawn.needs.joy != null)
            {
                JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.None, 0.2f);
            }

            waiting.handlingFacing = true;
            if (pawn.IsHashIntervalTick(400))
            {
                MakeThinkingBubble(pawn, TargetA.Pawn);
            }
        };
        yield return waiting;
    }

    public RTMoteBubble MakeThinkingBubble(Pawn pawn, Pawn target)
    {
        var obj = (RTMoteBubble)ThingMaker.MakeThing(MoteDefOf.DSFI_Mote_Thought);
        obj.SetupMoteBubble(job.targetA.Pawn);
        obj.Attach(pawn);
        GenSpawn.Spawn(obj, pawn.Position, pawn.Map);
        return obj;
    }
}
