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
    public string Description => "Run & Hide corridor: straight paths only";

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
    private static void BeforeStart(out IRandom __state) => __state = ChangeRandomSource();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location20_RunCorridor), "Start")]
    private static void AfterStart(IRandom __state)
    {
        RestoreRandomSource(__state);

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
    private static void BeforeCreateGeneration(out IRandom __state) =>
        __state = ChangeRandomSource();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location20_RunCorridor), "CreateGeneration")]
    private static void AfterCreateGeneration(IRandom __state) => RestoreRandomSource(__state);

    private static IRandom ChangeRandomSource()
    {
        var previous = RandomPatch.GetSource();

        try
        {
            var next = new CorridorRandom(previous);
            next.SetState(new() { Enabled = true, ForceZeroRandom = true });
            RandomPatch.SetSource(next);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to disable random", exception: ex);
        }

        return previous;
    }

    private static void RestoreRandomSource(IRandom previous)
    {
        try
        {
            RandomPatch.SetSource(previous);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to restore random state", exception: ex);
        }
    }
}

internal sealed class CorridorRandom : CustomRandom
{
    public CorridorRandom(IRandom from)
        : base(from) { }

    public override int RangeInt(int minInclusive, int maxExclusive)
    {
        if (maxExclusive <= minInclusive)
        {
            return base.RangeInt(minInclusive, maxExclusive);
        }

        if (minInclusive == 0 && maxExclusive == 2)
        {
            return 0;
        }

        return maxExclusive - 1;
    }
}
