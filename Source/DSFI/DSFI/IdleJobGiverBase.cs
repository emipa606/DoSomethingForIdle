using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI;

public abstract class IdleJobGiverBase
{
    public abstract float GetWeight(Pawn pawn, Trait traitIndustriousness);

    public abstract Job TryGiveJob(Pawn pawn);

    public abstract void LoadDef(IdleJobGiverDef def);
}
