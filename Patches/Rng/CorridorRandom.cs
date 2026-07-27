namespace KappiMod.Patches.Rng;

/// <summary>
/// Run &amp; Hide corridor: force straight-path Random.Range(int) outcomes.
/// </summary>
internal sealed class CorridorRandom : DeterministicRandom
{
    public CorridorRandom(DeterministicRandom from)
        : base(from) { }

    public override int RangeInt(int minInclusive, int maxExclusive)
    {
        if (maxExclusive <= minInclusive)
        {
            return base.RangeInt(minInclusive, maxExclusive);
        }

        if (minInclusive == 0 && maxExclusive == 2)
        {
            return 0;
        }

        return maxExclusive - 1;
    }
}
