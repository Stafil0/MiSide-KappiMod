using HarmonyLib;
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

    private static bool _disabledRng;

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

    private static bool DisableRng()
    {
        var previous = _disabledRng;
        _disabledRng = true;
        return previous;
    }

    private static void RestoreRng(bool previous) => _disabledRng = previous;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location20_RunCorridor), "Start")]
    private static void BeforeStart(out bool __state) => __state = DisableRng();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location20_RunCorridor), "Start")]
    private static void AfterStart(bool __state)
    {
        RestoreRng(__state);

        EventManager.ShowEvent(new($"{nameof(BlessRng)}: Run & Hide: straight paths only"));
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location20_RunCorridor), "CreateGeneration")]
    private static void BeforeCreateGeneration(out bool __state) => __state = DisableRng();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location20_RunCorridor), "CreateGeneration")]
    private static void AfterCreateGeneration(bool __state) => RestoreRng(__state);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UnityEngine.Random), nameof(UnityEngine.Random.Range), new[] { typeof(int), typeof(int) })]
    private static void AfterRangeInt(int minInclusive, int maxExclusive, ref int __result)
    {
        if (!_disabledRng || maxExclusive <= minInclusive)
        {
            return;
        }

        if (minInclusive == 0 && maxExclusive == 2)
        {
            __result = 0;
            return;
        }

        __result = maxExclusive - 1;
    }
}
