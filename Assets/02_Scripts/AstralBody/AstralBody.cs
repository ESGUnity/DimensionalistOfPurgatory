using UnityEngine;
public enum AstralState // [Temp] 거의 쓸모없긴함 [Temp]
{
    Idle,
    Move,
    Attack,
    Ability,
    Dead
}

public class AstralBody : MonoBehaviour
{
    CardData cardData;
    Vertex gridVertex;
    AstralState astralState; // 거의 쓸모없긴함 [Temp]
    string masterPlayerTag;
    AstralStats astralStats;
    Animator animator;


    private void Awake()
    {
        masterPlayerTag = gameObject.tag;
        astralStats = GetComponent<AstralStats>();
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        if (masterPlayerTag == "Player")
        {
            PhaseManager.Instance.PlayerBattleTurn += SelectAction;
        }
        else if (masterPlayerTag == "Opponent")
        {
            PhaseManager.Instance.OpponentBattleTurn += SelectAction;
        }
    }
    private void OnDisable()
    {
        if (masterPlayerTag == "Player")
        {
            PhaseManager.Instance.PlayerBattleTurn -= SelectAction;
        }
        else if (masterPlayerTag == "Opponent")
        {
            PhaseManager.Instance.OpponentBattleTurn -= SelectAction;
        }
    }
    private void Start()
    {

    }
    private void Update()
    {
        
    }
    public void SetAstralInfo(Vertex gridVertex, CardData cardData)
    {
        this.gridVertex = gridVertex;
        this.cardData = cardData;
        astralStats.SetAstralStats(cardData);
    }
    public void SelectAction() // 행동의 우선순위!
    {
        if (astralStats.MaxRitual != 0 && astralStats.CurrentRitual == astralStats.MaxRitual)
        {
            DoRitual();
            return;
        }

        if (astralStats.MaxMana != 0 && astralStats.CurrentMana == astralStats.MaxMana)
        {
            DoManaAbility();
            return;
        }

        if (astralStats.MaxCondition != 0 && astralStats.CurrentCondition == astralStats.MaxCondition)
        {
            DoConditionAbility();
            return;
        }
    }

    void RequestTargetPosition()
    {
        // 그리드 시스템에 현재 오브젝트의 그리드 버텍스(이 오브젝트 자체를 보내서 그리드 시스템 쪽에서 서치하게 만들기)와 태그 넘겨서 타겟 위치 찾기/ 아니면 플레이스먼트시스템에서 애초에 그리드 버텍스 정보까지 아스트랄 바디로 넘기기(이게 좋을듯 계속해서 보내야하니까 맨날 찾게 만들면 부담될듯 ㅇㅋ)
        // 이동 및 공격여부를 따지는 메서드는 별도 생성
    }




    void DoRitual()
    {

    }
    void DoIdle()
    {

    }
    void DoMove()
    {

    }

    void DoMeleeAttack()
    {

    }

    void DoRangeAttack()
    {

    }

    void DoManaAbility()
    {
        // 애니메이터에서 마나어빌리티 애니메이션 클립의 시간 가져오기
        // PhaseManager.Instance.RemainTurnTime(애니메이션 클립 시간)을 하고 클립 시간이 RemainTurnTime보다 크다면 클립 시간을 RemainTurnTime에 할당
        // PhaseManager는 그리드 시스템에서 영체 정보를 받아 한 쪽의 영체가 전멸하면 if문으로 battlePhase 코루틴 즉시 종료시키기
        ManaAbility();
        astralStats.CurrentMana = 0;
    }
    void DoConditionAbility()
    {
        ConditionAbility();
        astralStats.CurrentCondition = 0;
    }
    void DoDead()
    {
        DeadAbility();
    }


    public virtual void ManaAbility()
    {

    }

    public virtual void ConditionAbility()
    {

    }
    public virtual void DeadAbility()
    {

    }
}
