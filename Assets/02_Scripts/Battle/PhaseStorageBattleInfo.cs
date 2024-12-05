using System;
using System.Collections.Generic;
using UnityEngine;
public enum StatusEffect
{
    Stun,
    Seal,
    Declain,
    Encroachment,
    Slain,
    Punish
}

public class PhaseStorageBattleInfo 
{
    public event Action StunningPTurn;
    public event Action StunningOTurn;
    public event Action SealingPTurn;
    public event Action SealingOTurn;
    public event Action DeclainingPTurn;
    public event Action DeclainingOTurn;
    public event Action SlainingPTurn;
    public event Action SlainingOTurn;
    public event Action PunishingPTurn;
    public event Action PunishingOTurn;

    public List<GameObject> PlayerAstral = new();
    public List<GameObject> OpponentAstral = new();

    PhaseManager phaseManager;
    // 기절
    List<GameObject> stunnedP = new();
    List<GameObject> stunnedO = new();
    // 봉인
    List<GameObject> sealedP = new();
    List<GameObject> sealedO = new();
    // 쇠락
    List<GameObject> declainedP = new();
    List<GameObject> declainedO = new();
    // 침식
    List<GameObject> encroachmentedP = new();
    List<GameObject> encroachmentedO = new();
    // 처치
    List<GameObject> slainedPThisGame = new();
    List<GameObject> slainedOThisGame = new();
    // 처형
    List<GameObject> punishedPThisGame = new();
    List<GameObject> punishedOThisGame = new();

    public PhaseStorageBattleInfo()
    {
        phaseManager = PhaseManager.Instance;
    }

    public bool EndBattlePhase()
    {
        if (PlayerAstral.Count <= 0 && OpponentAstral.Count <= 0)
        {
            return true;
        }
        else if (PlayerAstral.Count <= 0)
        {
            return true;
        }
        else if (OpponentAstral.Count <= 0)
        {
            return true;
        }

        return false;
    }
    public string DecideWinner()
    {
        if (PlayerAstral.Count <= 0 && OpponentAstral.Count <= 0)
        {
            return "Draw";
        }
        else if (PlayerAstral.Count <= 0)
        {
            return "Opponent";
        }
        else if (OpponentAstral.Count <= 0)
        {
            return "Player";
        }

        return "";
    }
    public void AddAstralInList(GameObject astral)
    {
        if (astral.tag == "Player")
        {
            PlayerAstral.Add(astral);
        }
        else if (astral.tag == "Opponent")
        {
            OpponentAstral.Add(astral);
        }
    }
    public void RemoveAstralInList(GameObject astral)
    {
        if (astral.tag == "Player")
        {
            PlayerAstral.Remove(astral);
        }
        else if (astral.tag == "Opponent")
        {
            OpponentAstral.Remove(astral);
        }
    }
    public void NotifyStatusEffect(StatusEffect se, GameObject sufferedAstral) // 이건 상태이상 당한 영체가 호출할 것이다.
    {
        switch (se)
        {
            case StatusEffect.Stun:
                if (sufferedAstral.tag == "Player")
                {
                    stunnedP.Add(sufferedAstral); // 플레이어 영체가 기절되었다면
                    StunningOTurn?.Invoke(); // 상대 영체에게 기절당했음을 알린다.
                }
                else if (sufferedAstral.tag == " Opponent")
                {
                    stunnedO.Add(sufferedAstral);
                    StunningPTurn?.Invoke();
                }
                break;

            case StatusEffect.Seal:
                if (sufferedAstral.tag == "Player")
                {
                    sealedP.Add(sufferedAstral);
                    SealingOTurn?.Invoke();
                }
                else if (sufferedAstral.tag == " Opponent")
                {
                    sealedO.Add(sufferedAstral);
                    SealingPTurn?.Invoke();
                }
                break;

            case StatusEffect.Declain:
                if (sufferedAstral.tag == "Player")
                {
                    declainedP.Add(sufferedAstral);
                    DeclainingOTurn?.Invoke();
                }
                else if (sufferedAstral.tag == " Opponent")
                {
                    declainedO.Add(sufferedAstral);
                    DeclainingPTurn?.Invoke();
                }
                break;

            case StatusEffect.Encroachment: // 침식은 침식 시킬 때마다와 같은 조건에 사용되지 않는다. 단순 정화 시 해제되기 위한 용도.
                if (sufferedAstral.tag == "Player")
                {
                    encroachmentedP.Add(sufferedAstral);
                }
                else if (sufferedAstral.tag == " Opponent")
                {
                    encroachmentedO.Add(sufferedAstral);
                }
                break;

            case StatusEffect.Slain:
                if (sufferedAstral.tag == "Player")
                {
                    slainedPThisGame.Add(sufferedAstral);
                    SlainingOTurn?.Invoke();
                }
                else if (sufferedAstral.tag == " Opponent")
                {
                    slainedOThisGame.Add(sufferedAstral);
                    SlainingPTurn?.Invoke();
                }
                break;

            case StatusEffect.Punish:
                if (sufferedAstral.tag == "Player")
                {
                    punishedPThisGame.Add(sufferedAstral);
                    PunishingOTurn?.Invoke();
                }
                else if (sufferedAstral.tag == " Opponent")
                {
                    punishedOThisGame.Add(sufferedAstral);
                    PunishingPTurn?.Invoke();
                }
                break;
        }

    }
    public void NotifyReleaseStatusEffect(StatusEffect se, GameObject sufferedAstral) // 이건 상태이상 당한 영체가 호출할 것이다.
    {
        switch (se)
        {
            case StatusEffect.Stun:
                if (sufferedAstral.tag == "Player")
                {
                    if (stunnedP.Contains(sufferedAstral))
                    {
                        stunnedP.Remove(sufferedAstral);
                    }
                }
                else if (sufferedAstral.tag == " Opponent")
                {
                    if (stunnedP.Contains(sufferedAstral))
                    {
                        stunnedO.Remove(sufferedAstral);

                    }
                }
                break;
            case StatusEffect.Seal:
                if (sufferedAstral.tag == "Player")
                {
                    if (sealedP.Contains(sufferedAstral))
                    {
                        sealedP.Remove(sufferedAstral);

                    }
                }
                else if (sufferedAstral.tag == " Opponent")
                {
                    if (sealedO.Contains(sufferedAstral))
                    {
                        sealedO.Remove(sufferedAstral);

                    }
                }
                break;
            case StatusEffect.Declain:
                if (sufferedAstral.tag == "Player")
                {
                    if (declainedP.Contains(sufferedAstral))
                    {
                        declainedP.Remove(sufferedAstral);
                    }
                }
                else if (sufferedAstral.tag == " Opponent")
                {
                    if (declainedO.Contains(sufferedAstral))
                    {
                        declainedO.Remove(sufferedAstral);
                    }
                }
                break;
            case StatusEffect.Encroachment:
                if (sufferedAstral.tag == "Player")
                {
                    if (encroachmentedP.Contains(sufferedAstral))
                    {
                        encroachmentedP.Remove(sufferedAstral);
                    }
                }
                else if (sufferedAstral.tag == " Opponent")
                {
                    if (encroachmentedO.Contains(sufferedAstral))
                    {
                        encroachmentedO.Remove(sufferedAstral);
                    }
                }
                break;
        }

    }

}

