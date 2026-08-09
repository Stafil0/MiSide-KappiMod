using HarmonyLib;
using KappiMod.Logging;
using KappiMod.Mods;
using KappiMod.Patches.Core;
using KappiMod.UI.Internal.EventDisplay;
using KappiMod.Utils;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches.Rng;

[HarmonyPatch]
internal sealed class PlayerIdPatch : IPatch
{
    public string Id => "com.kappimod.playerid";
    public string Name => "Player ID Patch";
    public string Description => "Fixes the last chapter monitor ID";

    private readonly HarmonyLib.Harmony _harmony;

    public PlayerIdPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(PlayerIdPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Location15_ScreenID), nameof(Location15_ScreenID.Start))]
    private static void AfterStart(Location15_ScreenID __instance)
    {
        try
        {
            __instance.IDplayer = "0000";

            var screens = __instance.textScreens;
            string playerName = SteamHelper.Instance?.GetPersonaName() ?? "Player";
            for (int i = 0; i < screens.Length; i++)
            {
                screens[i].m_Text = $"ID [{playerName}]:0000";
            }

            EventManager.ShowEvent(new($"{nameof(BlessRng)}: Player ID set to 0000"));
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to restore random state", exception: ex);
        }
    }
}
