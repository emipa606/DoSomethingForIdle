using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace DSFI;

public class IdleJobGiverDef : Def
{
    public Type giverClass;

    public List<PawnCapacityDef> pawnCapacityRequirement = new List<PawnCapacityDef>();

    public float probabilityWeight;

    public List<SkillDef> relatedSkillPassion = new List<SkillDef>();

    public float searchDistance = 16f;

    public int usefulness;

    public WorkTags workTagsRequirement;

    public List<WorkTypeDef> workTypeRequirement = new List<WorkTypeDef>();
}
