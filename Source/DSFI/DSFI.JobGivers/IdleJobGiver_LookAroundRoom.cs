using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.JobGivers;

public class IdleJobGiver_LookAroundRoom : IdleJobGiver<IdleJobGiverDef_LookAroundRoom>
{
    private readonly HashSet<Pawn> roomOwners = new HashSet<Pawn>();

    public override float GetWeight(Pawn pawn, Trait traitIndustriousness)
    {
        return pawn.story.traits.HasTrait(TraitDefOf.Psychopath) ? 0f : base.GetWeight(pawn, traitIndustriousness);
    }

    public override Job TryGiveJob(Pawn pawn)
    {
        var map = pawn.Map;
        roomOwners.Clear();
        foreach (var item in pawn.relations.RelatedPawns.Where(x => x.Map == pawn.Map))
        {
            if (pawn.relations.OpinionOf(item) < def.requiredOpinion ||
                item.jobs.curDriver is { asleep: true } || item.ownership == null)
            {
                continue;
            }

            var ownedRoom = item.ownership.OwnedRoom;
            if (ownedRoom != null && ownedRoom.Role == RoomRoleDefOf.Bedroom)
            {
                roomOwners.Add(item);
            }
        }

        if (roomOwners.Count <= 0)
        {
            return null;
        }

        var pawn2 = roomOwners.RandomElement();
        if (pawn2.ownership.OwnedRoom.Cells
            .Where(x => x.Standable(map) && !x.IsForbidden(pawn) &&
                        pawn.CanReserveAndReach(x, PathEndMode.OnCell, Danger.None)).TryRandomElement(out var result))
        {
            return new Job(IdleJobDefOf.IdleJob_LookAroundRoom, pawn2, result)
            {
                locomotionUrgency = modSettings.wanderMovePolicy
            };
        }

        return null;
    }
}
