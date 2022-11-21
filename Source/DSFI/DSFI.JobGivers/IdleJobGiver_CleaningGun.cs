using Verse;
using Verse.AI;

namespace DSFI.JobGivers;

public class IdleJobGiver_CleaningGun : IdleJobGiver<IdleJobGiverDef>
{
    public override Job TryGiveJob(Pawn pawn)
    {
        Thing thing = null;
        if (pawn.equipment?.Primary != null && pawn.equipment.Primary.def.IsRangedWeapon)
        {
            thing = pawn.equipment.Primary;
        }

        if (thing == null)
        {
            return null;
        }

        if (thing.HitPoints >= thing.MaxHitPoints)
        {
            return null;
        }

        return new Job(IdleJobDefOf.IdleJob_CleaningGun, thing)
        {
            locomotionUrgency = modSettings.wanderMovePolicy
        };
    }
}
