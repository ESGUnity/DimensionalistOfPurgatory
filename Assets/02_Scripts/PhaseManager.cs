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
    public event Action OnAfterPreparation; // HandSystem���� ����
    public event Action OnAfterBattle; // HandSystem���� ����
    public event Action PlayerBattleTurn; // AstralBody���� ����
    public event Action OpponentBattleTurn; // AstralBody���� ����
    public PhaseStorageBattleInfo phaseStorageBattleInfo;



    [HideInInspector] public Phase CurrentPhase;
    [HideInInspector] public float RemainTime;

    float initializeTime = 0.1f;
    float afterTime = 2f;
    float preparationTime = 100f;
    float battleTime = 5f;
    string strikeFirst = "Player";
    float remainTurnTime = 0;

    static PhaseManager instance;
    public static PhaseManager Instance { get { return instance; } }
    private void Awake()
    {
        instance = this;

        phaseStorageBattleInfo = new PhaseStorageBattleInfo();
    }
    void Start()
    {
        StartCoroutine(InitializeGame());
    }

    void Update()
    {
        
    }
    IEnumerator InitializeGame()
    {
        CurrentPhase = Phase.AfterBattle;

        yield return new WaitForSeconds(initializeTime);

        OnAfterBattle?.Invoke();

        StartCoroutine(PreparationPhase());
    }
    IEnumerator PreparationPhase()
    {
        CurrentPhase = Phase.Preparation;

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

        RemainTime = afterTime;
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

        bool isPlayerTrun = strikeFirst != "Player"; // ���� �����ڰ� Player������ �ƴ����� ��ȯ

        while (phaseStorageBattleInfo.EndBattlePhase() == "Resume")
        {
            if (isPlayerTrun)
            {
                OpponentBattleTurn?.Invoke();
            }
            else
            {
                PlayerBattleTurn?.Invoke();
            }

            yield return new WaitForSeconds(remainTurnTime);
            remainTurnTime = 0;
            isPlayerTrun ^= true;
        }
        // ���ڿ� ���� ���� �ܰ� ���
        if (phaseStorageBattleInfo.DecideWinner() == "Draw")
        {

        }
        else if (phaseStorageBattleInfo.DecideWinner() == "Player")
        {

        }
        else if (phaseStorageBattleInfo.DecideWinner() == "Opponent")
        {

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
        OnAfterBattle?.Invoke();

        RemainTime = afterTime;
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
}
