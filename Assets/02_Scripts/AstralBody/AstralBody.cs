using UnityEngine;

public class AstralBody : MonoBehaviour
{
    CardData cardData;
    AstralStats astralStats;
    AstralAnimStateMachine astralAnimStateMachine;

    public Vertex thisGridVertex;
    Vertex targetEnemyGridVertex;
    string masterPlayerTag;
    string opponentTag;

    private void Awake()
    {
        
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
    public void SetAstralInfo(Vertex thisGridVertex, CardData cardData, string thisPlayerTag)
    {
        this.thisGridVertex = thisGridVertex;
        thisGridVertex.AstralOnGrid = gameObject; // 그리드 정보에 자기 자신을 할당

        this.cardData = cardData;
        astralStats = new AstralStats(cardData, this); // 영체 스탯 설정

        masterPlayerTag = thisPlayerTag; // 영체 주인의 태그 설정
        gameObject.tag = masterPlayerTag; // 자기 자신의 태그 설정

        if (masterPlayerTag == "Player")
        {
            opponentTag = "Opponent";
        }
        else if (masterPlayerTag == "Opponent")
        {
            opponentTag = "Player";
        }

        PhaseManager.Instance.phaseStorageBattleInfo.AddAstralInList(gameObject); // 전장에 영체가 존재한다는 정보 전달

        astralAnimStateMachine = new AstralAnimStateMachine(GetComponent<Animator>());
    }
    public void SelectAction() // 행동의 우선순위!
    {
        int distance = SetTargetPosition();
        if (targetEnemyGridVertex == null) // 만약 적이 없는데 SelectAction이 호출되었다면 즉시 리턴
        {
            return;
        }

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

    int SetTargetPosition()
    {
        targetEnemyGridVertex = GridManager.Instance.FindTargetVertex(thisGridVertex, opponentTag, out int depth);

        return depth;
    }



    void DoIdle()
    {
        astralAnimStateMachine.OnIdle();
    }
    void DoRitual()
    {
        // 영체 소환
        astralAnimStateMachine.OnRitual();
    }
    void DoManaAbility()
    {
        ManaAbility();
        astralStats.CurrentMana = 0;
    }
    void DoConditionAbility()
    {
        ConditionAbility();
        astralStats.CurrentCondition = 0;
    }


    void DoMeleeAttack()
    {

    }

    void DoRangeAttack()
    {

    }

    void DoMove()
    {

    }

    void DoDead()
    {
        DeadAbility();
        PhaseManager.Instance.phaseStorageBattleInfo.RemoveAstralInList(gameObject);

        // Dead Anim 받아서 클립 시간만큼 PhaseManager로 보내기
    }

    public virtual void FillCondition()
    {

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
