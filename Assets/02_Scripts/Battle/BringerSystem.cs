using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BringerSystem : MonoBehaviour
{
    public GameObject BringerObject;
    public GameObject BringerAttackVFX;
    public GameObject BringerAreaVFX;
    public BringerSystem OpponentBringer;
    public TextMeshProUGUI CurrentEssenceText;
    public TextMeshProUGUI LimitEssenceText;
    public Image TimeBar;

    [HideInInspector] public int MaxHealth;
    [HideInInspector] public int CurrentHealth;
    [HideInInspector] public int MaxEssence;
    [HideInInspector] public int CurrentEssence;
    [HideInInspector] public int LimitEssence;

    Animator bringerAnimator;
    [HideInInspector] public string thisPlayerTag;
    GameObject bringerAreaVFX;

    void Awake()
    {
        bringerAnimator = BringerObject.GetComponent<Animator>();

        thisPlayerTag = gameObject.tag;

        MaxEssence = 6;  // 6
        LimitEssence = 3;  // 3
        CurrentEssence = MaxEssence;
        MaxHealth = 20;
        CurrentHealth = MaxHealth;

        bringerAreaVFX = Instantiate(BringerAreaVFX);
        bringerAreaVFX.SetActive(false);

        UIManager.Instance.GenerateBringerUI(BringerObject, this);
    }
    void OnEnable() // 구독
    {
        if (thisPlayerTag == "Player")
        {
            PhaseManager.Instance.PlayerWin += WinAction;
            PhaseManager.Instance.PlayerBattleTurn += OnArea;
            PhaseManager.Instance.PlayerBattleTurn += HandleAstralAnimation;
            PhaseManager.Instance.OpponentBattleTurn += OffArea;
        }
        else if (thisPlayerTag == "Opponent")
        {
            PhaseManager.Instance.OpponentWin += WinAction;
            PhaseManager.Instance.PlayerBattleTurn += OffArea;
            PhaseManager.Instance.OpponentBattleTurn += OnArea;
            PhaseManager.Instance.OpponentBattleTurn += HandleAstralAnimation;
        }

        PhaseManager.Instance.OnPreparation += RestoreEssence;
        PhaseManager.Instance.OnPreparation += OffArea; // 준비 단계 돌입 시 애리어 끄기
    }
    void OnDisable() // 구독 해제
    {
        if (thisPlayerTag == "Player")
        {
            PhaseManager.Instance.PlayerWin -= WinAction;
            PhaseManager.Instance.PlayerBattleTurn -= OnArea;
            PhaseManager.Instance.PlayerBattleTurn -= HandleAstralAnimation;
            PhaseManager.Instance.OpponentBattleTurn -= OffArea;
        }
        else if (thisPlayerTag == "Opponent")
        {
            PhaseManager.Instance.OpponentWin -= WinAction;
            PhaseManager.Instance.PlayerBattleTurn -= OffArea;
            PhaseManager.Instance.OpponentBattleTurn -= OnArea;
            PhaseManager.Instance.OpponentBattleTurn -= HandleAstralAnimation;
        }
        PhaseManager.Instance.OnPreparation -= RestoreEssence;
        PhaseManager.Instance.OnPreparation -= OffArea; // 준비 단계 돌입 시 애리어 끄기
    }
    private void Update()
    {
        if (thisPlayerTag == "Player")
        {
            CurrentEssenceText.text = $"{CurrentEssence} / {MaxEssence}";
            LimitEssenceText.text = LimitEssence.ToString();

            if (PhaseManager.Instance.CurrentPhase == Phase.Preparation)
            {
                TimeBar.gameObject.SetActive(true);
                TimeBar.fillAmount =  PhaseManager.Instance.RemainTime / PhaseManager.Instance.preparationTime;
            }
            else if (PhaseManager.Instance.CurrentPhase == Phase.AfterPreparation)
            {
                TimeBar.gameObject.SetActive(true);
                TimeBar.fillAmount = PhaseManager.Instance.RemainTime / PhaseManager.Instance.afterPreparationTime;
            }
            else if (PhaseManager.Instance.CurrentPhase == Phase.AfterBattle)
            {
                TimeBar.gameObject.SetActive(true);
                TimeBar.fillAmount = PhaseManager.Instance.RemainTime / PhaseManager.Instance.afterBattleTime;
            }
            else
            {
                TimeBar.gameObject.SetActive(false);
            }
        }
        else if (thisPlayerTag == "Opponent")
        {
            // 현재는 Player만 존재. AI전이니까. AI는 TimeBar나 에센스 표시가 필요없다.
        }

        if (MaxEssence > 30)
        {
            MaxEssence = 30;
            CurrentEssence = MaxEssence;
        }
        if (LimitEssence > 10)
        {
            LimitEssence = 10;
        }

        if (CurrentHealth <= 0)
        {
            if (thisPlayerTag == "Player")
            {
                UIManager.Instance.OpponentWin();
            }
            else if (thisPlayerTag == "Opponent")
            {
                UIManager.Instance.PlayerWin();
            }
        }
    }
    public void WinAction()
    {
        bringerAnimator.Play("Attack", 0, 0); // Play로 즉시 시전
        Instantiate(BringerAttackVFX);

        if (thisPlayerTag == "Player")
        {
            if (PhaseManager.Instance.OnProtectedBy41001) // 기도 41001의 효과가 On일 때
            {
                StartCoroutine(Damaging(1));
                PhaseManager.Instance.OnProtectedBy41001 = false;
            }
            else
            {
                StartCoroutine(Damaging(PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral.Count)); // 남은 영체만큼 데미지 주기
            }
        }
        else if (thisPlayerTag == "Opponent")
        {
            if (PhaseManager.Instance.OnProtectedBy41001)
            {
                StartCoroutine(Damaging(1));
                PhaseManager.Instance.OnProtectedBy41001 = false;
            }
            else
            {
                StartCoroutine(Damaging(PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral.Count)); // 남은 영체만큼 데미지 주기
            }
        }
    }
    public void RestoreEssence()
    {
        MaxEssence += 2;
        LimitEssence += 1;
        CurrentEssence = MaxEssence;
    }
    public void Damaged(int damage)
    {
        CurrentHealth -= damage;
        bringerAnimator.Play("GetDown", 0, 0);
    }
    public void OnArea()
    {
        bringerAreaVFX.SetActive(true);
    }
    public void OffArea()
    {
        bringerAreaVFX.SetActive(false);
    }
    public void HandleAstralAnimation()
    {
        bringerAnimator.SetTrigger("HandleAstral");
    }
    IEnumerator Damaging(int damage)
    {
        yield return new WaitForSeconds(2.5f); // 공격이 발사되는 시간. 임시로 이렇게

        OpponentBringer.Damaged(damage);
    }
}
