using UnityEngine;

public class AstralVFXManager : MonoBehaviour
{
    public GameObject StunEffect;
    public GameObject StunVFX;
    public GameObject DeclainEffect;
    public GameObject DeclainVFX;
    public GameObject SealEffect;
    public GameObject SealVFX;
    public GameObject IncreaseDamageEffect;
    public GameObject InvincibilityEffect;
    public GameObject InvincibilityVFX;
    public GameObject HealEffect;
    public GameObject PunishEffect;
    public GameObject DeadEffect;
    public GameObject SpawnEffect;
    public GameObject CleanseEffect;

    public GameObject P10002;
    public GameObject C10003;
    public GameObject P20001;
    public GameObject M20002;
    public GameObject P40001;
    public GameObject P40003;
    public GameObject P50002;
    public GameObject P50003;
    public GameObject P80001;
    public GameObject M80001;
        
    public Material SpawnedMaterial;

    static AstralVFXManager instance;
    public static AstralVFXManager Instance { get { return instance; } }
    private void Awake()
    {
        instance = this;
    }
}
