using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum State
{
    Live,
    Dead
}

public class AstralBody : MonoBehaviour
{
    [HideInInspector] public GameObject projectile;
    [HideInInspector] public CardData cardData;
    [HideInInspector] public AstralStats astralStats;
    [HideInInspector] public AstralAnimStateMachine astralAnimStateMachine;
    [HideInInspector] public AstralStatusEffect astralStatusEffect;

    [HideInInspector] public Vertex thisGridVertex;
    Vertex targetEnemyGridVertex;
    [HideInInspector] public string masterPlayerTag;
    string opponentTag;
    [HideInInspector] public State state;
    [HideInInspector] public bool IsSpawned = false;

    private void Awake()
    {
        state = State.Live;
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
    private void Update()
    {
        if (IsSpawned && PhaseManager.Instance.CurrentPhase == Phase.AfterBattle) { DoDead(); } // ���� ������ �༮�̶�� ������ ������ �ı�.
    }
    public void SetAstralInfo(Vertex thisGridVertex, CardData cardData, string thisPlayerTag)
    {
        this.thisGridVertex = thisGridVertex;
        this.thisGridVertex.AstralOnGrid = gameObject; // �׸��� ������ �ڱ� �ڽ��� �Ҵ�

        this.cardData = cardData;
        astralStats = new AstralStats(cardData, this); // ��ü ���� ����

        masterPlayerTag = thisPlayerTag; // ��ü ������ �±� ����
        gameObject.tag = masterPlayerTag; // �ڱ� �ڽ��� �±� ����

        if (masterPlayerTag == "Player") // ������ �÷��̾ ���� �±� ����
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

        if (masterPlayerTag == "Player") // PhaseManager�� ��ü �ൿ �� ����
        {
            PhaseManager.Instance.PlayerBattleTurn += SelectAction;
        }
        else if (masterPlayerTag == "Opponent")
        {
            PhaseManager.Instance.OpponentBattleTurn += SelectAction;
        }

        UIManager.Instance.GenerateAstralUI(gameObject);

        SubcribeCondition();
        AssignProjectile();
    }
    public void SelectAction() // �ൿ�� �켱������ ���� �ൿ�� ���Ѵ�.
    {
        if (state == State.Dead) { return; } // ���� �׾��µ� �ൿ�Ϸ��ϸ� ��� ���� (������ġ)

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

        if (astralStats.MaxMana != 0 && astralStats.CurrentMana >= astralStats.MaxMana && astralStatusEffect.sealDuration <= 0) // ���� ���� �� �ɷ� ����
        {
            DoManaAbility();
            return;
        }

        if (astralStats.MaxCondition != 0 && astralStats.CurrentCondition >= astralStats.MaxCondition && astralStatusEffect.sealDuration <= 0)
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
        astralStats.CurrentMana = 0;
        ManaAbility();
        astralAnimStateMachine.OnManaAbility();
    }
    void DoConditionAbility()
    {
        astralStats.CurrentCondition = 0;
        ConditionAbility();
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
        Vertex MoveVertex = GridManager.Instance.DecideNextMoveVertex(thisGridVertex, targetEnemyGridVertex);
        if (MoveVertex == null || MoveVertex == thisGridVertex)
        {
            return;
        }
        else
        {
            MoveVertex.AstralOnGrid = gameObject;
            thisGridVertex.AstralOnGrid = null;
            thisGridVertex = MoveVertex;
        }

        transform.DOMove(MoveVertex.Coordinate, 1f / astralAnimStateMachine.moveAnimationClipLength).SetEase(Ease.Linear);
        astralAnimStateMachine.OnMove();
        transform.LookAt(MoveVertex.Coordinate);
    }
    public void DoDead()
    {
        state = State.Dead;

        UnsubscribeCondition();

        if (astralStatusEffect.sealDuration <= 0)
        {
            DeadAbility();
        }

        astralStatusEffect.DoneSlain();

        PhaseManager.Instance.phaseStorageBattleInfo.RemoveAstralInList(gameObject);
        Destroy(gameObject, 0.05f);
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




    public virtual void AssignProjectile() // ������Ÿ�� �Ҵ�(��Ÿ� 2�̻� ��ü �ش�)
    {

    }
    public virtual void SubcribeCondition() // ���� ����(���� ��ü �ش�)
    {

    }
    public virtual void UnsubscribeCondition() // ���� ���� ����(���� ��ü �ش�)
    {

    }
    public virtual void AddCondition() // ���� ���� �� CurrentCondition++(���� ��ü �ش�)
    {
        if (astralStats != null) // ������ġ!!!
        {
            astralStats.CurrentCondition++;
        }
    }
    public void SetAbilityTimeToAstralTurn(GameObject VFX)
    {
        PhaseManager.Instance.SetAstralActionTerm(GetVFXLength(VFX) + 0.1f);
    }
    public void AutoDestroyVFX(GameObject VFX)
    {
        Destroy(VFX, GetVFXLength(VFX));
    }
    public float GetVFXLength(GameObject VFX)
    {
        return VFX.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length;
    }
    public void SetAbilityTimeToAstralTurnInChild(GameObject VFX) // Position�� �����̴� �ִϸ��̼� ������ �߰���.
    {
        PhaseManager.Instance.SetAstralActionTerm(GetVFXLengthInChild(VFX) + 0.1f);
    }
    public void AutoDestroyVFXInChild(GameObject VFX)
    {
        Destroy(VFX, GetVFXLengthInChild(VFX));
    }
    public float GetVFXLengthInChild(GameObject VFX)
    {
        return VFX.GetComponentInChildren<Animator>().runtimeAnimatorController.animationClips[0].length;
    }

    IEnumerator MeleeCoroutine()
    {
        yield return new WaitForSeconds((1f / astralAnimStateMachine.attackAnimationClipLength) / 2f);
        if (targetEnemyGridVertex.AstralOnGrid != null)
        {
            targetEnemyGridVertex.AstralOnGrid.GetComponent<AstralBody>().astralStats.Damaged(astralStats.Damage);

            if (astralStats != null && astralStats.MaxMana != 0)
            {
                astralStats.CurrentMana += 10;
            }
        }
    }

    IEnumerator RangeCoroutine()
    {
        yield return new WaitForSeconds((1f / astralAnimStateMachine.attackAnimationClipLength) / 2f);

        if (targetEnemyGridVertex.AstralOnGrid != null)
        {
            Instantiate(projectile).GetComponent<AstralProjectile>().SetTarget(targetEnemyGridVertex.AstralOnGrid, this);

            if (astralStats != null && astralStats.MaxMana != 0)
            {
                astralStats.CurrentMana += 10;
            }
        }
    }
}
