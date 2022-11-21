using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.JobGivers;

public class IdleJobGiver_MessingAround : IdleJobGiver<IdleJobGiverDef>
{
    private HashSet<Pair<Room, Building>> rooms = new HashSet<Pair<Room, Building>>();

    public override Job TryGiveJob(Pawn pawn)
    {
        foreach (var item in pawn.Map.listerBuildings.allBuildingsColonist.Where(x =>
                     x.def.building is { isSittable: true } && x.Position.InHorDistOf(pawn.Position, 20f) &&
                     pawn.CanReserve(x)))
        {
            var room = item.Position.GetRoom(pawn.Map);
            if (room != null && (room.Role == RoomRoleDefOf.DiningRoom || room.Role == RoomRoleDefOf.RecRoom))
            {
                return new Job(IdleJobDefOf.IdleJob_MessingAround, item)
                {
                    locomotionUrgency = modSettings.wanderMovePolicy
                };
            }
        }

        return null;
    }
}
