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

internal sealed partial class MilaMinigamesPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location19_Game3), "Start")]
    private static void BeforeGame3Start(out bool __state) => __state = DisableRandom();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location19_Game3), "Start")]
    private static void AfterGame3Start(bool __state)
    {
        RestoreRandom(__state);

        const string MESSAGE = "Mila Game 3: solved board";
        EventManager.ShowEvent(new($"{nameof(BlessRng)}: {MESSAGE}"));
        KappiLogger.Log(MESSAGE);
    }
}
