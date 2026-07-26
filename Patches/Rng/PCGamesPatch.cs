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
    public string Description => "The Real World PC mandatory sequence: files and sliders";

    private readonly HarmonyLib.Harmony _harmony;

    private readonly PatchManager _patchManager = new();

    public PCGamesPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(PCGamesPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location14_PCGames), nameof(Location14_PCGames.OpenFilesGame))]
    private static void BeforeOpenFilesGame(out RandomState __state)
    {
        __state = DeterministicRandomPatch.GetState();

        try
        {
            DeterministicRandomPatch.SetState(new() { Enabled = true, ForceZeroRandom = true });
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to disable random", exception: ex);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location14_PCGames), nameof(Location14_PCGames.OpenFilesGame))]
    private static void AfterOpenFilesGame(RandomState __state)
    {
        try
        {
            DeterministicRandomPatch.SetState(__state);

            EventManager.ShowEvent(new($"{nameof(BlessRng)}: PC files puzzle solved"));
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to restore random state", exception: ex);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location14_PCGames), nameof(Location14_PCGames.OpenTreeGame))]
    private static void BeforeOpenTreeGame(out RandomState __state)
    {
        __state = DeterministicRandomPatch.GetState();

        try
        {
            DeterministicRandomPatch.SetState(new() { Enabled = true, ForceZeroRandom = true });
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to disable random", exception: ex);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location14_PCGames), nameof(Location14_PCGames.OpenTreeGame))]
    private static void AfterOpenTreeGame(RandomState __state)
    {
        try
        {
            DeterministicRandomPatch.SetState(__state);

            EventManager.ShowEvent(new($"{nameof(BlessRng)}: PC slider puzzle solved"));
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to restore random state", exception: ex);
        }
    }
}
