using HarmonyLib;
using KappiMod.Logging;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches.Rng;

[HarmonyPatch]
internal sealed class MilaMinigame1Patch : ScopedRandomPatch
{
    public override string Id => "com.kappimod.milaminigame1";
    public override string Name => "Mila Minigame 1 Patch";
    public override string Description => "Mila laser minigame: fixed shot curve";

    private static readonly float[] ShotCurve = { -1f, +0.5f, -0.5f, +1f, -0.5f };

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location19_Game1), "PointsRandom")]
    private static void LockGame1ShotCurve(Location19_Game1 __instance)
    {
        try
        {
            var curve = __instance.shotCurve;
            if (curve == null)
            {
                return;
            }

            var count = Math.Min(curve.Length, ShotCurve.Length);
            for (var i = 0; i < count; i++)
            {
                curve[i] = ShotCurve[i];
            }

            KappiLogger.Log($"[Game 1] shotCurve locked to [{string.Join(", ", ShotCurve)}]");
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to lock Game 1 shotCurve", exception: ex);
        }
    }
}
