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
        thisGridVertex.AstralOnGrid = gameObject; // �׸��� ������ �ڱ� �ڽ��� �Ҵ�

        this.cardData = cardData;
        astralStats = new AstralStats(cardData, this); // ��ü ���� ����

        masterPlayerTag = thisPlayerTag; // ��ü ������ �±� ����
        gameObject.tag = masterPlayerTag; // �ڱ� �ڽ��� �±� ����

        if (masterPlayerTag == "Player")
        {
            opponentTag = "Opponent";
        }
        else if (masterPlayerTag == "Opponent")
        {
            opponentTag = "Player";
        }

        PhaseManager.Instance.phaseStorageBattleInfo.AddAstralInList(gameObject); // ���忡 ��ü�� �����Ѵٴ� ���� ����

        astralAnimStateMachine = new AstralAnimStateMachine(GetComponent<Animator>());
    }
    public void SelectAction() // �ൿ�� �켱����!
    {
        int distance = SetTargetPosition();
        if (targetEnemyGridVertex == null) // ���� ���� ���µ� SelectAction�� ȣ��Ǿ��ٸ� ��� ����
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
        // ��ü ��ȯ
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

        // Dead Anim �޾Ƽ� Ŭ�� �ð���ŭ PhaseManager�� ������
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
