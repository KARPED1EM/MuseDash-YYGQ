using System;
using System.Collections.Generic;
using System.IO;
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
    internal enum Style { Default, DJMax, GC, Touhou }

    // The word-sprite identities we replace. Default art is embedded as "<id>.png" and the djmax
    // variant as "<id>_djmax.png"; a missing djmax variant falls back to the default.
    private static readonly string[] WordIdentities =
    {
        "ScorePerfect", "ScoreGreat", "ScorePass",
        "GoldPerfect", "GoldPerfectBg", "GoldGreat", "GoldGreatBg",
        "Early", "Late",
    };

    private static readonly Dictionary<string, Sprite> DefaultArt = new();
    private static readonly Dictionary<string, Sprite> DjmaxArt = new();
    private static readonly Dictionary<string, Sprite> GCArt = new();
    private static readonly Dictionary<string, Sprite> TouhouArt = new();
    // private static Sprite? _pickup; // TQL, theme-agnostic

    private static bool _styleResolved;

    internal static bool HasArt => DefaultArt.Count > 0 || DjmaxArt.Count > 0 || GCArt.Count > 0 || TouhouArt.Count > 0;
    // internal static Sprite? Pickup => _pickup;
    internal static Style Current { get; private set; } = Style.Default;

    /// <summary>Load art from the current effect pack directory. Clears and reloads on every
    ///     battle enter so a config change takes effect next run.</summary>
    internal static void EnsureLoaded()
    {
        DefaultArt.Clear();
        DjmaxArt.Clear();
        GCArt.Clear();
        TouhouArt.Clear();

        foreach (var id in WordIdentities)
        {
            if (TryLoad($"Default\\{id}.png", out var d)) DefaultArt[id] = d;
            if (TryLoad($"DJMax\\{id}_djmax.png", out var j)) DjmaxArt[id] = j;
            if (TryLoad($"GC\\{id}GC.png", out var g)) GCArt[id] = g;
            if (TryLoad($"Touhou\\{id}_touhou_black.png", out var t)) TouhouArt[id] = t;
        }
        Main.Log.Msg($"skin loaded | default {DefaultArt.Count}/{WordIdentities.Length}, djmax {DjmaxArt.Count}, GC {GCArt.Count}, Touhou {TouhouArt.Count}");
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
        // bool tag = false;
        if (_styleResolved) return;
        _styleResolved = true;
        // Current = IsDjmaxStyle() ? Style.DJMax : Style.Default;
        // if (IsDjmaxStyle()) Current = Style.DJMax; tag = true;
        // if (IsGCStyle()) Current = Style.GC; tag = true;
        // if (IsTouhouStyle()) Current = Style.Touhou; tag = true;
        //
        // if (!tag)
        // {
        //     Current = Style.Default;
        // }
        //
        // tag = false;
        Current = IsDjmaxStyle() ? Style.DJMax
            : IsGCStyle() ? Style.GC
            : IsTouhouStyle() ? Style.Touhou
            : Style.Default;
        
        Main.Log.Msg($"score style = {Current}");
    }

    /// <summary>The theme-agnostic identity of a word sprite, or null if it is not a judgment word.</summary>
    internal static string? Identify(string spriteName)
    {
        var kind = spriteName.Contains("Perfect") ? "Perfect"
            : spriteName.Contains("Great") ? "Great"
            : spriteName.Contains("Pass") ? "Pass"
            : spriteName.Contains("Early") ? "Early"
            : spriteName.Contains("Late") ? "Late"
            : null;

        if (kind == null) return null;
        // var prefix = spriteName.Contains("Gold") ? "Gold" : "Score";
        var prefix = spriteName.Contains("Gold") ? "Gold" 
            : spriteName.Contains("Score") ? "Score" : string.Empty;
            
        var background = spriteName.Contains("Bg") ? "Bg" : string.Empty;
        return prefix + kind + background;
    }

    /// <summary>The replacement sprite for an identity under the current style, or null to leave it.</summary>
    internal static Sprite? Resolve(string identity)
    {
        if (Current == Style.DJMax && DjmaxArt.TryGetValue(identity, out var djmax)) return djmax;
        if (Current == Style.GC && GCArt.TryGetValue(identity, out var gc)) return gc;
        if (Current == Style.Touhou &&  TouhouArt.TryGetValue(identity, out var touhou)) return touhou;

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

    private static bool IsGCStyle()
    {
        var ui = GameObject.Find("UI_2D/Standard/PnlBattle/PnlBattleUI");
        if (ui == null) return false;
        var root = ui.transform;
        for (var i = 0; i < root.childCount; i++)
        {
            var panel = root.GetChild(i);
            if (!panel.gameObject.activeSelf) continue;
            var djmax = panel.Find("Score/GC");
            if (djmax != null && djmax.gameObject.activeInHierarchy) return true;
        }
        return false;
    }

    private static bool IsTouhouStyle()
    {
        var ui = GameObject.Find("SceneObjectController");
        if (ui == null) return false;
        var root = ui.transform;
        for (var i = 0; i < root.childCount; i++)
        {
            var panel = root.GetChild(i);
            if (!panel.gameObject.activeSelf) continue;
            var djmax = panel.Find("Touhou_black");
            if (djmax != null && djmax.gameObject.activeInHierarchy) return true;
        }
        return false;
    }

    private static bool TryLoad(string fileName, out Sprite sprite)
    {
        sprite = null!;
        try
        {
            var filePath = Path.Combine(SettingManager.EffectPackPath, fileName);
            // Main.Log.Msg($"now {filePath}");
            if (!File.Exists(filePath)) return false;
            var bytes = File.ReadAllBytes(filePath);
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
            Main.Log.Warning($"failed to load '{fileName}': {e.Message}");
            return false;
        }
    }

}
