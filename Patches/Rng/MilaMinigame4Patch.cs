using HarmonyLib;
using KappiMod.Logging;
using KappiMod.Mods;
using KappiMod.UI.Internal.EventDisplay;
using UnityEngine;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches.Rng;

[HarmonyPatch]
internal sealed class MilaMinigame4Patch : ScopedRandomPatch
{
    public override string Id => "com.kappimod.milaminigame4";
    public override string Name => "Mila Minigame 4 Patch";
    public override string Description => "Mila invaders minigame: walls in one line at sides";

    private const float WallXMax = 0.4f;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location19_Game4), "RestartWorld")]
    private static void BeforeGame4Restart(out bool __state) => __state = DisableRandom();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location19_Game4), "RestartWorld")]
    private static void AfterGame4Restart(Location19_Game4 __instance, bool __state)
    {
        RestoreRandom(__state);

        try
        {
            SnapGame4WallsToSides(__instance);
            const string MESSAGE = "Mila Game 4: walls in one line at sides";
            EventManager.ShowEvent(new($"{nameof(BlessRng)}: {MESSAGE}"));
            KappiLogger.Log(MESSAGE);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to snap Game 4 walls to sides", exception: ex);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location19_Game4), "Update")]
    private static void BeforeGame4Update(out bool __state) => __state = DisableRandom();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location19_Game4), "Update")]
    private static void AfterGame4Update(bool __state) => RestoreRandom(__state);

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
