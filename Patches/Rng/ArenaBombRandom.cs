using UnityEngine;

namespace KappiMod.Patches.Rng;

/// <summary>
/// Run &amp; Hide arena bombs: fixed music / eye timer Random.Range(float) bands for a phase.
/// </summary>
internal sealed class ArenaBombRandom : DeterministicRandom
{
    public int Phase { get; }

    public ArenaBombRandom(DeterministicRandom from, int phase)
        : base(from)
    {
        Phase = phase;
    }

    public override float RangeFloat(float minInclusive, float maxInclusive)
    {
        switch (Phase)
        {
            case 1:
                if (TryForcePhase1(minInclusive, maxInclusive, out var phase1))
                {
                    return phase1;
                }
                break;
            case 2:
                if (TryForcePhase2(minInclusive, maxInclusive, out var phase2))
                {
                    return phase2;
                }
                break;
        }

        return base.RangeFloat(minInclusive, maxInclusive);
    }

    private static bool TryForcePhase1(float min, float max, out float result)
    {
        // Music ON
        if (Approx(min, 2f) && Approx(max, 4f))
        {
            result = 4f;
            return true;
        }

        // Music OFF
        if (Approx(min, 1f) && Approx(max, 2f))
        {
            result = 1f;
            return true;
        }

        result = default;
        return false;
    }

    private static bool TryForcePhase2(float min, float max, out float result)
    {
        // Eyes closed (first time)
        if (Approx(min, 3f) && Approx(max, 6f))
        {
            result = 4f;
            return true;
        }

        // Eyes closed (consecutive time)
        if (Approx(min, 2f) && Approx(max, 3f))
        {
            result = 3f;
            return true;
        }

        // Eyes open window
        if (Approx(min, -5f) && Approx(max, -4f))
        {
            result = -4f;
            return true;
        }

        result = default;
        return false;
    }

    private static bool Approx(float a, float b) => Mathf.Abs(a - b) < 0.001f;
}
