using Il2CppInterop.Runtime;
using KappiMod.Constants;
using KappiMod.Events;
using KappiMod.Logging;
using KappiMod.Patches;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniverseLib.Utility;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Mods.Extensions;

internal class ChibiMitaDialogueFix : IDisposable
{
    private const string BROKEN_DIALOGUE = "3D TextFactory 5";

    private readonly DialogueStartPatch _dialoguePatch;
    private Mob_ChibiMita? _cachedChibiMita;

    internal ChibiMitaDialogueFix(DialogueStartPatch dialoguePatch)
    {
        _dialoguePatch = dialoguePatch;

        if (SceneManager.GetActiveScene().name is SceneName.CHIBIMITA)
        {
            TryFindChibiMita();
        }
        else
        {
            _cachedChibiMita = null;
        }

        KappiCore.Loader.SceneWasInitialized += OnSceneWasInitialized;
        _dialoguePatch.OnPostfixDialogueStart += HandleDialogue;

        KappiLogger.Log("Initialized");
    }

    public void Dispose()
    {
        KappiCore.Loader.SceneWasInitialized -= OnSceneWasInitialized;
        _dialoguePatch.OnPostfixDialogueStart -= HandleDialogue;

        KappiLogger.Log("Cleaned up");
    }

    private void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (sceneName is SceneName.CHIBIMITA)
        {
            _cachedChibiMita = null;
            TryFindChibiMita();
        }
        else if (_cachedChibiMita != null)
        {
            _cachedChibiMita = null;
        }
    }

    private void HandleDialogue(object? sender, DialogueEventArgs args)
    {
        if (args.ObjectName is not BROKEN_DIALOGUE)
        {
            return;
        }

        if (!TryFindChibiMita() || _cachedChibiMita == null)
        {
            return;
        }

        _cachedChibiMita.AnimationStop();
        KappiLogger.Log("ChibiMita animation stopped");
    }

    private bool TryFindChibiMita()
    {
        if (!_cachedChibiMita.IsNullOrDestroyed())
        {
            return true;
        }

        _cachedChibiMita = Resources
            .FindObjectsOfTypeAll(Il2CppType.Of<Mob_ChibiMita>())
            ?.FirstOrDefault(x => x.name == "ChibiMita")
            ?.Cast<Mob_ChibiMita>();

        bool isFound = !_cachedChibiMita.IsNullOrDestroyed();
        KappiLogger.Log($"ChibiMita {(isFound ? "found" : "not found")}");
        return isFound;
    }
}
