using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DSFI.JobGivers;

public class IdleJobGiver_Gardening : IdleJobGiver<IdleJobGiverDef>
{
    public override Job TryGiveJob(Pawn pawn)
    {
        var plantSkill = pawn.skills.GetSkill(SkillDefOf.Plants).Level;
        if (!pawn.Map.zoneManager.AllZones.Where(delegate(Zone x)
            {
                if (x is not Zone_Growing zone_Growing)
                {
                    return false;
                }

                if (zone_Growing.cells.Count == 0)
                {
                    return false;
                }

                if (zone_Growing.GetPlantDefToGrow() == null || zone_Growing.GetPlantDefToGrow().plant == null)
                {
                    return false;
                }

                return plantSkill >= Mathf.RoundToInt(zone_Growing.GetPlantDefToGrow().plant.sowMinSkill * 0.75f);
            }).TryRandomElement(out var result))
        {
            return null;
        }

        var intVec = AIUtility.FindRandomSpotInZone(pawn, result, true, true);
        if (intVec != IntVec3.Invalid)
        {
            return new Job(IdleJobDefOf.IdleJob_Gardening, intVec)
            {
                locomotionUrgency = modSettings.wanderMovePolicy
            };
        }

        return null;
    }
}
