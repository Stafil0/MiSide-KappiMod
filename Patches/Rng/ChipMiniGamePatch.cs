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
internal sealed class ChipMiniGamePatch : IPatch
{
    public string Id => "com.kappimod.chipminigame";
    public string Name => "Chip Mini Game Patch";
    public string Description =>
        "Removes randomness from the chip mini-game by fixing the start and end points";

    private static readonly Vector3 _pointStart = new(-0.982f, 0.069f, 0.2185f);
    private static readonly Vector3 _pointFinish = new(0.982f, 0.069f, 0.2185f);

    private readonly HarmonyLib.Harmony _harmony;

    public ChipMiniGamePatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(ChipMiniGamePatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TamagotchiGame_Chip), nameof(TamagotchiGame_Chip.Restart))]
    private static void SetChipMiniGamePoints(TamagotchiGame_Chip __instance)
    {
        try
        {
            __instance.pointStart.gameObject.transform.localPosition = _pointStart;
            __instance.pointFinish.gameObject.transform.localPosition = _pointFinish;

            EventManager.ShowEvent(new($"{nameof(BlessRng)}: Chip mini-game points were set to fixed values"));
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to set points", exception: ex);
        }
    }
}
