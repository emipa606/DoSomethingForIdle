using Verse;
using Verse.AI;

namespace DSFI;

public class DSFISettings : ModSettings
{
    public bool noNapping;

    public LocomotionUrgency wanderMovePolicy = LocomotionUrgency.Walk;
    public float wanderMultiplier = 1f;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref wanderMultiplier, "wanderMultiplier", 1f);
        Scribe_Values.Look(ref noNapping, "noNapping");
        Scribe_Values.Look(ref wanderMovePolicy, "wanderMovePolicy", LocomotionUrgency.Walk);
        base.ExposeData();
    }
}
