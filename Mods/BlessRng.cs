using KappiMod.Config;
using KappiMod.Mods.Core;
using KappiMod.Patches.Core;
using KappiMod.Patches.Rng;
using KappiMod.Properties;

namespace KappiMod.Mods;

[ModInfo(
    name: "BlessRng Mod",
    description: "Removes RNG from the game",
    version: "1.2.0",
    author: BuildInfo.COMPANY
)]
public sealed class BlessRng : BaseMod
{
    public override bool IsEnabled
    {
        get => base.IsEnabled && ConfigManager.BlessRngMod.Value;
        protected set
        {
            base.IsEnabled = value;
            ConfigManager.BlessRngMod.Value = value;
        }
    }

    private readonly PatchManager _patchManager = new();

    protected override void OnEnable()
    {
        OnDisable();
        RegisterPatches();
    }

    protected override void OnDisable()
    {
        _patchManager.Dispose();
    }

    private void RegisterPatches()
    {
        _patchManager.RegisterPatch(new RandomPatch(new CustomRandom()));
        _patchManager.RegisterPatch<ChibiDoorUnlockerPatch>();
        _patchManager.RegisterPatch<ChipMiniGamePatch>();
        _patchManager.RegisterPatch<FixedItemSpawnPatch>();
        _patchManager.RegisterPatch<PassableDummiesPatch>();
        _patchManager.RegisterPatch<RingInstantReadyPatch>();
        _patchManager.RegisterPatch<LoopClockPatch>();
        _patchManager.RegisterPatch<MilaMinigame1Patch>();
        _patchManager.RegisterPatch<MilaMinigame2Patch>();
        _patchManager.RegisterPatch<MilaMinigame3Patch>();
        _patchManager.RegisterPatch<MilaMinigame4Patch>();
        _patchManager.RegisterPatch<RunCorridorPatch>();
        _patchManager.RegisterPatch<ArenaBombPatch>();
        _patchManager.RegisterPatch<TramEnemySpawnPatch>();
        _patchManager.RegisterPatch<ErrorWindowsPatch>();
        _patchManager.RegisterPatch<PCGamesPatch>();
        _patchManager.RegisterPatch<PlayerIdPatch>();

        // Disabled for now, need to discuss with other speedrunners before introducing it
        // _patchManager.RegisterPatch<MilaGrabPatch>();
    }
}
