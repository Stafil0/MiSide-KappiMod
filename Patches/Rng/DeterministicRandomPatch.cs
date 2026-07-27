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
        if (!_source.Enabled)
        {
            return true;
        }

        __result = _source.Value();
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
        if (!_source.Enabled)
        {
            return true;
        }

        __result = _source.InsideUnitSphere();
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
        if (!_source.Enabled)
        {
            return true;
        }

        __result = _source.InsideUnitCircle();
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
        if (!_source.Enabled)
        {
            return true;
        }

        __result = _source.OnUnitSphere();
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
        if (!_source.Enabled)
        {
            return true;
        }

        __result = _source.Rotation();
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
        if (!_source.Enabled)
        {
            return true;
        }

        __result = _source.RotationUniform();
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
        if (!_source.Enabled)
        {
            return true;
        }

        __result = _source.RangeFloat(minInclusive, maxInclusive);
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
        if (!_source.Enabled)
        {
            return true;
        }

        __result = _source.RangeInt(minInclusive, maxExclusive);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UnityEngine.Random), nameof(UnityEngine.Random.GetRandomUnitCircle))]
    private static bool GetRandomUnitCircle(out Vector2 output)
    {
        if (!_source.Enabled)
        {
            output = Vector2.zero;
            return true;
        }

        output = _source.GetRandomUnitCircle();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UnityEngine.Random), nameof(UnityEngine.Random.InitState))]
    private static void InitState(int seed)
    {
        if (!_source.Enabled)
        {
            return;
        }

        _source.ForcedSeed = seed;
    }

    #endregion Methods Patches
}
