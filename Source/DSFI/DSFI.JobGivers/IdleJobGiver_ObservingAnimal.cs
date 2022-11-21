using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.JobGivers;

public class IdleJobGiver_ObservingAnimal : IdleJobGiver<IdleJobGiverDef>
{
    private static readonly HashSet<Pawn> pawns = new HashSet<Pawn>();

    public override Job TryGiveJob(Pawn pawn)
    {
        if (!JoyUtility.EnjoyableOutsideNow(pawn.Map))
        {
            return null;
        }

        pawns.Clear();
        foreach (var item in GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, def.searchDistance, true))
        {
            if (item is Pawn pawn2 && pawn2.RaceProps.Animal && !pawn2.HostileTo(pawn))
            {
                pawns.Add(pawn2);
            }
        }

        if (!pawns.Any())
        {
            return null;
        }

        var pawn3 = pawns.RandomElement();
        return new Job(IdleJobDefOf.IdleJob_ObservingAnimal, pawn3)
        {
            locomotionUrgency = modSettings.wanderMovePolicy
        };
    }
}
