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
    public string Description => "Keeps the error windows in the first slot";

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
        try
        {
            __instance.errorWindowClone.transform.localPosition = _firstPosition;
            __instance.errorWindowOKPosition.transform.localPosition = _firstPosition;
            __instance.errorWindowReady.transform.localPosition = _firstPosition;

            var positions = __instance.positions;
            if (positions == null)
            {
                positions = new(__instance.positions.Length);
                __instance.positions = positions;
            }

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = _firstPosition;
            }

            EventManager.ShowEvent(
                new($"{nameof(BlessRng)}: Ghostly: error windows remain in the first slot")
            );
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to set positions", exception: ex);
        }
    }
}
