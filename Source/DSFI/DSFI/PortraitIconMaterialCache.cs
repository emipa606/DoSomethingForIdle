using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DSFI;

public static class PortraitIconMaterialCache
{
    private static readonly Dictionary<KeyValuePair<Pawn, Color>, Material> _matDict =
        new Dictionary<KeyValuePair<Pawn, Color>, Material>();

    private static readonly Dictionary<Pawn, Texture> _texDict = new Dictionary<Pawn, Texture>();

    public static Material MatFrom(Pawn pawn, Color color)
    {
        if (!UnityData.IsInMainThread)
        {
            Log.Error("Tried to get a material from a different thread.");
            return null;
        }

        var key = new KeyValuePair<Pawn, Color>(pawn, color);
        if (_matDict.TryGetValue(key, out var value))
        {
            return value;
        }

        if (!_texDict.TryGetValue(pawn, out var value2))
        {
            value2 = new RenderTexture(128, 128, 0)
            {
                filterMode = FilterMode.Bilinear
            };
            Find.PawnCacheRenderer.RenderPawn(pawn, (RenderTexture)value2, new Vector3(0f, 0f, 0.2f), 1f, 0f,
                Rot4.South, true, false, true, false, true);
            _texDict.Add(pawn, value2);
        }

        value = new Material(ShaderDatabase.TransparentPostLight)
        {
            name = $"{pawn.Name}_PortraitIcon",
            mainTexture = value2,
            color = color
        };
        _matDict.Add(key, value);

        return value;
    }
}
