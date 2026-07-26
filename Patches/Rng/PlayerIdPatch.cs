using HarmonyLib;
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
        "Fixes the last chapter monitor ID";

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location15_ScreenID), "Start")]
    private static void BeforeStart(out bool __state) => __state = DisableRandom();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location15_ScreenID), "Start")]
    private static void AfterStart(bool __state)
    {
        RestoreRandom(__state);

        EventManager.ShowEvent(new($"{nameof(BlessRng)}: Player ID set to (0000)"));
    }
}
