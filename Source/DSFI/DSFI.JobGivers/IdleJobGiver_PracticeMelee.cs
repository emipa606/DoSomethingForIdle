using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.JobGivers;

public class IdleJobGiver_PracticeMelee : IdleJobGiver<IdleJobGiverDef>
{
    public override float GetWeight(Pawn pawn, Trait traitIndustriousness)
    {
        var num = 1f;
        if (pawn.story.traits.HasTrait(TraitDefOf.Brawler))
        {
            num *= 2f;
        }

        if (pawn.story.traits.HasTrait(TraitDefOf.Bloodlust))
        {
            num *= 1.2f;
        }

        return base.GetWeight(pawn, traitIndustriousness) * num;
    }

    public override Job TryGiveJob(Pawn pawn)
    {
        if (pawn.equipment?.Primary == null || !pawn.equipment.Primary.def.IsMeleeWeapon)
        {
            return null;
        }

        var result = IntVec3.Invalid;
        var ownedRoom = pawn.ownership?.OwnedRoom;
        if (ownedRoom != null && !ownedRoom.Cells
                .Where(x => x.Standable(pawn.Map) && !x.IsForbidden(pawn) &&
                            pawn.CanReserveAndReach(x, PathEndMode.OnCell, Danger.None))
                .TryRandomElement(out result))
        {
            result = AIUtility.FindRandomSpotOutsideColony(pawn, -1f, true, true);
        }

        if (result.IsValid)
        {
            return new Job(IdleJobDefOf.IdleJob_PracticeMelee, pawn.equipment.Primary, result)
            {
                locomotionUrgency = modSettings.wanderMovePolicy
            };
        }

        return null;
    }
}
