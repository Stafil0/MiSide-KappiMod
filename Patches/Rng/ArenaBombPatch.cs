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
    public string Description => "Run & Hide bombs: fixed music and eye timers";

    private static IRandom? _previous;

    private readonly HarmonyLib.Harmony _harmony;

    public ArenaBombPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(ArenaBombPatch));
    }

    public void Dispose()
    {
        RestoreRandomSource();
        _harmony.UnpatchSelf();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location20_Arena), nameof(Location20_Arena.PlayPhase))]
    private static void BeforePlayPhase(int x)
    {
        try
        {
            if (x is not (1 or 2))
            {
                RestoreRandomSource();
                return;
            }

            var previous = ChangeRandomSource(x);
            _previous ??= previous;
        }
        catch (Exception ex)
        {
            KappiLogger.LogException($"Failed in {nameof(ArenaBombPatch)}", exception: ex);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location20_Arena), nameof(Location20_Arena.PlayPhase))]
    private static void AfterPlayPhase(int x)
    {
        try
        {
            if (x is not (1 or 2))
            {
                return;
            }

            EventManager.ShowEvent(
                new($"{nameof(BlessRng)}: Run & Hide bombs: fixed music and eye timers")
            );
        }
        catch (Exception ex)
        {
            KappiLogger.LogException($"Failed in {nameof(ArenaBombPatch)}", exception: ex);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location20_Arena), nameof(Location20_Arena.MitaSwitchRecorder))]
    private static void BeforeSwitchRecorder()
    {
        try
        {
            var previous = ChangeRandomSource(phase: 1);
            _previous ??= previous;
        }
        catch (Exception ex)
        {
            KappiLogger.LogException($"Failed in {nameof(ArenaBombPatch)}", exception: ex);
        }
    }

    private static IRandom ChangeRandomSource(int phase)
    {
        var previous = RandomPatch.GetSource();

        if (previous is ArenaBombRandom bomb && bomb.Phase == phase && bomb.Enabled)
        {
            return previous;
        }

        try
        {
            var next = new ArenaBombRandom(previous, phase);
            next.SetState(new() { Enabled = true, ForceZeroRandom = false });
            RandomPatch.SetSource(next);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to disable random", exception: ex);
        }

        return previous;
    }

    private static void RestoreRandomSource()
    {
        if (_previous is null)
        {
            return;
        }

        try
        {
            RandomPatch.SetSource(_previous);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to restore random state", exception: ex);
        }

        _previous = null;
    }
}

internal sealed class ArenaBombRandom : CustomRandom
{
    public int Phase { get; }

    public ArenaBombRandom(IRandom from, int phase)
        : base(from)
    {
        Phase = phase;
    }

    public override float RangeFloat(float minInclusive, float maxInclusive) =>
        Phase switch
        {
            // Music ON
            1 when Approx(minInclusive, 2f) && Approx(maxInclusive, 4f) => 4f,
            // Music OFF
            1 when Approx(minInclusive, 1f) && Approx(maxInclusive, 2f) => 1f,
            // Eyes closed (first time)
            2 when Approx(minInclusive, 3f) && Approx(maxInclusive, 6f) => 4f,
            // Eyes closed (consecutive time)
            2 when Approx(minInclusive, 2f) && Approx(maxInclusive, 3f) => 3f,
            // Eyes open window
            2 when Approx(minInclusive, -5f) && Approx(maxInclusive, -4f) => -4f,
            _ => base.RangeFloat(minInclusive, maxInclusive),
        };

    private static bool Approx(float a, float b) => Mathf.Abs(a - b) < 0.001f;
}
