using UnityEngine;
public enum AstralState // [Temp] ���� ��������� [Temp]
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
    AstralState astralState; // ���� ��������� [Temp]
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
    public void SelectAction() // �ൿ�� �켱����!
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
        // �׸��� �ý��ۿ� ���� ������Ʈ�� �׸��� ���ؽ�(�� ������Ʈ ��ü�� ������ �׸��� �ý��� �ʿ��� ��ġ�ϰ� �����)�� �±� �Ѱܼ� Ÿ�� ��ġ ã��/ �ƴϸ� �÷��̽���Ʈ�ý��ۿ��� ���ʿ� �׸��� ���ؽ� �������� �ƽ�Ʈ�� �ٵ�� �ѱ��(�̰� ������ ����ؼ� �������ϴϱ� �ǳ� ã�� ����� �δ�ɵ� ����)
        // �̵� �� ���ݿ��θ� ������ �޼���� ���� ����
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
        // �ִϸ����Ϳ��� ���������Ƽ �ִϸ��̼� Ŭ���� �ð� ��������
        // PhaseManager.Instance.RemainTurnTime(�ִϸ��̼� Ŭ�� �ð�)�� �ϰ� Ŭ�� �ð��� RemainTurnTime���� ũ�ٸ� Ŭ�� �ð��� RemainTurnTime�� �Ҵ�
        // PhaseManager�� �׸��� �ý��ۿ��� ��ü ������ �޾� �� ���� ��ü�� �����ϸ� if������ battlePhase �ڷ�ƾ ��� �����Ű��
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
