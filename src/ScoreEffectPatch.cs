using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2CppAssets.Scripts.PeroTools.Managers;
// using Il2CppTMPro;
using UnityEngine;

namespace YYGQ;

/// <summary>
///     Every in-battle score visual spawns through <c>Effect.CreateInstance</c> — the single door this
///     postfix rides. Judgment WORDS are SpriteRenderers we swap by identity; the note-pickup SCORE is
///     a TextMeshPro number we hide and replace with an overlay sprite that fades with the vanilla one.
/// </summary>
[HarmonyPatch(typeof(Effect), nameof(Effect.CreateInstance))]
internal static class ScoreEffectPatch
{
    // private const string OverlayName = "YYGQ_Pickup";

    private static void Postfix(Effect __instance, GameObject __result)
    {
        try
        {
            if (__instance == null || __result == null) return;
            Apply(__result, __instance.uid);
        }
        catch (Exception)
        {
            // Leave the visual vanilla; never disrupt the game's effect pipeline.
        }
    }

    private static void Apply(GameObject instance, string uid)
    {
        if (string.IsNullOrEmpty(uid)) return;

        // The note-pickup score (TxtScoreGet / TxtScoreGetAir) is a TextMeshPro number, not a sprite.
        // if (JudgmentSkin.Pickup != null && uid.StartsWith("TxtScoreGet", StringComparison.Ordinal))
        // {
        //     ApplyPickup(instance);
        //     return;
        // }

        // Judgment words — gate to the score-effect family first (keeps collab skill vfx out).
        if (!JudgmentSkin.HasArt) return;
        if (!uid.StartsWith("ImgScore", StringComparison.Ordinal) &&
            !uid.StartsWith("ImgDouble", StringComparison.Ordinal)) return;
            // !uid.StartsWith("ImgEarly", StringComparison.Ordinal) &&
            // !uid.StartsWith("ImgLate", StringComparison.Ordinal)) return;

        JudgmentSkin.EnsureStyleResolved();

        // Swap every renderer that shows a word (foreground + the fever/gold background layer), leaving
        // the timing tags and any non-word layer untouched.
        foreach (var renderer in instance.GetComponentsInChildren<SpriteRenderer>(true))
        {
            var sprite = renderer?.sprite;
            if (sprite == null) continue;
            // Main.Log.Msg($"Applying sprite Name is {sprite.name}");
            var identity = JudgmentSkin.Identify(sprite.name);
            if (identity == null) continue;
            // Main.Log.Msg($"Applying identity is {identity}");
            var replacement = JudgmentSkin.Resolve(identity);
            if (replacement != null) renderer!.sprite = replacement;
        }
    }

    // Hide the TMP number's mesh and stand a sprite overlay in its place. The overlay is a plain
    // SpriteRenderer, so it does not inherit the Animator's alpha the way the number does — we mirror
    // the number's animated alpha onto it each frame (see PumpPickupFade) so it fades in/out with the
    // vanilla motion. Pool-safe: the overlay child is created once and reactivated on every reuse.
    // private static void ApplyPickup(GameObject instance)
    // {
    //     var sortingLayer = 0;
    //     var sortingOrder = 20;
    //     var mesh = instance.GetComponent<MeshRenderer>();
    //     if (mesh != null)
    //     {
    //         sortingLayer = mesh.sortingLayerID;
    //         sortingOrder = mesh.sortingOrder;
    //         mesh.enabled = false;
    //     }
    //
    //     SpriteRenderer overlay;
    //     var existing = instance.transform.Find(OverlayName);
    //     if (existing == null)
    //     {
    //         var go = new GameObject(OverlayName);
    //         go.transform.SetParent(instance.transform, false);
    //         go.transform.localPosition = Vector3.zero;
    //         overlay = go.AddComponent<SpriteRenderer>();
    //         overlay.sprite = JudgmentSkin.Pickup;
    //     }
    //     else
    //     {
    //         existing.gameObject.SetActive(true);
    //         overlay = existing.GetComponent<SpriteRenderer>();
    //         if (overlay == null) return;
    //     }
    //
    //     overlay.sortingLayerID = sortingLayer;
    //     overlay.sortingOrder = sortingOrder + 1;
    //     Track(overlay, instance);
    // }

    // private sealed class Overlay
    // {
    //     public SpriteRenderer Renderer = null!;
    //     public TMP_Text Source = null!;
    // }

    // private static readonly List<Overlay> Live = new();

    // private static void Track(SpriteRenderer overlay, GameObject instance)
    // {
    //     var source = instance.GetComponent<TMP_Text>();
    //     if (source == null) return;
    //     foreach (var entry in Live)
    //     {
    //         if (entry.Renderer != overlay) continue;
    //         entry.Source = source;
    //         return;
    //     }
    //     Live.Add(new Overlay { Renderer = overlay, Source = source });
    // }

    /// <summary>
    ///     Per-frame (Main.OnLateUpdate, after the game's animation pass): mirror each live pickup
    ///     number's animated alpha onto its overlay. Cheap — the list holds only currently-live pickups
    ///     (usually none), and a dead entry is pruned the frame its effect returns to the pool.
    /// </summary>
    
    // internal static void PumpPickupFade()
    // {
    //     for (var i = Live.Count - 1; i >= 0; i--)
    //     {
    //         var entry = Live[i];
    //         if (entry.Renderer == null || entry.Source == null || !entry.Renderer.gameObject.activeInHierarchy)
    //         {
    //             Live.RemoveAt(i);
    //             continue;
    //         }
    //
    //         var alpha = entry.Source.alpha;
    //         var color = entry.Renderer.color;
    //         if (color.a != alpha)
    //         {
    //             color.a = alpha;
    //             entry.Renderer.color = color;
    //         }
    //     }
    // }
}
