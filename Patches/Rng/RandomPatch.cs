using HarmonyLib;
using KappiMod.Patches.Core;
using UnityEngine;

namespace KappiMod.Patches.Rng;

[HarmonyPatch]
public sealed class RandomPatch : IPatch
{
    public string Id => "com.kappimod.random";
    public string Name => "Random Patch";
    public string Description => "Forces Unity's Random class to use custom RNG";

    public static bool IsInitialized => _source is not null && _harmony is not null;

    private static IRandom? _source;
    private static IRandom Source
    {
        get
        {
            if (_source is null)
            {
                throw new InvalidOperationException($"{nameof(RandomPatch)} is not initialized.");
            }

            return _source;
        }
    }

    private static HarmonyLib.Harmony? _harmony;

    public RandomPatch(IRandom source)
    {
        if (IsInitialized)
        {
            throw new Exception($"{nameof(RandomPatch)} is already initialized");
        }

        _source = source;

        _harmony = new(Id);
        _harmony.PatchAll(typeof(RandomPatch));
    }

    public void Dispose()
    {
        _harmony?.UnpatchSelf();
        _harmony = null;
        _source = null;
    }

    public static IRandom GetSource()
    {
        ValidateHarmonyPatch();
        return _source!;
    }

    public static void SetSource(IRandom source)
    {
        ValidateHarmonyPatch();
        _source = source;
    }

    public static RandomState GetState() => GetSource().GetState();

    public static void SetState(RandomState state) => GetSource().SetState(state);

    private static void ValidateHarmonyPatch()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException(
                $"{nameof(RandomPatch)} is not initialized. "
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
        if (!Source.Enabled)
        {
            return true;
        }

        __result = Source.Value();
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
        if (!Source.Enabled)
        {
            return true;
        }

        __result = Source.InsideUnitSphere();
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
        if (!Source.Enabled)
        {
            return true;
        }

        __result = Source.InsideUnitCircle();
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
        if (!Source.Enabled)
        {
            return true;
        }

        __result = Source.OnUnitSphere();
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
        if (!Source.Enabled)
        {
            return true;
        }

        __result = Source.Rotation();
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
        if (!Source.Enabled)
        {
            return true;
        }

        __result = Source.RotationUniform();
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
        if (!Source.Enabled)
        {
            return true;
        }

        __result = Source.RangeFloat(minInclusive, maxInclusive);
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
        if (!Source.Enabled)
        {
            return true;
        }

        __result = Source.RangeInt(minInclusive, maxExclusive);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UnityEngine.Random), nameof(UnityEngine.Random.GetRandomUnitCircle))]
    private static bool GetRandomUnitCircle(out Vector2 output)
    {
        if (!Source.Enabled)
        {
            output = Vector2.zero;
            return true;
        }

        output = Source.GetRandomUnitCircle();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UnityEngine.Random), nameof(UnityEngine.Random.InitState))]
    private static void InitState(int seed)
    {
        if (!Source.Enabled)
        {
            return;
        }

        Source.ForcedSeed = seed;
    }

    #endregion Methods Patches
}
