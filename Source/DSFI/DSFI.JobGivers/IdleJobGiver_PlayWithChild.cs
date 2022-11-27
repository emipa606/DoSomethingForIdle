using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.JobGivers;

public class IdleJobGiver_PlayWithChild : IdleJobGiver<IdleJobGiverDef>
{
    public override Job TryGiveJob(Pawn pawn)
    {
        if (!ModLister.BiotechInstalled)
        {
            return null;
        }

        var validPawns = pawn.Map.mapPawns.FreeColonists.Where(baby =>
            baby.Faction == pawn.Faction && baby.DevelopmentalStage.Baby() && baby.Awake() &&
            baby.needs?.play?.CurLevelPercentage < 0.95f &&
            baby.Spawned && baby.Position.DistanceTo(pawn.Position) < def.searchDistance);

        if (!validPawns.Any())
        {
            return null;
        }

        var baby = validPawns.RandomElement();
        foreach (var babyPlayDef in DefDatabase<BabyPlayDef>.AllDefs.InRandomOrder())
        {
            if (babyPlayDef.Worker.CanDo(pawn, baby))
            {
                return babyPlayDef.Worker.TryGiveJob(pawn, baby);
            }
        }

        return null;
    }
}
