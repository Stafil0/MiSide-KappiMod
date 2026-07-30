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
internal sealed class RingInstantReadyPatch : IPatch
{
    public string Id => "com.kappimod.ringinstantready";
    public string Name => "Ring Instant Ready Patch";
    public string Description =>
        "Skips the ring wait event and makes the ring ready instantly in the Cappie chapter";

    private readonly HarmonyLib.Harmony _harmony;

    public RingInstantReadyPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(RingInstantReadyPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location7_RingWork), nameof(Location7_RingWork.Start))]
    private static void SkipRingWait(Location7_RingWork __instance)
    {
        try
        {
            __instance.ReadyTime();

            EventManager.ShowEvent(new($"{nameof(BlessRng)}: Ring wait event skipped"));
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to skip ring wait event", exception: ex);
        }
    }
}
