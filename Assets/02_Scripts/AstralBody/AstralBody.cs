using DG.Tweening;
using System.Collections;
using UnityEngine;

public class AstralBody : MonoBehaviour
{
    [HideInInspector] public GameObject projectile;
    CardData cardData;
    [HideInInspector] public AstralStats astralStats;
    AstralAnimStateMachine astralAnimStateMachine;
    [HideInInspector] public AstralStatusEffect astralStatusEffect;

    [HideInInspector] public Vertex thisGridVertex;
    Vertex targetEnemyGridVertex;
    [HideInInspector] public string masterPlayerTag;
    string opponentTag;

    private void Awake()
    {
        
    }
    private void OnEnable()
    {

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
    void Start()
    {
        SetProjectileAndAbilityVFXAndObserving();
    }
    private void Update()
    {
        
    }
    public void SetAstralInfo(Vertex thisGridVertex, CardData cardData, string thisPlayerTag)
    {
        this.thisGridVertex = thisGridVertex;
        this.thisGridVertex.AstralOnGrid = gameObject; // �׸��� ������ �ڱ� �ڽ��� �Ҵ�

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

        astralAnimStateMachine = new AstralAnimStateMachine(GetComponent<Animator>()); // ��ü �ִԽ�����Ʈ �Ҵ�
        astralStatusEffect = GetComponent<AstralStatusEffect>();

        if (masterPlayerTag == "Player")
        {
            PhaseManager.Instance.PlayerBattleTurn += SelectAction;
        }
        else if (masterPlayerTag == "Opponent")
        {
            PhaseManager.Instance.OpponentBattleTurn += SelectAction;
        }
    }
    public void SelectAction() // �ൿ�� �켱����!
    {
        int distance = SetTargetAndGetDistance();
        if (targetEnemyGridVertex == null) // ���� ���� ���µ� SelectAction�� ȣ��Ǿ��ٸ� ��� ���� (������ġ)
        {
            DoIdle();
            Debug.Log("�� �αװ� ������ ���׻���");
            return;
        }

        if (astralStats.MaxRitual != 0 && astralStats.CurrentRitual == astralStats.MaxRitual) // �ǽ� ���� �� ��ȯ(1����)
        {
            DoRitual();
            return;
        }

        astralStatusEffect.DurationDecrease();

        if (astralStatusEffect.encroachmentDuration > 0) // ħ�� �������� �״´ٸ� ����
        {
            astralStats.Damaged(astralStatusEffect.encroachmentDamage);
            if (astralStats.CurrentHealth <= 0)
            {
                return;
            }
        }

        if (astralStatusEffect.stunDuration > 0) // ���� ���¶�� ��� ����
        {
            return;
        }

        if (astralStats.MaxMana != 0 && astralStats.CurrentMana == astralStats.MaxMana && astralStatusEffect.sealDuration <= 0) // ���� ���� �� �ɷ� ����
        {
            DoManaAbility();
            return;
        }

        if (astralStats.MaxCondition != 0 && astralStats.CurrentCondition == astralStats.MaxCondition && astralStatusEffect.sealDuration <= 0)
        {
            DoConditionAbility();
            return;
        }

        if (astralStats.Range >= distance)
        {
            if (astralStats.Range == 1)
            {
                DoMeleeAttack();
            }
            else if (astralStats.Range > 1)
            {
                DoRangeAttack();
            }
        }
        else
        {
            DoMove();
        }
    }
    int SetTargetAndGetDistance()
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
        astralAnimStateMachine.OnManaAbility();
    }
    void DoConditionAbility()
    {
        ConditionAbility();
        astralStats.CurrentCondition = 0;
        astralAnimStateMachine.OnConditionAbility();
    }
    void DoMeleeAttack()
    {
        StartCoroutine(MeleeCoroutine());
        astralAnimStateMachine.OnAttack();
        transform.LookAt(targetEnemyGridVertex.Coordinate);
    }
    void DoRangeAttack()
    {
        StartCoroutine(RangeCoroutine());

        astralAnimStateMachine.OnAttack();
        transform.LookAt(targetEnemyGridVertex.Coordinate);

    }
    void DoMove()
    {
        if (GridManager.Instance.Grids.Vertices.Contains(thisGridVertex))
        {
            thisGridVertex.Alram();
        }

        Vertex MoveVertex = GridManager.Instance.DecideNextMoveVertex(thisGridVertex, targetEnemyGridVertex);
        MoveVertex.AstralOnGrid = gameObject;
        thisGridVertex.AstralOnGrid = null;
        thisGridVertex = MoveVertex;

        transform.DOMove(MoveVertex.Coordinate, 1f / astralAnimStateMachine.moveAnimationClipLength).SetEase(Ease.Linear);
        astralAnimStateMachine.OnMove();
        transform.LookAt(MoveVertex.Coordinate);

        if (GridManager.Instance.Grids.Vertices.Contains(thisGridVertex))
        {
            thisGridVertex.Alram();
        }
    }
    public void DoDead()
    {
        if (astralStatusEffect.sealDuration <= 0)
        {
            DeadAbility();
        }

        astralStatusEffect.DoneSlain();

        PhaseManager.Instance.phaseStorageBattleInfo.RemoveAstralInList(gameObject);
        Destroy(gameObject, 0.2f);
    }
    public virtual void ManaAbility()
    {

    }

    public virtual void ConditionAbility()
    {

    }
    // �Ҹ��� �θ��� ������ ���� ������ �ߴٸ� �̸� �������̵��ؼ� ���� �����ϱ�
    public virtual void DeadAbility()
    {

    }




    // ��ü ���� �� ��Ÿ��� 2 �̻��̰ų� ���� �ɷ¿� VFX�� �ִٸ� �ݵ�� �������̵��� ��.
    // ������ �ʿ��ϴٸ� ������ �ϱ�
    public virtual void SetProjectileAndAbilityVFXAndObserving()
    {

    }

    public void SetAstralActionTurnForAbility(GameObject VFX)
    {
        PhaseManager.Instance.SetAstralActionTerm(VFX.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);
    }

    IEnumerator MeleeCoroutine()
    {
        yield return new WaitForSeconds((1f / astralAnimStateMachine.attackAnimationClipLength) / 2f);

        targetEnemyGridVertex.AstralOnGrid.GetComponent<AstralBody>().astralStats.Damaged(astralStats.Damage);
    }

    IEnumerator RangeCoroutine()
    {
        yield return new WaitForSeconds((1f / astralAnimStateMachine.attackAnimationClipLength) / 2f);

        Instantiate(projectile).GetComponent<AstralProjectile>().SetTarget(targetEnemyGridVertex.AstralOnGrid, this);
    }
}
