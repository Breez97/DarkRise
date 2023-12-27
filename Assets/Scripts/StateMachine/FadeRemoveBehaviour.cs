using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeRemoveBehaviour : StateMachineBehaviour
{
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private float fadeDelay = 0.0f;
    [SerializeField] private float timeElapsed = 0f;
    private float fadeDelayElapsed = 0f; 
    SpriteRenderer spriteRenderer;
    GameObject objectToRemove;
    Color startColor;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeElapsed = 0f;
        spriteRenderer = animator.GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
        objectToRemove = animator.gameObject;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (fadeDelay > fadeDelayElapsed)
        {
            fadeDelayElapsed += Time.deltaTime;
        }
        else 
        {
            timeElapsed += Time.deltaTime;
            float newAlpha = startColor.a * (1 - (timeElapsed / fadeTime));
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

            if (timeElapsed > fadeTime)
            {
                Destroy(objectToRemove);
            }
        }
    }
}
