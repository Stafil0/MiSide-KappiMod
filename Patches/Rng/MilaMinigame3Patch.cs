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
internal sealed class MilaMinigame3Patch : ScopedRandomPatch
{
    public override string Id => "com.kappimod.milaminigame3";
    public override string Name => "Mila Minigame 3 Patch";
    public override string Description => "Mila figures mini-game: solved board";

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location19_Game3), "Start")]
    private static void BeforeGame3Start(out bool __state) => __state = DisableRandom();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location19_Game3), "Start")]
    private static void AfterGame3Start(bool __state)
    {
        RestoreRandom(__state);

        EventManager.ShowEvent(new($"{nameof(BlessRng)}: Mila Game 3: board solved"));
    }
}
