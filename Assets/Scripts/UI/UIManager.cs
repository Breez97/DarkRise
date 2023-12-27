using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;
    public CanvasGroup magicSpellsMenu;
    public RectTransform spellsBar;
    private Vector3 defaultSpellsBarScale;
    [SerializeField] private Vector3 targetSpellsBarScale;
    private DG.Tweening.Sequence currentMagicSpellTween;


    private void Awake()
    {
        QualitySettings.vSyncCount = 1;  // Отключает вертикальную синхронизацию
        Application.targetFrameRate = 144;
        defaultSpellsBarScale = spellsBar.transform.localScale;
    }

    public Canvas gameCanvas;

    private void OnEnable()
    {
        CharacterEvents.characterDamaged += CharacterTookDamage;
        CharacterEvents.characterHealed += CharacterHealed;
        CharacterEvents.showMagicSpellsMenu += ShowMagicSpellsMenu;
        CharacterEvents.closeMagicSpellsMenu += CloseMagicSpellsMenu;
    }

    private void OnDisable()
    {
        CharacterEvents.characterDamaged -= CharacterTookDamage;
        CharacterEvents.characterHealed -= CharacterHealed;
        CharacterEvents.showMagicSpellsMenu -= ShowMagicSpellsMenu;
        CharacterEvents.closeMagicSpellsMenu = CloseMagicSpellsMenu;
    }

    public void CharacterTookDamage(GameObject character, int damageReceived)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = damageReceived.ToString();
    }

    public void CharacterHealed(GameObject character, int healthRestored)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = healthRestored.ToString();
    }

    public void ShowMagicSpellsMenu()
    {
        Debug.Log("show");
        magicSpellsMenu.interactable = true;
        currentMagicSpellTween.Pause();
        currentMagicSpellTween?.Kill();
        magicSpellsMenu.gameObject.SetActive(true);

        currentMagicSpellTween = DOTween.Sequence()
            .Join(magicSpellsMenu.DOFade(1f, 0.5f))
            .Join(spellsBar.DOScale(targetSpellsBarScale, 0.5f));



    }

    public void CloseMagicSpellsMenu()
    {
        magicSpellsMenu.interactable = false;
        currentMagicSpellTween.Pause();
        currentMagicSpellTween?.Kill();
        Debug.Log("close");
        currentMagicSpellTween = DOTween.Sequence()
                .Join(magicSpellsMenu.DOFade(0, 0.5f))
                .Join(spellsBar.DOScale(defaultSpellsBarScale, 0.5f))
                .OnComplete(() =>
                {
                    if (!magicSpellsMenu.interactable)
                    {
                        magicSpellsMenu.gameObject.SetActive(false);
                    }


                });

    }


}
