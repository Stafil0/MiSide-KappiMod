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
internal sealed class PCGamesPatch : IPatch
{
    public string Id => "com.kappimod.pcgames";
    public string Name => "PC Games Patch";
    public string Description =>
        "Real-world PC mandatory sequence, solved: files + sliders";

    private readonly HarmonyLib.Harmony _harmony;

    public PCGamesPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(PCGamesPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    private static bool DisableRandom()
    {
        var previous = DeterministicRandomPatch.DisabledRandom;
        DeterministicRandomPatch.DisabledRandom = true;
        return previous;
    }

    private static void RestoreRandom(bool previous) => DeterministicRandomPatch.DisabledRandom = previous;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location14_PCGames), nameof(Location14_PCGames.OpenFilesGame))]
    private static void BeforeOpenFilesGame(out bool __state) => __state = DisableRandom();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location14_PCGames), nameof(Location14_PCGames.OpenFilesGame))]
    private static void AfterOpenFilesGame(bool __state)
    {
        RestoreRandom(__state);

        const string MESSAGE = "PC files: solved";
        EventManager.ShowEvent(new($"{nameof(BlessRng)}: {MESSAGE}"));
        KappiLogger.Log(MESSAGE);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location14_PCGames), nameof(Location14_PCGames.OpenTreeGame))]
    private static void BeforeOpenTreeGame(out bool __state) => __state = DisableRandom();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location14_PCGames), nameof(Location14_PCGames.OpenTreeGame))]
    private static void AfterOpenTreeGame(bool __state)
    {
        RestoreRandom(__state);

        const string MESSAGE = "PC sliders: solved";
        EventManager.ShowEvent(new($"{nameof(BlessRng)}: {MESSAGE}"));
        KappiLogger.Log(MESSAGE);
    }
}
