using HarmonyLib;
using KappiMod.Logging;
using KappiMod.Mods;
using KappiMod.UI.Internal.EventDisplay;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches.Rng;

[HarmonyPatch]
internal sealed class PCGamesPatch : ScopedRandomPatch
{
    public override string Id => "com.kappimod.pcgames";
    public override string Name => "PC Games Patch";
    public override string Description =>
        "The Real World PC mandatory sequence: files and sliders";

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location14_PCGames), nameof(Location14_PCGames.OpenFilesGame))]
    private static void BeforeOpenFilesGame(out bool __state) => __state = DisableRandom();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location14_PCGames), nameof(Location14_PCGames.OpenFilesGame))]
    private static void AfterOpenFilesGame(bool __state)
    {
        RestoreRandom(__state);

        EventManager.ShowEvent(new($"{nameof(BlessRng)}: PC files puzzle solved"));
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location14_PCGames), nameof(Location14_PCGames.OpenTreeGame))]
    private static void BeforeOpenTreeGame(out bool __state) => __state = DisableRandom();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location14_PCGames), nameof(Location14_PCGames.OpenTreeGame))]
    private static void AfterOpenTreeGame(bool __state)
    {
        RestoreRandom(__state);

        EventManager.ShowEvent(new($"{nameof(BlessRng)}: PC slider puzzle solved"));
    }
}
