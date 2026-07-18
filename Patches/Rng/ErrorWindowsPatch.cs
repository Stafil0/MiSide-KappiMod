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
internal sealed class ErrorWindowsPatch : ScopedRandomPatch
{
    public override string Id => "com.kappimod.errorwindows";
    public override string Name => "Error Windows Patch";
    public override string Description =>
        "Error window always jumps to the first slot";

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location11_ErrorWindows), nameof(Location11_ErrorWindows.ClickOkStage2))]
    private static void BeforeClickOkStage2(out bool __state) => __state = DisableRandom();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location11_ErrorWindows), nameof(Location11_ErrorWindows.ClickOkStage2))]
    private static void AfterClickOkStage2(bool __state) => RestoreRandom(__state);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location11_ErrorWindows), nameof(Location11_ErrorWindows.Play))]
    private static void AfterPlay()
    {
        const string MESSAGE = "Ghostly: error OK stays in first slot";
        EventManager.ShowEvent(new($"{nameof(BlessRng)}: {MESSAGE}"));
        KappiLogger.Log(MESSAGE);
    }
}
