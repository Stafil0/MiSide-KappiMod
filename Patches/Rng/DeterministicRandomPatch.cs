using HarmonyLib;
using KappiMod.Patches.Core;
using UnityEngine;

namespace KappiMod.Patches.Rng;

[HarmonyPatch]
public sealed class DeterministicRandomPatch : IPatch
{
    public string Id => "com.kappimod.deterministicrandom";
    public string Name => "Deterministic Random";
    public string Description => "Forces Unity's Random class to return deterministic values";

    public static bool IsInitialized => _harmony is not null;

    private static DeterministicRandom _source = new();
    private static HarmonyLib.Harmony? _harmony;

    public DeterministicRandomPatch(int forcedSeed = 12345, bool forceZeroRandom = false)
    {
        if (IsInitialized)
        {
            return;
        }

        _source = new DeterministicRandom(
            enabled: false,
            forcedSeed: forcedSeed,
            forceZeroRandom: forceZeroRandom
        );

        _harmony = new(Id);
        _harmony.PatchAll(typeof(DeterministicRandomPatch));
    }

    public void Dispose()
    {
        _harmony?.UnpatchSelf();
        _harmony = null;
        _source = new();
    }

    public static DeterministicRandom GetSource()
    {
        ValidateHarmonyPatch();
        return _source;
    }

    public static void SetSource(DeterministicRandom source)
    {
        ValidateHarmonyPatch();
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public static RandomState GetState() => GetSource().GetState();

    public static void SetState(RandomState state) => GetSource().SetState(state);

    public static bool Enabled
    {
        get => GetSource().Enabled;
        set => GetSource().Enabled = value;
    }

    public static int ForcedSeed
    {
        get => GetSource().ForcedSeed;
        set => GetSource().ForcedSeed = value;
    }

    public static bool ForceZeroRandom
    {
        get => GetSource().ForceZeroRandom;
        set => GetSource().ForceZeroRandom = value;
    }

    private static void ValidateHarmonyPatch()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException(
                $"{nameof(DeterministicRandomPatch)} is not initialized. "
                    + "Create an instance before using "
                    + $"{nameof(GetSource)}, {nameof(SetSource)}, {nameof(GetState)}, or {nameof(SetState)}."
            );
        }
    }

    #region Getters Patches

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UnityEngine.Random), nameof(UnityEngine.Random.value), MethodType.Getter)]
    private static bool Value(ref float __result)
    {
        var source = _source;
        if (!source.Enabled)
        {
            return true;
        }

        __result = source.Value();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(UnityEngine.Random),
        nameof(UnityEngine.Random.insideUnitSphere),
        MethodType.Getter
    )]
    private static bool InsideUnitSphere(ref Vector3 __result)
    {
        var source = _source;
        if (!source.Enabled)
        {
            return true;
        }

        __result = source.InsideUnitSphere();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(UnityEngine.Random),
        nameof(UnityEngine.Random.insideUnitCircle),
        MethodType.Getter
    )]
    private static bool InsideUnitCircle(ref Vector2 __result)
    {
        var source = _source;
        if (!source.Enabled)
        {
            return true;
        }

        __result = source.InsideUnitCircle();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(UnityEngine.Random),
        nameof(UnityEngine.Random.onUnitSphere),
        MethodType.Getter
    )]
    private static bool OnUnitSphere(ref Vector3 __result)
    {
        var source = _source;
        if (!source.Enabled)
        {
            return true;
        }

        __result = source.OnUnitSphere();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(UnityEngine.Random),
        nameof(UnityEngine.Random.rotation),
        MethodType.Getter
    )]
    private static bool Rotation(ref Quaternion __result)
    {
        var source = _source;
        if (!source.Enabled)
        {
            return true;
        }

        __result = source.Rotation();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(UnityEngine.Random),
        nameof(UnityEngine.Random.rotationUniform),
        MethodType.Getter
    )]
    private static bool RotationUniform(ref Quaternion __result)
    {
        var source = _source;
        if (!source.Enabled)
        {
            return true;
        }

        __result = source.RotationUniform();
        return false;
    }

    #endregion Getters Patches

    #region Methods Patches

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(UnityEngine.Random),
        nameof(UnityEngine.Random.Range),
        new[] { typeof(float), typeof(float) }
    )]
    private static bool RangeFloat(float minInclusive, float maxInclusive, ref float __result)
    {
        var source = _source;
        if (!source.Enabled)
        {
            return true;
        }

        __result = source.RangeFloat(minInclusive, maxInclusive);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(UnityEngine.Random),
        nameof(UnityEngine.Random.Range),
        new[] { typeof(int), typeof(int) }
    )]
    private static bool RangeInt(int minInclusive, int maxExclusive, ref int __result)
    {
        var source = _source;
        if (!source.Enabled)
        {
            return true;
        }

        __result = source.RangeInt(minInclusive, maxExclusive);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UnityEngine.Random), nameof(UnityEngine.Random.GetRandomUnitCircle))]
    private static bool GetRandomUnitCircle(out Vector2 output)
    {
        var source = _source;
        if (!source.Enabled)
        {
            output = Vector2.zero;
            return true;
        }

        output = source.GetRandomUnitCircle();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UnityEngine.Random), nameof(UnityEngine.Random.InitState))]
    private static void InitState(int seed)
    {
        var source = _source;
        if (!source.Enabled)
        {
            return;
        }

        source.ForcedSeed = seed;
    }

    #endregion Methods Patches
}
