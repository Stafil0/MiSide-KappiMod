using UnityEngine;

namespace KappiMod.Patches.Rng;

public class CustomRandom : IRandom
{
    public bool Enabled { get; set; }

    private int _forcedSeed;
    public int ForcedSeed
    {
        get => _forcedSeed;
        set
        {
            _forcedSeed = value;
            _random = new System.Random(value);
        }
    }

    public bool ForceZeroRandom { get; set; }

    private System.Random _random;

    public CustomRandom(bool enabled = false, int forcedSeed = 12345, bool forceZeroRandom = false)
    {
        Enabled = enabled;
        _forcedSeed = forcedSeed;
        ForceZeroRandom = forceZeroRandom;
        _random = new System.Random(_forcedSeed);
    }

    public CustomRandom(IRandom from)
        : this(from.Enabled, from.ForcedSeed, from.ForceZeroRandom) { }

    public RandomState GetState() =>
        new()
        {
            Enabled = Enabled,
            ForcedSeed = ForcedSeed,
            ForceZeroRandom = ForceZeroRandom,
        };

    public void SetState(RandomState state)
    {
        if (state.Enabled is bool enabled)
        {
            Enabled = enabled;
        }

        if (state.ForcedSeed is int seed)
        {
            ForcedSeed = seed;
        }

        if (state.ForceZeroRandom is bool force)
        {
            ForceZeroRandom = force;
        }
    }

    public virtual float Value()
    {
        if (ForceZeroRandom)
        {
            return 0.0f;
        }

        return (float)_random.NextDouble();
    }

    public virtual Vector3 InsideUnitSphere()
    {
        if (ForceZeroRandom)
        {
            return Vector3.zero;
        }

        float phi = 2.0f * Mathf.PI * (float)_random.NextDouble();
        float cosTheta = 2.0f * (float)_random.NextDouble() - 1.0f;
        float radius = Mathf.Pow((float)_random.NextDouble(), 1.0f / 3.0f);

        float sinTheta = Mathf.Sqrt(1.0f - cosTheta * cosTheta);

        float x = radius * sinTheta * Mathf.Cos(phi);
        float y = radius * sinTheta * Mathf.Sin(phi);
        float z = radius * cosTheta;

        return new Vector3(x, y, z);
    }

    public virtual Vector2 InsideUnitCircle()
    {
        if (ForceZeroRandom)
        {
            return Vector2.zero;
        }

        float angle = 2.0f * Mathf.PI * (float)_random.NextDouble();
        float radius = Mathf.Sqrt((float)_random.NextDouble());

        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);

        return new Vector2(x, y);
    }

    public virtual Vector3 OnUnitSphere()
    {
        if (ForceZeroRandom)
        {
            return Vector3.zero;
        }

        float u1 = (float)_random.NextDouble();
        float u2 = (float)_random.NextDouble();
        float sqrtU1 = Mathf.Sqrt(u1);
        float phi = 2.0f * Mathf.PI * u2;

        float x = sqrtU1 * Mathf.Cos(phi);
        float y = sqrtU1 * Mathf.Sin(phi);
        float z = Mathf.Sqrt(1.0f - u1);

        return new Vector3(x, y, z);
    }

    public virtual Quaternion Rotation()
    {
        if (ForceZeroRandom)
        {
            return Quaternion.identity;
        }

        float u1 = (float)_random.NextDouble();
        float u2 = (float)_random.NextDouble();
        float u3 = (float)_random.NextDouble();

        float sqrt1MinusU1 = Mathf.Sqrt(1.0f - u1);
        float sqrtU1 = Mathf.Sqrt(u1);

        float twoPI_U2 = 2.0f * Mathf.PI * u2;
        float twoPI_U3 = 2.0f * Mathf.PI * u3;

        float x = sqrt1MinusU1 * Mathf.Sin(twoPI_U2);
        float y = sqrt1MinusU1 * Mathf.Cos(twoPI_U2);
        float z = sqrtU1 * Mathf.Sin(twoPI_U3);
        float w = sqrtU1 * Mathf.Cos(twoPI_U3);

        return new Quaternion(x, y, z, w);
    }

    public virtual Quaternion RotationUniform() => Rotation();

    public virtual float RangeFloat(float minInclusive, float maxInclusive)
    {
        if (ForceZeroRandom)
        {
            return minInclusive;
        }

        return minInclusive + (float)_random.NextDouble() * (maxInclusive - minInclusive);
    }

    public virtual int RangeInt(int minInclusive, int maxExclusive)
    {
        if (ForceZeroRandom)
        {
            return minInclusive;
        }

        return _random.Next(minInclusive, maxExclusive);
    }

    public virtual Vector2 GetRandomUnitCircle()
    {
        if (ForceZeroRandom)
        {
            return Vector2.zero;
        }

        float angle = 2.0f * Mathf.PI * (float)_random.NextDouble();
        float radius = Mathf.Sqrt((float)_random.NextDouble());

        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);

        return new Vector2(x, y);
    }
}
