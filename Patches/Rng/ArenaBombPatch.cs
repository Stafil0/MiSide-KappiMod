using HarmonyLib;
using KappiMod.Logging;
using KappiMod.Mods;
using KappiMod.Patches.Core;
using KappiMod.UI.Internal.EventDisplay;
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

    private static DeterministicRandom? _previous;

    private readonly HarmonyLib.Harmony _harmony;

    public ArenaBombPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(ArenaBombPatch));
    }

    public void Dispose()
    {
        try
        {
            RestorePreviousSource();
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to restore random source on dispose", exception: ex);
        }

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
                RestorePreviousSource();
                return;
            }

            InstallBombSource(x);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to install arena bomb random", exception: ex);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location20_Arena), nameof(Location20_Arena.PlayPhase))]
    private static void AfterPlayPhase(int x)
    {
        if (x is not (1 or 2))
        {
            return;
        }

        EventManager.ShowEvent(new($"{nameof(BlessRng)}: Run & Hide bombs: fixed music and eye timers"));
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location20_Arena), nameof(Location20_Arena.MitaSwitchRecorder))]
    private static void BeforeSwitchRecorder()
    {
        try
        {
            InstallBombSource(phase: 1);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to install arena bomb random", exception: ex);
        }
    }

    private static void InstallBombSource(int phase)
    {
        var current = DeterministicRandomPatch.GetSource();

        if (current is ArenaBombRandom bomb && bomb.Phase == phase && bomb.Enabled)
        {
            return;
        }

        if (current is not ArenaBombRandom)
        {
            _previous = current;
        }

        var basis = _previous ?? new DeterministicRandom(current);
        _previous ??= basis;

        var next = new ArenaBombRandom(basis, phase);
        next.SetState(new() { Enabled = true, ForceZeroRandom = false });
        DeterministicRandomPatch.SetSource(next);
    }

    private static void RestorePreviousSource()
    {
        if (_previous is null)
        {
            return;
        }

        DeterministicRandomPatch.SetSource(_previous);
        _previous = null;
    }
}
