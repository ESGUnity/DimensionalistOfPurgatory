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
    public event Action OnAfterPreparation; // HandSystem���� ����
    public event Action OnPreparation; // HandSystem���� ����
    public event Action PlayerBattleTurn; // AstralBody���� ����
    public event Action OpponentBattleTurn; // AstralBody���� ����
    public event Action PlayerWin;
    public event Action OpponentWin;
    public PhaseStorageBattleInfo phaseStorageBattleInfo;



    [HideInInspector] public Phase CurrentPhase;
    [HideInInspector] public float RemainTime;

    public float initializeTime;
    public float afterPreparationTime;
    public float afterBattleTime;
    public float preparationTime;
    float battleTime = 5f; // �ʿ����.
    string strikeFirst = "Player"; // �켱 �������� Player����.
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
        initializeTime = 6f; // ������ public�̾ ������ �Ҵ��ؾ��Ѵ�.
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

        foreach (GameObject go in phaseStorageBattleInfo.PlayerAstralOriginPos.Keys) // ����ִ� ��ü�� ���� ��ġ�� �ǵ�����
        {
            if (!go.GetComponent<AstralBody>().thisGridVertex.Equals(phaseStorageBattleInfo.PlayerAstralOriginPos[go]))
            {
                phaseStorageBattleInfo.PlayerAstralOriginPos[go].AstralOnGrid = go;
                go.transform.position = phaseStorageBattleInfo.PlayerAstralOriginPos[go].Coordinate;
                go.GetComponent<AstralBody>().thisGridVertex.AstralOnGrid = null;
                go.GetComponent<AstralBody>().thisGridVertex = phaseStorageBattleInfo.PlayerAstralOriginPos[go];
                go.transform.eulerAngles = Vector3.zero;
            }
            else // �̵����� �ʾҴٸ� ���� ������
            {
                go.transform.eulerAngles = Vector3.zero;
            }
        }

        // AI�� �� �۵� ���Ѵ�.
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

        bool isPlayerTurn = strikeFirst != "Player"; // ���� �����ڰ� Player������ �ƴ����� ��ȯ

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
        // ���ڿ� ���� ���� �ܰ� ���
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
        // ������ �ٲٱ�
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
            phaseStorageBattleInfo.OpponentAstral.Find(v => v.GetComponent<AstralBody>().cardData.Id == 40002).GetComponent<AstralBody>().astralStatusEffect.sealDuration <= 0)) // 40002�� ����ũ���� ź���ڷ�, ��� �ε��ڰ� �⵵�� ������ �� ���� ���� ������ �Ѵ�. HandSystem���� ȣ��.
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
