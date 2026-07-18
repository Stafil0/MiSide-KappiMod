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
    public string Description => "Tram: fixed enemy spawn offsets";

    private readonly HarmonyLib.Harmony _harmony;

    public TramEnemySpawnPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(TramEnemySpawnPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    private static bool DisableRandom()
    {
        var previous = DeterministicRandomPatch.DisabledRandom;
        DeterministicRandomPatch.DisabledRandom = true;
        return previous;
    }

    private static void RestoreRandom(bool previous) => DeterministicRandomPatch.DisabledRandom = previous;

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

        const string MESSAGE = "Tram: fixed enemy spawns";
        EventManager.ShowEvent(new($"{nameof(BlessRng)}: {MESSAGE}"));
        KappiLogger.Log(MESSAGE);
    }
}
