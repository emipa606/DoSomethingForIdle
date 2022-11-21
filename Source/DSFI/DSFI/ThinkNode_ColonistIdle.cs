using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace DSFI;

public class ThinkNode_ColonistIdle : ThinkNode
{
    private readonly JobGiver_WanderColony wanderJobGiver = new JobGiver_WanderColony();
    private List<IdleJobGiverBase> cachedJobGivers;

    public ThinkNode_ColonistIdle()
    {
        Log.Message("[DSFI] Initialized");
    }

    private List<IdleJobGiverBase> JobGivers
    {
        get
        {
            if (cachedJobGivers != null)
            {
                return cachedJobGivers;
            }

            cachedJobGivers = new List<IdleJobGiverBase>();
            foreach (var item in DefDatabase<IdleJobGiverDef>.AllDefs.ToList())
            {
                if (Activator.CreateInstance(item.giverClass) is IdleJobGiverBase idleJobGiverBase)
                {
                    idleJobGiverBase.LoadDef(item);
                    cachedJobGivers.Add(idleJobGiverBase);
                }
                else
                {
                    Log.Warning($"Failed to create IdleJobGiver with named '{item.giverClass}'. so ignoring that.");
                }
            }

            return cachedJobGivers;
        }
    }

    public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
    {
        if (pawn.GetLord() != null)
        {
            return wanderJobGiver.TryIssueJobPackage(pawn, default);
        }

        if (!pawn.Awake())
        {
            return wanderJobGiver.TryIssueJobPackage(pawn, default);
        }

        if ((pawn.needs?.food?.Starving).GetValueOrDefault())
        {
            return wanderJobGiver.TryIssueJobPackage(pawn, default);
        }

        if (pawn.story == null)
        {
            return wanderJobGiver.TryIssueJobPackage(pawn, default);
        }

        JobGivers.TryRandomElementByWeight(
            x => x.GetWeight(pawn, pawn.story.traits.GetTrait(TraitDefOf.Industriousness)), out var result);
        if (result == null)
        {
            return wanderJobGiver.TryIssueJobPackage(pawn, default);
        }

        var job = result.TryGiveJob(pawn);
        return job != null ? new ThinkResult(job, this) : wanderJobGiver.TryIssueJobPackage(pawn, default);
    }
}
