using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Canvas playerInfoCanvas;
    [SerializeField] private GameObject[] objectsToActivateScripts;
    public CharacterDialogueUI characterDialogueUI;

    private Queue<Dialogue.CharacterDialogue> dialogueQueue;
    private bool isTyping = false;

    void Start()
    {
        dialogueQueue = new Queue<Dialogue.CharacterDialogue>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueQueue.Clear();

        foreach (Dialogue.CharacterDialogue characterDialogue in dialogue.characterDialogues)
        {
            dialogueQueue.Enqueue(characterDialogue);
        }

        

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        Dialogue.CharacterDialogue currentDialogue = dialogueQueue.Peek();
        StartCoroutine(TypeSentence(currentDialogue));
    }

    IEnumerator TypeSentence(Dialogue.CharacterDialogue dialogue)
    {
        isTyping = true;

        dialogue.character.GetComponent<Canvas>().enabled = true;

        characterDialogueUI.nameText.text = dialogue.nameCharacter;

        foreach (string sentence in dialogue.sentences)
        {
            Debug.Log(dialogue.character.name + ": " + sentence);
            characterDialogueUI.dialogueText.text = ""; 

            foreach (char letter in sentence.ToCharArray())
            {
                characterDialogueUI.dialogueText.text += letter;
                yield return null; 
            }

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }

        dialogueQueue.Dequeue();
        isTyping = false;
        dialogue.character.GetComponent<Canvas>().enabled = false;

        if (dialogueQueue.Count > 0)
        {
            DisplayNextSentence();
        }
        else
        {
            EndDialogue();
        }
    }


    private void EndDialogue()
    {
        playerInfoCanvas.enabled = true;
        ActivateScriptsOnObjects();
    }

    private void ActivateScriptsOnObjects()
    {
        foreach (GameObject obj in objectsToActivateScripts)
        {
            MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour script in scripts)
            {
                script.enabled = true;
            }
        }
    }
}
