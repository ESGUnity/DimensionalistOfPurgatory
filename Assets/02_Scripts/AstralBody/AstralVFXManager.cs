using UnityEngine;

public class AstralVFXManager : MonoBehaviour
{
    public GameObject StunEffect;
    public GameObject DeclainEffect;
    public GameObject SealEffect;
    public GameObject PunishEffect;
    public GameObject DeadEffect;

    public GameObject P10002;
    public GameObject C10003;

    static AstralVFXManager instance;
    public static AstralVFXManager Instance { get { return instance; } }
    private void Awake()
    {
        instance = this;
    }
}
