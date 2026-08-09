using UnityEngine;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Events;

public sealed class Dialogue3DEventArgs : DialogueEventArgs
{
    private readonly Dialogue_3DText DialogueInstance;

    private Dialogue3DEventArgs(
        Dialogue_3DText dialogueInstance,
        string objectName,
        string sceneName,
        int indexString,
        string text,
        GameObject? speaker,
        DialoguePatchType patchType
    )
        : base(objectName, sceneName, indexString, text, speaker, patchType)
    {
        DialogueInstance = dialogueInstance;
    }

    public static Dialogue3DEventArgs Create(
        Dialogue_3DText instance,
        DialoguePatchType patchType
    ) =>
        new(
            instance,
            instance.name,
            // Object scene — GetActiveScene() is wrong with additive chapter loads.
            instance.gameObject.scene.name,
            instance.indexString,
            instance.textPrint,
            instance.speaker,
            patchType
        );

    public override void Skip() => DialogueInstance.SkipDialogue();
}
