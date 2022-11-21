using UnityEngine;
using Verse;

namespace DSFI;

[StaticConstructorOnStartup]
public class RTMoteBubble : MoteDualAttached
{
    public Material iconMat;

    public Pawn motePawn;

    public void SetupMoteBubble(Pawn pawn)
    {
        motePawn = pawn;
        iconMat = PortraitIconMaterialCache.MatFrom(pawn, Color.white);
    }

    public override void Draw()
    {
        base.Draw();
        if (motePawn == null || !(iconMat != null))
        {
            return;
        }

        var drawPos = DrawPos;
        drawPos.y += 0.01f;
        var alpha = Alpha;
        if (alpha <= 0f)
        {
            return;
        }

        var color = instanceColor;
        color.a *= alpha;
        var material = iconMat;
        if (color != material.color)
        {
            material = PortraitIconMaterialCache.MatFrom(motePawn, color);
        }

        var s = new Vector3(def.graphicData.drawSize.x, 1f, def.graphicData.drawSize.y);
        var matrix = default(Matrix4x4);
        matrix.SetTRS(drawPos, Quaternion.identity, s);
        Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
    }
}
