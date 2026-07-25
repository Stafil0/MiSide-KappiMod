using HarmonyLib;
using KappiMod.Logging;
using KappiMod.Mods;
using KappiMod.Patches.Core;
using KappiMod.UI.Internal.EventDisplay;
using UnityEngine;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches.Rng;

[HarmonyPatch]
internal sealed class ArenaBombPatch : IPatch
{
    public string Id => "com.kappimod.arenabomb";
    public string Name => "Arena Bomb Patch";
    public string Description =>
        "Run & Hide bombs: fixed music and eye timers";

    private static bool _disabledRng;
    private static int _phase;

    private readonly HarmonyLib.Harmony _harmony;

    public ArenaBombPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(ArenaBombPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    private static bool DisableRng()
    {
        var previous = _disabledRng;
        _disabledRng = true;
        return previous;
    }

    private static void RestoreRng(bool previous) => _disabledRng = previous;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location20_Arena), nameof(Location20_Arena.PlayPhase))]
    private static void BeforePlayPhase(int x, out bool __state)
    {
        _phase = x is 1 or 2 ? x : 0;
        __state = x is 1 or 2 ? DisableRng() : _disabledRng;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location20_Arena), nameof(Location20_Arena.PlayPhase))]
    private static void AfterPlayPhase(int x, bool __state)
    {
        RestoreRng(__state);

        if (x is not (1 or 2))
        {
            return;
        }

        EventManager.ShowEvent(new($"{nameof(BlessRng)}: Run & Hide bombs: fixed music and eye timers"));
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location20_Arena), nameof(Location20_Arena.ResetPhase))]
    private static void BeforeResetPhase(out bool __state) => __state = DisableRng();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location20_Arena), nameof(Location20_Arena.ResetPhase))]
    private static void AfterResetPhase(bool __state) => RestoreRng(__state);

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location20_Arena), nameof(Location20_Arena.MitaSwitchRecorder))]
    private static void BeforeSwitchRecorder(out bool __state)
    {
        _phase = 1;
        __state = DisableRng();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location20_Arena), nameof(Location20_Arena.MitaSwitchRecorder))]
    private static void AfterSwitchRecorder(bool __state) => RestoreRng(__state);

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location20_Arena), "Update")]
    private static void BeforeUpdate(out bool __state) => __state = DisableRng();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location20_Arena), "Update")]
    private static void AfterUpdate(bool __state) => RestoreRng(__state);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UnityEngine.Random), nameof(UnityEngine.Random.Range), new[] { typeof(float), typeof(float) })]
    private static void AfterRangeFloat(float minInclusive, float maxInclusive, ref float __result)
    {
        if (!_disabledRng)
        {
            return;
        }

        switch (_phase)
        {
            case 1:
                ForcePhase1Random(minInclusive, maxInclusive, ref __result);
                break;
            case 2:
                ForcePhase2Random(minInclusive, maxInclusive, ref __result);
                break;
        }
    }

    private static void ForcePhase1Random(float min, float max, ref float result)
    {
        // Music ON
        if (Approx(min, 2f) && Approx(max, 4f))
        {
            result = 4f;
            return;
        }

        // Music OFF
        if (Approx(min, 1f) && Approx(max, 2f))
        {
            result = 1f;
        }
    }

    private static void ForcePhase2Random(float min, float max, ref float result)
    {
        // Eyes closed (first time)
        if (Approx(min, 3f) && Approx(max, 6f))
        {
            result = 4f;
            return;
        }

        // Eyes closed (consecutive time)
        if (Approx(min, 2f) && Approx(max, 3f))
        {
            result = 3f;
            return;
        }

        // Eyes open window
        if (Approx(min, -5f) && Approx(max, -4f))
        {
            result = -4f;
        }
    }

    private static bool Approx(float a, float b) => Mathf.Abs(a - b) < 0.001f;
}
