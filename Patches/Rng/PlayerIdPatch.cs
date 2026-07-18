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
internal sealed class PlayerIdPatch : IPatch
{
    public string Id => "com.kappimod.playerid";
    public string Name => "Player ID Patch";
    public string Description =>
        "Fixed last chapter monitor ID";

    private readonly HarmonyLib.Harmony _harmony;

    public PlayerIdPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(PlayerIdPatch));
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

    private static void RestoreRandom(bool previous) =>
        DeterministicRandomPatch.DisabledRandom = previous;

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
