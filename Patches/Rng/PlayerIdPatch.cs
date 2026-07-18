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
internal sealed class PlayerIdPatch : ScopedRandomPatch
{
    public override string Id => "com.kappimod.playerid";
    public override string Name => "Player ID Patch";
    public override string Description =>
        "Fixed last chapter monitor ID";

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location15_ScreenID), "Start")]
    private static void BeforeStart(out bool __state) => __state = DisableRandom();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location15_ScreenID), "Start")]
    private static void AfterStart(bool __state)
    {
        RestoreRandom(__state);

        const string MESSAGE = "Player ID: (0000)";
        EventManager.ShowEvent(new($"{nameof(BlessRng)}: {MESSAGE}"));
        KappiLogger.Log(MESSAGE);
    }
}
