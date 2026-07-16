using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace YYGQ;

/// <summary>
///     The embedded judgment skin: the word art (NB! / SB? / icon and the gold fever variants) and the
///     note-pickup art (TQL), decoded once from the assembly's embedded PNGs.
///     <para>
///         Every scene theme names its word sprites the same way and differs only in which atlas the
///         scene loads, so art is keyed by a theme-agnostic <see cref="Identify" /> derived from the
///         sprite name. The score STYLE (a battle-HUD mode, not the background scene) then selects the
///         default or djmax variant — mirroring how MuseDashInfo+ reads the active Score/&lt;Style&gt;
///         sub-panel — and any style other than djmax falls back to the default art.
///     </para>
/// </summary>
internal static class JudgmentSkin
{
    internal enum Style { Default, Djmax }

    // The word-sprite identities we replace. Default art is embedded as "<id>.png" and the djmax
    // variant as "<id>_djmax.png"; a missing djmax variant falls back to the default.
    private static readonly string[] WordIdentities =
    {
        "ScorePerfect", "ScoreGreat", "ScorePass",
        "GoldPerfect", "GoldPerfectBg", "GoldGreat", "GoldGreatBg",
    };

    private static readonly Dictionary<string, Sprite> DefaultArt = new();
    private static readonly Dictionary<string, Sprite> DjmaxArt = new();
    private static Sprite? _pickup; // TQL, theme-agnostic

    private static bool _loaded;
    private static bool _styleResolved;

    internal static bool HasArt => DefaultArt.Count > 0 || DjmaxArt.Count > 0;
    internal static Sprite? Pickup => _pickup;
    internal static Style Current { get; private set; } = Style.Default;

    /// <summary>Decode the embedded art once. Textures are pinned against UnloadUnusedAssets.</summary>
    internal static void EnsureLoaded()
    {
        if (_loaded) return;
        _loaded = true;
        foreach (var id in WordIdentities)
        {
            if (TryLoad(id, out var d)) DefaultArt[id] = d;
            if (TryLoad(id + "_djmax", out var j)) DjmaxArt[id] = j;
        }
        TryLoad("note", out _pickup);
        Main.Log.Msg($"skin loaded — default {DefaultArt.Count}/{WordIdentities.Length}, djmax {DjmaxArt.Count}, pickup {_pickup != null}");
    }

    /// <summary>Arm score-style re-detection for a new run (resolved lazily on the first judgment).</summary>
    internal static void OnBattleEnter()
    {
        _styleResolved = false;
        Current = Style.Default;
    }

    /// <summary>
    ///     Resolve the score style once per run, on the first judgment: the battle HUD — whose active
    ///     Score/&lt;Style&gt; sub-panel is the source of truth — is fully built by then, unlike at scene load.
    /// </summary>
    internal static void EnsureStyleResolved()
    {
        if (_styleResolved) return;
        _styleResolved = true;
        Current = IsDjmaxStyle() ? Style.Djmax : Style.Default;
        Main.Log.Msg($"score style = {Current}");
    }

    /// <summary>The theme-agnostic identity of a word sprite, or null if it is not a judgment word.</summary>
    internal static string? Identify(string spriteName)
    {
        var kind = spriteName.Contains("Perfect") ? "Perfect"
            : spriteName.Contains("Great") ? "Great"
            : spriteName.Contains("Pass") ? "Pass"
            : null;
        if (kind == null) return null;
        var prefix = spriteName.Contains("Gold") ? "Gold" : "Score";
        var background = spriteName.Contains("Bg") ? "Bg" : string.Empty;
        return prefix + kind + background;
    }

    /// <summary>The replacement sprite for an identity under the current style, or null to leave it.</summary>
    internal static Sprite? Resolve(string identity)
    {
        if (Current == Style.Djmax && DjmaxArt.TryGetValue(identity, out var djmax)) return djmax;
        return DefaultArt.TryGetValue(identity, out var fallback) ? fallback : null;
    }

    // The active battle panel (PnlBattleOthers / Wisadel / Bloodheir / …) carries the live style's
    // Score sub-panel; djmax is on when that panel's Score/Djmax is active in the hierarchy.
    private static bool IsDjmaxStyle()
    {
        var ui = GameObject.Find("UI_2D/Standard/PnlBattle/PnlBattleUI");
        if (ui == null) return false;
        var root = ui.transform;
        for (var i = 0; i < root.childCount; i++)
        {
            var panel = root.GetChild(i);
            if (!panel.gameObject.activeSelf) continue;
            var djmax = panel.Find("Score/Djmax");
            if (djmax != null && djmax.gameObject.activeInHierarchy) return true;
        }
        return false;
    }

    private static bool TryLoad(string id, out Sprite sprite)
    {
        sprite = null!;
        try
        {
            var bytes = ReadEmbedded($"YYGQ.{id}.png");
            if (bytes == null) return false;
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            if (!ImageConversion.LoadImage(texture, bytes)) return false;
            texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
            // 100 px/unit + a centered pivot match the game's own word sprites, so same-sized art lands
            // at the same on-screen size and anchor as the original.
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            sprite.hideFlags = HideFlags.DontUnloadUnusedAsset;
            return true;
        }
        catch (Exception e)
        {
            Main.Log.Warning($"failed to load '{id}': {e.Message}");
            return false;
        }
    }

    private static byte[]? ReadEmbedded(string resourceName)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream == null) return null;
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return buffer.ToArray();
    }
}
