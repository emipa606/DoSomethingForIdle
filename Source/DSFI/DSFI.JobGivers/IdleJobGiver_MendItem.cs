using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.JobGivers;

public class IdleJobGiver_MendItem : IdleJobGiver<IdleJobGiverDef>
{
    private readonly List<Thing> targets = new List<Thing>();

    public override Job TryGiveJob(Pawn pawn)
    {
        if (pawn.GetRegion() == null)
        {
            return null;
        }

        var skills = pawn.skills;
        if (skills != null && skills.GetSkill(SkillDefOf.Crafting).Level == 0)
        {
            return null;
        }

        targets.Clear();
        foreach (var item in pawn.Map.zoneManager.AllZones.Where(x => x is Zone_Stockpile))
        {
            foreach (var allContainedThing in item.AllContainedThings)
            {
                if (allContainedThing is Apparel { WornByCorpse: false } apparel && apparel.IsInValidBestStorage() &&
                    apparel.HitPoints < apparel.MaxHitPoints &&
                    pawn.CanReserveAndReach(apparel, PathEndMode.Touch, Danger.None) &&
                    apparel.def.stuffCategories != null &&
                    apparel.def.stuffCategories.Any(x => x.defName is "Fabric" or "Leathery"))
                {
                    targets.Add(apparel);
                }
            }
        }

        if (targets.Count <= 0)
        {
            return null;
        }

        var thing = targets.RandomElement();
        return new Job(IdleJobDefOf.IdleJob_MendItem, thing)
        {
            locomotionUrgency = modSettings.wanderMovePolicy
        };
    }
}
