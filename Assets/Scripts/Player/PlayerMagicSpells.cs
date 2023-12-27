using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum MagicSpells
{
    Fire,
    Wind,
    Shield,
    TimeSlow
}
public class PlayerMagicSpells : MonoBehaviour
{
    public MagicSpells currentSpell;
    [SerializeField] private float shieldSpellTime = 20f;
    [SerializeField] private int countSpellsInInterval = 3;
    private int currentSpellsCount = 0;
    [SerializeField] private float spellsDelay  = 60f;
    [SerializeField] private float damageDecreaseMultiplier = 0.5f;
    public float DamageDecreaseMultiplier { get => damageDecreaseMultiplier; }
    [SerializeField] private GameObject playerShield;
    [SerializeField] private UnityEvent onShieldActivate;
    public UnityEvent onShieldDeactivate;
    [SerializeField] private GameObject playerTimeSlowEffect;
    [SerializeField] private float timeSlowSpellTime = 5f;
    [SerializeField] private float timeSlowTimeScale = 0.7f;
    private bool canMagic = true;
    public bool CanMagic { get =>  canMagic; }
    private Dictionary<MagicSpells, string> spellNames = new Dictionary<MagicSpells, string>() 
    { 
        { MagicSpells.Fire, "isFireSpell" }, 
        { MagicSpells.Wind, "isWindSpell" }, 
        { MagicSpells.Shield, "isShieldSpell" },
        { MagicSpells.TimeSlow, "isTimeSlowSpell" }
    };
  
    public string GetMagicSpell()
    {
        string magicSpellName;
        switch (currentSpell) {
            case MagicSpells.Fire:
                magicSpellName = spellNames[MagicSpells.Fire];
                break;
            case MagicSpells.Wind:
                magicSpellName = spellNames[MagicSpells.Wind];
                break;
            case MagicSpells.Shield:
                magicSpellName = spellNames[MagicSpells.Shield];
                break;
            case MagicSpells.TimeSlow:
                magicSpellName = spellNames[MagicSpells.TimeSlow];
                break;
            default:
                magicSpellName = spellNames[MagicSpells.Fire];
                break;

        }
        return magicSpellName;
    }

    public IEnumerable<string> GetSpellNames()
    {
        return spellNames.Values;

    }

    public void OnShieldSpell()
    {
        playerShield.SetActive(true);
        onShieldActivate?.Invoke();
        StartCoroutine(ShieldActivate());
    }
    private IEnumerator ShieldActivate()
    {
        
        yield return new WaitForSeconds(shieldSpellTime);
        onShieldDeactivate?.Invoke();    
        playerShield.SetActive(false);  
        
    }
    public void OnMagicSpellActivate()
    {
        ++currentSpellsCount;
        if (currentSpellsCount == countSpellsInInterval)
        {
            canMagic = false;
            StartCoroutine(OnOutOfMagicSpells());   

        }
        
    }
    private IEnumerator OnOutOfMagicSpells()
    {
        yield return new WaitForSeconds(spellsDelay);
        canMagic = true;
        currentSpellsCount = 0;
    }

    public void OnTimeSlowSpell()
    {

        playerTimeSlowEffect.SetActive(true);
        StartCoroutine(TimeSlowActivate());
    }
    private IEnumerator TimeSlowActivate()
    {
        
        yield return new WaitForSeconds(timeSlowSpellTime);
        playerTimeSlowEffect.SetActive(false);
        Time.timeScale = 1f;

    }
    private void SlowTimeScale()
    {
        Time.timeScale = timeSlowTimeScale;
    }

}
