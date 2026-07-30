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
internal sealed class MilaMinigame4Patch : IPatch
{
    public string Id => "com.kappimod.milaminigame4";
    public string Name => "Mila Minigame 4 Patch";
    public string Description => "Mila invaders mini-game: walls aligned in a single line on the sides";

    private const float WallXMax = 0.4f;

    private readonly HarmonyLib.Harmony _harmony;

    public MilaMinigame4Patch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(MilaMinigame4Patch));
    }

    public void Dispose() => _harmony.UnpatchSelf();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location19_Game4), "RestartWorld")]
    private static void BeforeGame4Restart(out RandomState __state)
    {
        __state = RandomPatch.GetState();

        try
        {
            RandomPatch.SetState(new() { Enabled = true, ForceZeroRandom = true });
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to disable random", exception: ex);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location19_Game4), "RestartWorld")]
    private static void AfterGame4Restart(Location19_Game4 __instance, RandomState __state)
    {
        try
        {
            RandomPatch.SetState(__state);

            SnapGame4WallsToSides(__instance);

            EventManager.ShowEvent(new($"{nameof(BlessRng)}: Mila Game 4: walls aligned in a single line on the sides"));
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to snap Game 4 walls to sides", exception: ex);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location19_Game4), "Update")]
    private static void BeforeGame4Update(out RandomState __state)
    {
        __state = RandomPatch.GetState();

        try
        {
            RandomPatch.SetState(new() { Enabled = true, ForceZeroRandom = true });
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to disable random", exception: ex);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location19_Game4), "Update")]
    private static void AfterGame4Update(RandomState __state)
    {
        try
        {
            RandomPatch.SetState(__state);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to restore random state", exception: ex);
        }
    }

    private static void SnapGame4WallsToSides(Location19_Game4 game)
    {
        var objects = game.createObjects;
        if (objects == null)
        {
            return;
        }

        var snapped = 0;
        for (var i = 0; i < objects.Count; i++)
        {
            var obj = objects[i];
            if (obj == null)
            {
                continue;
            }

            if (obj.GetComponentInParent<Location19_Game4_Ship>() != null)
            {
                continue;
            }

            var t = obj.transform;
            var pos = t.localPosition;
            if (Mathf.Abs(pos.x) < 1e-4f)
            {
                continue;
            }

            pos.x = Mathf.Sign(pos.x) * WallXMax;
            t.localPosition = pos;
            snapped++;
        }

        KappiLogger.Log($"[Game 4] snapped walls to sides");
    }
}
