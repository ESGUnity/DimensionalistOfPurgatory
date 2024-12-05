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
        this.thisGridVertex.AstralOnGrid = gameObject; // 그리드 정보에 자기 자신을 할당

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

        astralAnimStateMachine = new AstralAnimStateMachine(GetComponent<Animator>()); // 영체 애님스테이트 할당
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
    public void SelectAction() // 행동의 우선순위!
    {
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

        if (astralStats.MaxMana != 0 && astralStats.CurrentMana == astralStats.MaxMana && astralStatusEffect.sealDuration <= 0) // 마나 충족 시 능력 시전
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
        // 영체 소환
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
    // 소멸의 부름이 없더라도 만약 구독을 했다면 이를 오버라이드해서 구독 해제하기
    public virtual void DeadAbility()
    {

    }




    // 영체 생산 시 사거리가 2 이상이거나 고유 능력에 VFX가 있다면 반드시 오버라이드할 것.
    // 조건이 필요하다면 구독도 하기
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
