using HarmonyLib;
using KappiMod.Logging;
using KappiMod.Patches.Core;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches.Rng;

[HarmonyPatch]
internal sealed class MilaGrabPatch : IPatch
{
    public string Id => "com.kappimod.milagrab";
    public string Name => "Mila Grab Patch";
    public string Description =>
        "Broken Mita only grabs the first scripted time; later grabs blocked. Teleport point fixed";

    private readonly HarmonyLib.Harmony _harmony;

    public MilaGrabPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(MilaGrabPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location8_MitaBrokeLife), nameof(Location8_MitaBrokeLife.MolestStart))]
    private static bool BlockRecurringMolest(Location8_MitaBrokeLife __instance)
    {
        try
        {
            return __instance.oneTime;
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to gate MolestStart", exception: ex);
            return true;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location8_MitaBrokeLife), nameof(Location8_MitaBrokeLife.TeleportRandom))]
    private static bool FixedTeleportPoint(Location8_MitaBrokeLife __instance)
    {
        try
        {
            var points = __instance.pointsRandomTeleport;
            if (points == null || points.Length == 0 || points[0] == null)
            {
                return true;
            }

            __instance.transform.position = points[0].position;
            return false;
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to fix TeleportRandom", exception: ex);
            return true;
        }
    }
}
