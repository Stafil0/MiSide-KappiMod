using HarmonyLib;
using KappiMod.Patches.Core;

namespace KappiMod.Patches.Rng;

[HarmonyPatch]
internal sealed partial class MilaMinigamesPatch : IPatch
{
    public string Id => "com.kappimod.milaminigames";
    public string Name => "Mila Minigames Patch";
    public string Description =>
        "Mila's 4 minigames pre-solved: straight laser, zigzag towers, figures matched, walls in one line";

    private readonly HarmonyLib.Harmony _harmony;

    public MilaMinigamesPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(MilaMinigamesPatch));
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
}
