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
    public event Action OnAfterPreparation;
    public event Action OnAfterBattle;
    public event Action PlayerBattleTurn;
    public event Action OpponentBattleTurn;



    [HideInInspector] public Phase CurrentPhase;
    [HideInInspector] public float RemainTime;

    float afterTime = 2f;
    float preparationTime = 10f;
    float battleTime = 5f;   

    static PhaseManager instance;
    public static PhaseManager Instance { get { return instance; } }
    private void Awake()
    {
        instance = this;
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
        yield return new WaitForSeconds(3f);
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

        RemainTime = battleTime;
        while (RemainTime > 0)
        {
            RemainTime -= Time.deltaTime;
            yield return null;
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
}
