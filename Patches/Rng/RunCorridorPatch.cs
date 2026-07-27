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
internal sealed class RunCorridorPatch : IPatch
{
    public string Id => "com.kappimod.runcorridor";
    public string Name => "Run Corridor Patch";
    public string Description =>
        "Run & Hide corridor: straight paths only";

    private readonly HarmonyLib.Harmony _harmony;

    public RunCorridorPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(RunCorridorPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location20_RunCorridor), "Start")]
    private static void BeforeStart(out DeterministicRandom __state) =>
        __state = InstallCorridorSource();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location20_RunCorridor), "Start")]
    private static void AfterStart(DeterministicRandom __state)
    {
        RestoreSource(__state);

        try
        {
            EventManager.ShowEvent(new($"{nameof(BlessRng)}: Run & Hide: straight paths only"));
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to show corridor event", exception: ex);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location20_RunCorridor), "CreateGeneration")]
    private static void BeforeCreateGeneration(out DeterministicRandom __state) =>
        __state = InstallCorridorSource();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location20_RunCorridor), "CreateGeneration")]
    private static void AfterCreateGeneration(DeterministicRandom __state) =>
        RestoreSource(__state);

    private static DeterministicRandom InstallCorridorSource()
    {
        var previous = DeterministicRandomPatch.GetSource();

        try
        {
            var next = new CorridorRandom(previous);
            next.SetState(new() { Enabled = true, ForceZeroRandom = true });
            DeterministicRandomPatch.SetSource(next);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to disable random", exception: ex);
        }

        return previous;
    }

    private static void RestoreSource(DeterministicRandom previous)
    {
        try
        {
            DeterministicRandomPatch.SetSource(previous);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to restore random state", exception: ex);
        }
    }
}
