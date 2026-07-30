using UnityEngine;

namespace KappiMod.Patches.Rng;

public struct RandomState
{
    public bool? Enabled;
    public int? ForcedSeed;
    public bool? ForceZeroRandom;
}

public interface IRandom
{
    bool Enabled { get; set; }
    int ForcedSeed { get; set; }
    bool ForceZeroRandom { get; set; }

    RandomState GetState();
    void SetState(RandomState state);

    float Value();
    Vector3 InsideUnitSphere();
    Vector2 InsideUnitCircle();
    Vector3 OnUnitSphere();
    Quaternion Rotation();
    Quaternion RotationUniform();
    float RangeFloat(float minInclusive, float maxInclusive);
    int RangeInt(int minInclusive, int maxExclusive);
    Vector2 GetRandomUnitCircle();
}
