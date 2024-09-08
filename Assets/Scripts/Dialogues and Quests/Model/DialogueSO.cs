using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Narration/Dialogues")]
public class DialogueSO : ScriptableObject
{
    [SerializeField] public List<DialoguesLines> lines;
    public int lineCount => lines.Count;

    [Serializable]
    public struct DialoguesLines
    {
        [TextArea]
        public string Text;
        public List<Options> Options;
    }

    [Serializable]
    public struct Options
    {
        public string Option;
        public int ReplyIndex;
    }
}


