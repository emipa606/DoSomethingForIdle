using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.JobGivers;

public class IdleJobGiver_Thinking : IdleJobGiver<IdleJobGiverDef_Thinking>
{
    private HashSet<Pair<Room, Building>> rooms = new HashSet<Pair<Room, Building>>();

    public override Job TryGiveJob(Pawn pawn)
    {
        if (pawn.ownership?.OwnedRoom == null)
        {
            return null;
        }

        var source = pawn.ownership.OwnedRoom.Cells.Where(c =>
            c.Standable(pawn.Map) && !c.IsForbidden(pawn) &&
            pawn.CanReserveAndReach(c, PathEndMode.OnCell, Danger.None));
        if (!source.Any())
        {
            return null;
        }

        LocalTargetInfo invalid = source.RandomElement();
        if (!(from x in pawn.needs.mood.thoughts.memories.Memories
                where x.otherPawn != null && pawn.relations.OpinionOf(x.otherPawn) >= def.requiredOpinion
                select x.otherPawn).TryRandomElementByWeight(other => pawn.relations.OpinionOf(other), out var result))
        {
            return null;
        }

        return new Job(IdleJobDefOf.IdleJob_Thinking, result, invalid)
        {
            locomotionUrgency = modSettings.wanderMovePolicy
        };
    }
}
