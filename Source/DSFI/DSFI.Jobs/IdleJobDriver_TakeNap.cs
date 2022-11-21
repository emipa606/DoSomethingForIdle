using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.Jobs;

public class IdleJobDriver_TakeNap : IdleJobDriver
{
    private const float napRestRate = 0.5f;
    private float goalNeedRest;

    public Building_Bed Bed => (Building_Bed)job.GetTarget(TargetIndex.A).Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(Bed, job, Bed.SleepingSlotsCount, 0, null, errorOnFailed);
    }

    public override bool CanBeginNowWhileLyingDown()
    {
        return JobInBedUtility.InBedOrRestSpotNow(pawn, job.GetTarget(TargetIndex.A));
    }

    public override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Bed.ClaimBedIfNonMedical(TargetIndex.A);
        yield return Toils_Bed.GotoBed(TargetIndex.A);
        goalNeedRest = 0.5f + (0.5f * pawn.needs.rest.CurLevelPercentage);
        var toil = new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Never
        };
        toil.FailOnBedNoLongerUsable(TargetIndex.A);
        toil.socialMode = RandomSocialMode.Off;
        toil.initAction = delegate
        {
            pawn.pather.StopDead();
            if (!Bed.OccupiedRect().Contains(pawn.Position))
            {
                Log.Error($"Can't start LayDown toil because pawn is not in the bed. pawn={pawn}");
                pawn.jobs.EndCurrentJob(JobCondition.Errored);
            }
            else
            {
                pawn.jobs.posture = PawnPosture.LayingOnGroundFaceUp;
                asleep = false;
                if (pawn.mindState.applyBedThoughtsTick == 0)
                {
                    pawn.mindState.applyBedThoughtsTick = Find.TickManager.TicksGame + Rand.Range(2500, 10000);
                    pawn.mindState.applyBedThoughtsOnLeave = false;
                }

                if (pawn.ownership != null && pawn.CurrentBed() != pawn.ownership.OwnedBed)
                {
                    ThoughtUtility.RemovePositiveBedroomThoughts(pawn);
                }
            }
        };
        toil.tickAction = delegate
        {
            pawn.GainComfortFromCellIfPossible();
            if (!asleep)
            {
                if (pawn.needs.rest != null && pawn.needs.rest.CurLevel < RestUtility.FallAsleepMaxLevel(pawn))
                {
                    asleep = true;
                }
            }
            else if (pawn.needs.rest == null || pawn.needs.rest.CurLevelPercentage >= goalNeedRest)
            {
                asleep = false;
            }

            if (asleep && pawn.needs.rest != null)
            {
                var restEffectiveness =
                    Bed == null || !Bed.def.statBases.StatListContains(StatDefOf.BedRestEffectiveness)
                        ? 0.8f
                        : Bed.GetStatValue(StatDefOf.BedRestEffectiveness);
                pawn.needs.rest.TickResting(restEffectiveness);
            }

            if (pawn.mindState.applyBedThoughtsTick != 0 &&
                pawn.mindState.applyBedThoughtsTick <= Find.TickManager.TicksGame)
            {
                ApplyBedThoughts(pawn);
                pawn.mindState.applyBedThoughtsTick += 60000;
                pawn.mindState.applyBedThoughtsOnLeave = true;
            }

            if (pawn.IsHashIntervalTick(100) && !pawn.Position.Fogged(pawn.Map))
            {
                if (asleep)
                {
                    FleckMaker.ThrowMetaIcon(pawn.Position, pawn.Map, FleckDefOf.SleepZ);
                }

                if (pawn.health.hediffSet.GetNaturallyHealingInjuredParts().Any())
                {
                    FleckMaker.ThrowMetaIcon(pawn.Position, pawn.Map, FleckDefOf.HealingCross);
                }
            }

            if (pawn.ownership != null && Bed is { Medical: false } && !Bed.OwnersForReading.Contains(pawn))
            {
                if (pawn.Downed)
                {
                    pawn.Position = CellFinder.RandomClosewalkCellNear(pawn.Position, pawn.Map, 1);
                }

                pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
            }
            else if (pawn.IsHashIntervalTick(211))
            {
                pawn.jobs.CheckForJobOverride();
            }
        };
        toil.AddFinishAction(delegate
        {
            if (pawn.mindState.applyBedThoughtsOnLeave)
            {
                ApplyBedThoughts(pawn);
            }

            asleep = false;
        });
        yield return toil;
    }

    private static void ApplyBedThoughts(Pawn actor)
    {
        if (actor.needs.mood == null)
        {
            return;
        }

        var memories = actor.needs.mood.thoughts.memories;
        var building_Bed = actor.CurrentBed();
        memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInBedroom);
        memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInBarracks);
        memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptOutside);
        memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptOnGround);
        memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInCold);
        memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInHeat);
        if (actor.GetRoom(RegionType.Set_Passable).PsychologicallyOutdoors)
        {
            memories.TryGainMemory(ThoughtDefOf.SleptOutside);
        }

        if (building_Bed == null || building_Bed.CostListAdjusted().Count == 0)
        {
            memories.TryGainMemory(ThoughtDefOf.SleptOnGround);
        }

        if (actor.AmbientTemperature < actor.def.GetStatValueAbstract(StatDefOf.ComfyTemperatureMin))
        {
            memories.TryGainMemory(ThoughtDefOf.SleptInCold);
        }

        if (actor.AmbientTemperature > actor.def.GetStatValueAbstract(StatDefOf.ComfyTemperatureMax))
        {
            memories.TryGainMemory(ThoughtDefOf.SleptInHeat);
        }

        if (building_Bed == null || building_Bed != actor.ownership.OwnedBed || building_Bed.ForPrisoners ||
            actor.story.traits.HasTrait(TraitDefOf.Ascetic))
        {
            return;
        }

        ThoughtDef thoughtDef = null;
        if (building_Bed.GetRoom(RegionType.Set_Passable).Role == RoomRoleDefOf.Bedroom)
        {
            thoughtDef = ThoughtDefOf.SleptInBedroom;
        }
        else if (building_Bed.GetRoom(RegionType.Set_Passable).Role == RoomRoleDefOf.Barracks)
        {
            thoughtDef = ThoughtDefOf.SleptInBarracks;
        }

        if (thoughtDef == null)
        {
            return;
        }

        var scoreStageIndex = RoomStatDefOf.Impressiveness.GetScoreStageIndex(building_Bed
            .GetRoom(RegionType.Set_Passable).GetStat(RoomStatDefOf.Impressiveness));
        if (thoughtDef.stages[scoreStageIndex] != null)
        {
            memories.TryGainMemory(ThoughtMaker.MakeThought(thoughtDef, scoreStageIndex));
        }
    }
}
