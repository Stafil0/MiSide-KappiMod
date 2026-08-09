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
internal sealed class MilaMinigame3Patch : IPatch
{
    public string Id => "com.kappimod.milaminigame3";
    public string Name => "Mila Minigame 3 Patch";
    public string Description => "Mila figures mini-game: solved board";

    private readonly HarmonyLib.Harmony _harmony;

    public MilaMinigame3Patch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(MilaMinigame3Patch));
    }

    public void Dispose() => _harmony.UnpatchSelf();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location19_Game3), nameof(Location19_Game3.Start))]
    private static void BeforeGame3Start(out RandomState __state)
    {
        __state = new();
        try
        {
            __state = RandomPatch.GetState();
            RandomPatch.SetState(new() { Enabled = true, ForceZeroRandom = true });
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to disable random", exception: ex);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location19_Game3), nameof(Location19_Game3.Start))]
    private static void AfterGame3Start(RandomState __state)
    {
        try
        {
            RandomPatch.SetState(__state);

            EventManager.ShowEvent(new($"{nameof(BlessRng)}: Mila Game 3: board solved"));
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to restore random state", exception: ex);
        }
    }
}
