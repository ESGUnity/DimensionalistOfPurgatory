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
        if (IsSpawned && PhaseManager.Instance.CurrentPhase == Phase.AfterBattle) { DoDead(); } // 만약 스폰된 녀석이라면 전투가 끝나면 파괴.
    }
    public void SetAstralInfo(Vertex thisGridVertex, CardData cardData, string thisPlayerTag)
    {
        this.thisGridVertex = thisGridVertex;
        this.thisGridVertex.AstralOnGrid = gameObject; // 그리드 정보에 자기 자신을 할당

        this.cardData = cardData;
        astralStats = new AstralStats(cardData, this); // 영체 스탯 설정

        masterPlayerTag = thisPlayerTag; // 영체 주인의 태그 설정
        gameObject.tag = masterPlayerTag; // 자기 자신의 태그 설정

        if (masterPlayerTag == "Player") // 마스터 플레이어에 따라 태그 설정
        {
            opponentTag = "Opponent";
        }
        else if (masterPlayerTag == "Opponent")
        {
            opponentTag = "Player";
        }

        PhaseManager.Instance.phaseStorageBattleInfo.AddAstralInList(gameObject); // 전장에 영체가 존재한다는 정보 전달

        astralAnimStateMachine = new AstralAnimStateMachine(GetComponent<Animator>()); // 영체 애님스테이트 할당
        astralStatusEffect = GetComponent<AstralStatusEffect>();

        if (masterPlayerTag == "Player") // PhaseManager의 영체 행동 턴 구독
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
    public void SelectAction() // 행동의 우선순위에 따라서 행동을 정한다.
    {
        if (state == State.Dead) { return; } // 만약 죽었는데 행동하려하면 즉시 리턴 (안전장치)

        int distance = SetTargetAndGetDistance();

        if (targetEnemyGridVertex == null) // 만약 적이 없는데 SelectAction이 호출되었다면 즉시 리턴 (안전장치)
        {
            DoIdle();
            Debug.Log("이 로그가 나오면 버그상태");
            return;
        }

        if (astralStats.MaxRitual != 0 && astralStats.CurrentRitual == astralStats.MaxRitual) // 의식 충족 시 소환(1순위)
        {
            DoRitual();
            return;
        }

        astralStatusEffect.DurationDecrease();

        if (astralStatusEffect.encroachmentDuration > 0) // 침식 데미지로 죽는다면 리턴
        {
            astralStats.Damaged(astralStatusEffect.encroachmentDamage);
            if (astralStats.CurrentHealth <= 0)
            {
                return;
            }
        }

        if (astralStatusEffect.stunDuration > 0) // 기절 상태라면 즉시 리턴
        {
            return;
        }

        if (astralStats.MaxMana != 0 && astralStats.CurrentMana >= astralStats.MaxMana && astralStatusEffect.sealDuration <= 0) // 마나 충족 시 능력 시전
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
        // 영체 소환
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




    public virtual void AssignProjectile() // 프로젝타일 할당(사거리 2이상 영체 해당)
    {

    }
    public virtual void SubcribeCondition() // 조건 구독(조건 영체 해당)
    {

    }
    public virtual void UnsubscribeCondition() // 조건 구독 해제(조건 영체 해당)
    {

    }
    public virtual void AddCondition() // 조건 충족 시 CurrentCondition++(조건 영체 해당)
    {
        if (astralStats != null) // 안전장치!!!
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
    public void SetAbilityTimeToAstralTurnInChild(GameObject VFX) // Position이 움직이는 애니메이션 때문에 추가함.
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
