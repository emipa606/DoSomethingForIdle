using System;
using RimWorld;
using Verse;

namespace DSFI;

public abstract class IdleJobGiver<T> : IdleJobGiverBase where T : IdleJobGiverDef
{
    protected readonly DSFISettings modSettings = LoadedModManager.GetMod<DSFIMod>().GetSettings<DSFISettings>();
    protected T def;

    public override float GetWeight(Pawn pawn, Trait traitIndustriousness)
    {
        if (def == null)
        {
            return 0f;
        }

        if ((pawn.story.DisabledWorkTagsBackstoryAndTraits & def.workTagsRequirement) != 0)
        {
            return 0f;
        }

        foreach (var item in def.workTypeRequirement)
        {
            if ((pawn.story.DisabledWorkTagsBackstoryAndTraits & item.workTags) != 0)
            {
                return 0f;
            }
        }

        foreach (var item2 in def.pawnCapacityRequirement)
        {
            if (!pawn.health.capacities.CapableOf(item2))
            {
                return 0f;
            }
        }

        var num = 1f;
        foreach (var item3 in def.relatedSkillPassion)
        {
            var skill = pawn.skills.GetSkill(item3);
            if (skill.TotallyDisabled)
            {
                return 0f;
            }

            switch (skill.passion)
            {
                case Passion.Major:
                    num *= 1.5f;
                    break;
                case Passion.Minor:
                    num *= 1.2f;
                    break;
            }
        }

        if (traitIndustriousness != null && def.usefulness != 0)
        {
            num = (4f - Math.Abs(traitIndustriousness.Degree - def.usefulness)) / 2f;
        }

        return def.probabilityWeight * num;
    }

    public override void LoadDef(IdleJobGiverDef def)
    {
        this.def = def as T;
    }
}
