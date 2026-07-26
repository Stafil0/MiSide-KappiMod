using HarmonyLib;
using KappiMod.Mods;
using KappiMod.UI.Internal.EventDisplay;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches.Rng;

[HarmonyPatch]
internal sealed class TramEnemySpawnPatch : ScopedRandomPatch
{
    public override string Id => "com.kappimod.tramenemyspawn";
    public override string Name => "Tram Enemy Spawn Patch";
    public override string Description => "Tram enemies: fixed spawn offsets";

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Location11_Lift), "CreateEnemy")]
    private static void BeforeCreateEnemy(out bool __state) => __state = DisableRandom();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location11_Lift), "CreateEnemy")]
    private static void AfterCreateEnemy(bool __state) => RestoreRandom(__state);

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
