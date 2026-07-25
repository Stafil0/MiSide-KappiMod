using HarmonyLib;
using KappiMod.Logging;
using KappiMod.Mods;
using KappiMod.Patches.Core;
using KappiMod.UI.Internal.EventDisplay;
using UnityEngine;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches.Rng;

[HarmonyPatch]
internal sealed class ErrorWindowsPatch : IPatch
{
    public string Id => "com.kappimod.errorwindows";
    public string Name => "Error Windows Patch";
    public string Description => "Error windows always stay in the first slot";

    private static readonly Vector3 _firstPosition = new(-6.692f, 1.341f, -1.094f);

    private readonly HarmonyLib.Harmony _harmony;

    public ErrorWindowsPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(ErrorWindowsPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location11_ErrorWindows), nameof(Location11_ErrorWindows.Play))]
    private static void AfterPlay(Location11_ErrorWindows __instance)
    {
        __instance.errorWindowClone.transform.localPosition = _firstPosition;
        __instance.errorWindowOKPosition.transform.localPosition = _firstPosition;
        __instance.errorWindowReady.transform.localPosition = _firstPosition;

        __instance.positions = new(new Vector3[15]);
        for (int i = 0; i < __instance.positions.Length; ++i)
        {
            __instance.positions[i] = _firstPosition;
        }

        const string MESSAGE = "Ghostly: error windows stay in the first slot";
        EventManager.ShowEvent(new($"{nameof(BlessRng)}: {MESSAGE}"));
        KappiLogger.Log(MESSAGE);
    }
}
