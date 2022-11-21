using Mlie;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DSFI;

public class DSFIMod : Mod
{
    private static string currentVersion;
    private readonly DSFISettings settings;

    public DSFIMod(ModContentPack content)
        : base(content)
    {
        settings = GetSettings<DSFISettings>();
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(
                ModLister.GetActiveModWithIdentifier("Mlie.DoSomethingForIdle"));
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(inRect);
        listing_Standard.Label("DSFI_ConfigWanderMultiplier".Translate(), -1f,
            "DSFI_TT_ConfigWanderMultiplier".Translate());
        settings.wanderMultiplier = Widgets.HorizontalSlider(listing_Standard.GetRect(22f), settings.wanderMultiplier,
            0.01f, 2f, false, null, null, $"{settings.wanderMultiplier:f2}");
        listing_Standard.Gap();
        listing_Standard.Label("DSFI_ConfigWanderMovePolicy".Translate());
        var checkOn = settings.wanderMovePolicy == LocomotionUrgency.Walk;
        Widgets.CheckboxLabeled(listing_Standard.GetRect(22f), "DSFI_ConfigWanderMovePolicy_Walk".Translate(),
            ref checkOn);
        if (checkOn)
        {
            settings.wanderMovePolicy = LocomotionUrgency.Walk;
        }

        var checkOn2 = settings.wanderMovePolicy == LocomotionUrgency.Jog;
        Widgets.CheckboxLabeled(listing_Standard.GetRect(22f), "DSFI_ConfigWanderMovePolicy_Run".Translate(),
            ref checkOn2);
        if (checkOn2)
        {
            settings.wanderMovePolicy = LocomotionUrgency.Jog;
        }

        listing_Standard.Gap();
        listing_Standard.CheckboxLabeled("DSFI_NoNaps".Translate(), ref settings.noNapping,
            "DSFI_TT_NoNaps".Translate());
        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("DSFI_ModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
        base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "Do Something for Idle";
    }
}
