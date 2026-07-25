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
internal sealed class LoopClockPatch : IPatch
{
    public string Id => "com.kappimod.loopclock";
    public string Name => "Loop Clock Patch";
    public string Description =>
        "Auto-matches the clock so no dialing is required";

    private const float SecondsPerCycle = 60f;
    private const float MinutesPerCycle = 12f;

    private readonly HarmonyLib.Harmony _harmony;

    public LoopClockPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(LoopClockPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location8_Clock), nameof(Location8_Clock.OtherClockMove))]
    private static void ForceClockMatch(Location8_Clock __instance, bool _x)
    {
        try
        {
            if (_x)
            {
                return;
            }

            __instance.timeClock = __instance.timeOtherClock;

            SyncPlayerClockHands(__instance);

            EventManager.ShowEvent(new($"{nameof(BlessRng)}: Clock was auto-matched"));
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to auto-match loop clock", exception: ex);
        }
    }

    private static void SyncPlayerClockHands(Location8_Clock clock)
    {
        var t = clock.timeClock;
        if (clock.clockSecond != null)
        {
            clock.clockSecond.localRotation = Quaternion.Euler(0f, t * SecondsPerCycle * 360f, 0f);
        }

        if (clock.clockMinute != null)
        {
            clock.clockMinute.localRotation = Quaternion.Euler(0f, t * MinutesPerCycle * 360f, 0f);
        }

        if (clock.clockHour != null)
        {
            clock.clockHour.localRotation = Quaternion.Euler(0f, t * 360f, 0f);
        }
    }
}
