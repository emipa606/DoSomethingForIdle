using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.Jobs;

public class IdleJobDriver_WatchDoing : IdleJobDriver
{
    private const float xp = 10f;

    private const int watchingTicksMin = 1000;

    private const int watchingTicksMax = 1500;

    private readonly List<Pair<SkillDef, float>> learnings = new List<Pair<SkillDef, float>>();

    private float ticks;

    private float watchingTicks;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    public override IEnumerable<Toil> MakeNewToils()
    {
        var targetPawn = job.targetA.Thing as Pawn;
        watchingTicks = Rand.Range(1000, 1500);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        learnings.Clear();
        if (targetPawn?.CurJob != null)
        {
            if (targetPawn.CurJob.RecipeDef is { requiredGiverWorkType: { } })
            {
                var relevantSkills = targetPawn.CurJob.RecipeDef.requiredGiverWorkType.relevantSkills;
                foreach (var item in relevantSkills)
                {
                    learnings.Add(new Pair<SkillDef, float>(item,
                        targetPawn.skills.GetSkill(item).Level / 20f * 10f / relevantSkills.Count));
                }
            }
            else if (targetPawn.CurJob.def.defName == "Research")
            {
                learnings.Add(new Pair<SkillDef, float>(SkillDefOf.Intellectual,
                    targetPawn.skills.GetSkill(SkillDefOf.Intellectual).Level / 20f * 10f));
            }
        }

        var toil = new Toil().FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
        toil.defaultCompleteMode = ToilCompleteMode.Never;
        toil.socialMode = RandomSocialMode.SuperActive;
        toil.tickAction = delegate
        {
            ticks += 1f;
            if (pawn.IsHashIntervalTick(20))
            {
                foreach (var learning in learnings)
                {
                    pawn.skills.Learn(learning.First, learning.Second);
                }

                return;
            }

            if (ticks > watchingTicks)
            {
                ReadyForNextToil();
            }
        };
        yield return toil;
    }
}
