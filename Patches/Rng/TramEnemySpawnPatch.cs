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
internal sealed class TramEnemySpawnPatch : IPatch
{
    public string Id => "com.kappimod.tramenemyspawn";
    public string Name => "Tram Enemy Spawn Patch";
    public string Description => "Tram enemies: fixed spawn offsets";

    private readonly HarmonyLib.Harmony _harmony;

    public TramEnemySpawnPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(TramEnemySpawnPatch));
    }

    public void Dispose() => _harmony.UnpatchSelf();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location11_Lift), "CreateEnemy")]
    private static void BeforeCreateEnemy(out RandomState __state)
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
    [HarmonyPatch(typeof(Location11_Lift), "CreateEnemy")]
    private static void AfterCreateEnemy(RandomState __state)
    {
        try
        {
            DeterministicRandomPatch.SetState(__state);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to restore random state", exception: ex);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location11_Lift), nameof(Location11_Lift.TurretUse))]
    private static void AfterTurretUse(bool x)
    {
        if (!x)
        {
            return;
        }

        EventManager.ShowEvent(new($"{nameof(BlessRng)}: Tram: enemy spawns fixed"));
    }
}
