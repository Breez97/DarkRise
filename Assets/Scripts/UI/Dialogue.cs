using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [System.Serializable]
    public class CharacterDialogue
    {
        public GameObject character;
        public string nameCharacter;
        [TextArea(3, 10)]
        public string[] sentences;
    }

    public CharacterDialogue[] characterDialogues;
}
