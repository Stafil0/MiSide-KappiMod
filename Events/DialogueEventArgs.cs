using UnityEngine;

namespace KappiMod.Events;

public enum DialoguePatchType
{
    Postfix,
    Prefix,
}

public abstract class DialogueEventArgs : EventArgs
{
    public string ObjectName { get; private init; }
    public string SceneName { get; private init; }
    public int IndexString { get; private init; }
    public string Text { get; private init; }
    public GameObject? Speaker { get; private init; }
    public DialoguePatchType PatchType { get; private init; }

    protected DialogueEventArgs(
        string objectName,
        string sceneName,
        int indexString,
        string text,
        GameObject? speaker,
        DialoguePatchType patchType)
    {
        ObjectName = objectName;
        SceneName = sceneName;
        IndexString = indexString;
        Text = text;
        Speaker = speaker;
        PatchType = patchType;
    }

    public abstract void Skip();
}
