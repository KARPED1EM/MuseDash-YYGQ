using MelonLoader;

namespace YYGQ;

/// <summary>
///     A joke Muse Dash mod: it reskins the in-battle judgment words (PERFECT / GREAT / PASS and their
///     fever/gold variants) and the note-pickup score into the "YYGQ" set — NB! / SB? and TQL — with a
///     dedicated DJMax-style variant. Everything is embedded, so the mod is a single self-contained DLL.
/// </summary>
public class Main : MelonMod
{
    internal static MelonLogger.Instance Log { get; private set; } = null!;

    public override void OnInitializeMelon()
    {
        Log = LoggerInstance;
        // MelonMod auto-applies the [HarmonyPatch] classes in this assembly; nothing else to wire up.
    }

    // Decode the embedded art before the first note (off the note-hit path, so no first-judgment
    // hitch) and arm score-style detection for the new run. GameMain is the battle scene.
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName != "GameMain") return;
        JudgmentSkin.EnsureLoaded();
        JudgmentSkin.OnBattleEnter();
    }

    // Drive the pickup overlay's fade after the game's animation pass. Guarded: a hiccup here must
    // never break the frame — the worst case is one pickup that does not fade.
    
    // public override void OnLateUpdate()
    // {
    //     try { ScoreEffectPatch.PumpPickupFade(); }
    //     catch (System.Exception) { /* never break the frame over a fade */ }
    // }
}
