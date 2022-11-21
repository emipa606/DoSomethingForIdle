using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.Jobs;

public class IdleJobDriver_PracticeMelee : IdleJobDriver
{
    private FieldInfo fieldJitterer;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var p = pawn;
        var target = job.GetTarget(TargetIndex.B);
        var localJob = job;
        return p.Reserve(target, localJob, 1, -1, null, errorOnFailed);
    }

    public override IEnumerable<Toil> MakeNewToils()
    {
        if (fieldJitterer == null)
        {
            fieldJitterer = typeof(Pawn_DrawTracker).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .First(x => x.Name == "jitterer");
        }

        var jitterer = fieldJitterer.GetValue(pawn.Drawer) as JitterHandler;
        yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
        var practice = Toils_General.Wait(2000);
        practice.socialMode = RandomSocialMode.Normal;
        practice.FailOn(() => TargetB.Cell.IsForbidden(pawn));
        practice.tickAction = delegate
        {
            if (!pawn.IsHashIntervalTick(200))
            {
                return;
            }

            pawn.skills.Learn(SkillDefOf.Melee, 5f);
            if (jitterer == null)
            {
                return;
            }

            jitterer.AddOffset(Rand.Range(0.25f, 0.75f), pawn.Rotation.AsAngle);
            if (!(Rand.Value > 0.7f))
            {
                return;
            }

            practice.handlingFacing = true;
            pawn.Rotation = Rot4.Random;
        };
        yield return practice;
    }

    public override string GetReport()
    {
        return job.def.reportString.Replace("TargetA", TargetA.Thing.def.LabelCap);
    }
}
