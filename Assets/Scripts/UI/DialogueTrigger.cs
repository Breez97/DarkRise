using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Canvas playerInfoCanvas;
    [SerializeField] private enum DialogueDirectionSpeak
    {
        Left,
        Right
    }

    [SerializeField] DialogueDirectionSpeak directionToSay = DialogueDirectionSpeak.Left;
    private GameObject player;
    public Dialogue dialogue;
    public float triggerDistance = 3f;
    private bool startDialogue = false;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        float xDifference = Mathf.Abs(player.transform.position.x - transform.position.x);

        if (distanceToPlayer <= triggerDistance && xDifference <= triggerDistance && !startDialogue)
        {
            startDialogue = true;
            TriggerDialogue();
        }
    }

    public void TriggerDialogue()
    {

        float directionMultiplier = (directionToSay == DialogueDirectionSpeak.Left) ? 1f : -1f;
        player.transform.localScale = new Vector3(player.transform.localScale.x * directionMultiplier, player.transform.localScale.y, player.transform.localScale.z);

        MonoBehaviour[] playerComponents = player.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour component in playerComponents)
        {
            if (component.GetType() == typeof(TouchingDirections))
            {
                continue;
            }

            component.enabled = false;
        }
        playerInfoCanvas.enabled = false;
        Rigidbody2D playerRigidbody2D = player.GetComponent<Rigidbody2D>();
        if (playerRigidbody2D != null)
        {
            playerRigidbody2D.velocity = new Vector2(0f, playerRigidbody2D.velocity.y);
        }
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }


}
