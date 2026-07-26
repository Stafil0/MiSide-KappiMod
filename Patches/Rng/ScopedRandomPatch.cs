using KappiMod.Patches.Core;

namespace KappiMod.Patches.Rng;

internal abstract class ScopedRandomPatch : IPatch
{
    public abstract string Id { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }

    private readonly HarmonyLib.Harmony _harmony;

    protected ScopedRandomPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(GetType());
    }

    public virtual void Dispose() => _harmony.UnpatchSelf();

    protected static bool DisableRandom()
    {
        var previous = DeterministicRandomPatch.ForceZeroRandom;
        DeterministicRandomPatch.ForceZeroRandom = true;
        return previous;
    }

    protected static void RestoreRandom(bool previous) =>
        DeterministicRandomPatch.ForceZeroRandom = previous;
}
