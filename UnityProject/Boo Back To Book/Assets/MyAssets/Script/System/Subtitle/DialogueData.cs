using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Game/DialogueData")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        public string ID;
        public AudioClip Clip;

        [TextArea]
        public string SubTitleText;
    }

    public DialogueLine[] lines;
}
