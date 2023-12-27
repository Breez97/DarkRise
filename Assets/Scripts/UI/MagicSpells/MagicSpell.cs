using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MagicSpell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private MagicSpells magicSpell;
    private PlayerMagicSpells playerMagicSpell; 
    private Vector3 defaultScale;

    private void Awake()
    {
        playerMagicSpell = FindFirstObjectByType<PlayerMagicSpells>();
        this.defaultScale = transform.localScale;   
    }
    public void OnPointerClick(PointerEventData eventData)
    {

        playerMagicSpell.currentSpell = magicSpell;
        CharacterEvents.closeMagicSpellsMenu.Invoke();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        DOTween.Play(transform.DOScale(transform.localScale * 1.3f, 0.4f));

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DOTween.Play(transform.DOScale(defaultScale, 0.4f));
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
