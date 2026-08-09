using UnityEngine;
using UniverseLib.Utility;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Events;

public sealed class Dialogue2DEventArgs : DialogueEventArgs
{
    private readonly Location18_Novella Novella;

    private Dialogue2DEventArgs(
        string objectName,
        string sceneName,
        int indexString,
        string text,
        GameObject? speaker,
        DialoguePatchType patchType,
        Location18_Novella novella
    )
        : base(objectName, sceneName, indexString, text, speaker, patchType)
    {
        Novella = novella;
    }

    public static Dialogue2DEventArgs Create(
        Location18_Novella novella,
        DialoguePatchType patchType
    )
    {
        string objectName = !novella.dialoguePlay.IsNullOrDestroyed()
            ? novella.dialoguePlay.name
            : novella.name;

        return new(
            objectName,
            novella.gameObject.scene.name,
            novella.indexStringDialogue,
            novella.textNeed ?? string.Empty,
            speaker: null,
            patchType,
            novella
        );
    }

    public override void Skip()
    {
        if (Novella.IsNullOrDestroyed() || !Novella.controllDialogue)
        {
            return;
        }

        if (Novella.playPrint)
        {
            if (Novella.textDialogue != null && Novella.textNeed != null)
            {
                Novella.textDialogue.text = Novella.textNeed;
            }

            Novella.DialoguePrintFinish();
            return;
        }

        if (Novella.dialogueShow && Novella.timeWasObject == 0f)
        {
            Novella.NextDialogue();
        }
    }
}
