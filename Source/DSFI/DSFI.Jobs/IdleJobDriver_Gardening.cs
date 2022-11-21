using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.Jobs;

public class IdleJobDriver_Gardening : IdleJobDriver
{
    private const int workTotal = 500;

    private readonly List<IntVec3> listChecked = new List<IntVec3>();

    private readonly int plantWant = Rand.RangeInclusive(3, 5);
    private int plantCount;

    private int workDone;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    public override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch).FailOnForbidden(TargetIndex.A);
        var labelLoopStart = Toils_General.Label();
        var labelLoopEnd = Toils_General.Label();
        yield return labelLoopStart;
        yield return Toils_General.Do(delegate
        {
            if (TargetA.Cell.GetZone(Map) is not Zone_Growing zone_Growing || !zone_Growing.AllContainedThings.Where(
                        x =>
                            x is Plant plant && !plant.HostileTo(pawn) && !plant.IsForbidden(pawn) &&
                            !plant.HarvestableNow &&
                            pawn.CanReserveAndReach(x, PathEndMode.Touch, Danger.None) &&
                            !listChecked.Contains(plant.Position))
                    .TryRandomElement(out var result))
            {
                job.SetTarget(TargetIndex.A, LocalTargetInfo.Invalid);
            }
            else
            {
                Map.reservationManager.Reserve(pawn, job, result);
                job.SetTarget(TargetIndex.A, result);
            }
        });
        yield return Toils_Jump.JumpIf(labelLoopEnd, () => !job.targetA.IsValid || plantCount >= plantWant);
        yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch).FailOnDestroyedOrNull(TargetIndex.A);
        var toil = new Toil().FailOnDestroyedNullOrForbidden(TargetIndex.A);
        toil.defaultCompleteMode = ToilCompleteMode.Never;
        toil.socialMode = RandomSocialMode.SuperActive;
        toil.WithProgressBar(TargetIndex.A, () => workDone / 500f);
        toil.PlaySustainerOrSound(SoundDefOf.Interact_Sow);
        toil.WithEffect(() => EffecterDefOf.Harvest_Plant, TargetIndex.A);
        toil.initAction = delegate
        {
            workDone = 0;
            reportState = Rand.Bool ? ReportStringState.A : ReportStringState.B;
        };
        toil.tickAction = delegate
        {
            workDone++;
            pawn.skills.Learn(SkillDefOf.Plants, 0.01f);
            if (workDone >= 500)
            {
                ReadyForNextToil();
            }
        };
        toil.AddFinishAction(delegate
        {
            plantCount++;
            Map.reservationManager.Release(job.targetA, pawn, job);
            if (job.targetA.Thing is Plant plant)
            {
                plant.Growth += plant.GrowthRate * 2000f / (60000f * plant.def.plant.growDays);
            }

            listChecked.Add(job.targetA.Cell);
            reportState = ReportStringState.None;
        });
        yield return toil;
        yield return Toils_Jump.Jump(labelLoopStart);
        yield return labelLoopEnd;
    }

    public override string GetReport()
    {
        if (reportState != ReportStringState.None)
        {
            return base.GetReport();
        }

        var zone = TargetA.Cell.GetZone(Map);
        return zone != null ? job.def.reportString.Replace("TargetA", zone.label) : "";
    }
}
