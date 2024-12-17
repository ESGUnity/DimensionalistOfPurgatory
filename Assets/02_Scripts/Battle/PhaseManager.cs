using System;
using System.Collections;
using UnityEngine;

public enum Phase 
{
    Preparation, 
    AfterPreparation,
    Battle,
    AfterBattle 
}
public class PhaseManager : MonoBehaviour
{
    public event Action OnInitialize;
    public event Action OnAfterPreparation; // HandSystem에서 구독
    public event Action OnPreparation; // HandSystem에서 구독
    public event Action PlayerBattleTurn; // AstralBody에서 구독
    public event Action OpponentBattleTurn; // AstralBody에서 구독
    public event Action PlayerWin;
    public event Action OpponentWin;
    public PhaseStorageBattleInfo phaseStorageBattleInfo;



    [HideInInspector] public Phase CurrentPhase;
    [HideInInspector] public float RemainTime;

    public float initializeTime;
    public float afterPreparationTime;
    public float afterBattleTime;
    public float preparationTime;
    float battleTime = 5f; // 필요없다.
    string strikeFirst = "Player"; // 우선 선공권은 Player에게.
    float remainTurnTime = 1;
    public bool OnProtectedBy41001;

    static PhaseManager instance;
    public static PhaseManager Instance { get { return instance; } }
    private void Awake()
    {
        instance = this;

        phaseStorageBattleInfo = new PhaseStorageBattleInfo();
    }
    void Start()
    {
        initializeTime = 6f; // 변수가 public이어서 별도로 할당해야한다.
        afterPreparationTime = 2f;
        afterBattleTime = 6f;
        preparationTime = 30f;
        OnProtectedBy41001 = false;
        StartCoroutine(InitializeGame());
    }

    void Update()
    {
        
    }
    IEnumerator InitializeGame()
    {
        CurrentPhase = Phase.AfterBattle;

        yield return new WaitForSeconds(initializeTime);

        OnInitialize?.Invoke();

        StartCoroutine(PreparationPhase());
    }
    IEnumerator PreparationPhase()
    {
        OnPreparation?.Invoke();

        CurrentPhase = Phase.Preparation;

        foreach (GameObject go in phaseStorageBattleInfo.PlayerAstralOriginPos.Keys) // 살아있는 영체를 원래 위치로 되돌리기
        {
            if (!go.GetComponent<AstralBody>().thisGridVertex.Equals(phaseStorageBattleInfo.PlayerAstralOriginPos[go]))
            {
                phaseStorageBattleInfo.PlayerAstralOriginPos[go].AstralOnGrid = go;
                go.transform.position = phaseStorageBattleInfo.PlayerAstralOriginPos[go].Coordinate;
                go.GetComponent<AstralBody>().thisGridVertex.AstralOnGrid = null;
                go.GetComponent<AstralBody>().thisGridVertex = phaseStorageBattleInfo.PlayerAstralOriginPos[go];
                go.transform.eulerAngles = Vector3.zero;
            }
            else // 이동하지 않았다면 고개만 돌리기
            {
                go.transform.eulerAngles = Vector3.zero;
            }
        }

        // AI는 잘 작동 안한다.
        //foreach (GameObject go in phaseStorageBattleInfo.OpponentAstralOriginPos.Keys)
        //{
        //    go.GetComponent<AstralBody>().thisGridVertex.AstralOnGrid = null;
        //    phaseStorageBattleInfo.OpponentAstralOriginPos[go].AstralOnGrid = go;
        //    go.transform.position = phaseStorageBattleInfo.OpponentAstralOriginPos[go].Coordinate;
        //    go.GetComponent<AstralBody>().thisGridVertex = phaseStorageBattleInfo.OpponentAstralOriginPos[go];
        //    go.transform.eulerAngles = new Vector3(0, 180, 0);
        //}


        RemainTime = preparationTime;
        while (RemainTime > 0)
        {
            RemainTime -= Time.deltaTime;
            yield return null;
        }

        StartCoroutine(AfterPreparationPhase());
    }
    IEnumerator AfterPreparationPhase()
    {
        CurrentPhase = Phase.AfterPreparation;
        OnAfterPreparation?.Invoke();

        RemainTime = afterPreparationTime;
        while (RemainTime > 0)
        {
            RemainTime -= Time.deltaTime;
            yield return null;
        }

        StartCoroutine(BattlePhase());
    }
    IEnumerator BattlePhase()
    {
        CurrentPhase = Phase.Battle;

        bool isPlayerTurn = strikeFirst != "Player"; // 이전 선공자가 Player였는지 아닌지를 반환

        while (!phaseStorageBattleInfo.EndBattlePhase())
        {
            if (isPlayerTurn)
            {
                OpponentBattleTurn?.Invoke();
            }
            else
            {
                PlayerBattleTurn?.Invoke();
            }

            while (remainTurnTime > 0)
            {
                remainTurnTime -= Time.deltaTime;
                yield return null;
            }

            remainTurnTime = 0.1f;
            isPlayerTurn ^= true;
        }
        // 승자에 따른 전투 단계 결과
        if (phaseStorageBattleInfo.DecideWinner() == "Draw")
        {

        }
        else if (phaseStorageBattleInfo.DecideWinner() == "Player")
        {
            PlayerWin?.Invoke();
        }
        else if (phaseStorageBattleInfo.DecideWinner() == "Opponent")
        {
            OpponentWin?.Invoke();
        } 
        // 선공자 바꾸기
        if (strikeFirst == "Player")
        {
            strikeFirst = "Opponent";
        }
        else
        {
            strikeFirst = "Player";
        }

        StartCoroutine(AfterBattlePhase());
    }
    IEnumerator AfterBattlePhase()
    {
        CurrentPhase = Phase.AfterBattle;

        RemainTime = afterBattleTime;
        while (RemainTime > 0)
        {
            RemainTime -= Time.deltaTime;
            yield return null;
        }


        StartCoroutine(PreparationPhase());
    }


    public void SetAstralActionTerm(float clipLength)
    {
        if (clipLength > remainTurnTime)
        {
            remainTurnTime = clipLength;
        }
    }

    public bool SealPray()
    {
        if ((phaseStorageBattleInfo.PlayerAstral.Find(v => v.GetComponent<AstralBody>().cardData.Id == 40002) != null && 
            phaseStorageBattleInfo.PlayerAstral.Find(v => v.GetComponent<AstralBody>().cardData.Id == 40002).GetComponent<AstralBody>().astralStatusEffect.sealDuration <= 0) ||
            (phaseStorageBattleInfo.OpponentAstral.Find(v => v.GetComponent<AstralBody>().cardData.Id == 40002) != null &&
            phaseStorageBattleInfo.OpponentAstral.Find(v => v.GetComponent<AstralBody>().cardData.Id == 40002).GetComponent<AstralBody>().astralStatusEffect.sealDuration <= 0)) // 40002는 데모크렉스 탄압자로, 모든 인도자가 기도를 시전할 수 없게 막는 역할을 한다. HandSystem에서 호출.
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
