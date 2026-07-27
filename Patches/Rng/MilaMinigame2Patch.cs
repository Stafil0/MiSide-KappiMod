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
internal sealed class MilaMinigame2Patch : IPatch
{
    public string Id => "com.kappimod.milaminigame2";
    public string Name => "Mila Minigame 2 Patch";
    public string Description => "Mila towers mini-game: zigzag pattern";

    private const float MinCatchRadius = 0.25f;
    private const float MinStepMult = 0.7f;
    private const float MinDirZ = 0.05f;
    private const float LastTowerDirX = 1f;
    private const float LastTowerDirZ = 0.225f;
    private const float VisualScaleFactor = 0.2f;

    private readonly HarmonyLib.Harmony _harmony;

    public MilaMinigame2Patch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(MilaMinigame2Patch));
    }

    public void Dispose() => _harmony.UnpatchSelf();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location19_Game2), "Start")]
    private static void BeforeGame2Start(out RandomState __state)
    {
        __state = DeterministicRandomPatch.GetState();

        try
        {
            DeterministicRandomPatch.SetState(new() { Enabled = true, ForceZeroRandom = true });
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to disable random", exception: ex);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location19_Game2), "Start")]
    private static void AfterGame2Start(Location19_Game2 __instance, RandomState __state)
    {
        try
        {
            DeterministicRandomPatch.SetState(__state);

            ApplyGame2ZigzagLayout(__instance);

            EventManager.ShowEvent(new($"{nameof(BlessRng)}: Mila Game 2: zigzag pattern applied"));
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to apply Game 2 zigzag layout", exception: ex);
        }
    }

    private static void ApplyGame2ZigzagLayout(Location19_Game2 game)
    {
        var points = game.points;
        if (points == null || points.Count == 0)
        {
            return;
        }

        for (var i = 0; i < points.Count; i++)
        {
            ApplyCatchRadius(points[i], MinCatchRadius);
        }

        var anchor = points[0]?.point;
        if (anchor == null)
        {
            return;
        }

        var pos = anchor.transform.position;
        var pathLen = 0f;

        for (var i = 1; i < points.Count; i++)
        {
            var entry = points[i];
            var prev = points[i - 1];
            if (entry?.point == null || prev == null)
            {
                continue;
            }

            Vector3 dir;
            if (i == points.Count - 1)
            {
                // Peek left + slightly up from previous — easier grab, still on-screen.
                dir = new Vector3(LastTowerDirX, 0f, LastTowerDirZ).normalized;
            }
            else
            {
                // Alternate X sign: most sideways, least +Z → tight 2-column pile.
                var xSign = (i & 1) == 1 ? -1f : 1f;
                dir = new Vector3(xSign, 0f, MinDirZ).normalized;
            }

            var step = MinStepMult * (prev.distance + entry.distance);
            pos += dir * step;
            entry.point.transform.position = pos;
            pathLen += step;
        }

        var last = points[points.Count - 1]?.point;
        var endToEnd = last != null
            ? Vector3.Distance(anchor.transform.position, last.transform.position)
            : 0f;

        KappiLogger.Log($"[Game 2] zigzag {points.Count} towers, pathLen={pathLen:F3}, endToEnd={endToEnd:F3}, catchRadius={MinCatchRadius}, stepMult={MinStepMult}");
    }

    private static void ApplyCatchRadius(Location19_Game2_Point entry, float catchRadius)
    {
        if (entry == null)
        {
            return;
        }

        entry.distance = catchRadius;

        var circle = entry.sprCircle;
        if (circle == null)
        {
            return;
        }

        var scale = catchRadius * VisualScaleFactor;
        circle.transform.localScale = new Vector3(scale, scale, scale);
    }
}
