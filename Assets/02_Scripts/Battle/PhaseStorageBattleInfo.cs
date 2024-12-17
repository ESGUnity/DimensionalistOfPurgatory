using System;
using System.Collections.Generic;
using UnityEngine;
public enum StatusEffect
{
    Stun,
    Seal,
    Declain,
    Encroachment,
    IncreaseDamage,
    Invincibility,
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

    public Dictionary<GameObject, Vertex> PlayerAstralOriginPos = new();
    public Dictionary<GameObject, Vertex> OpponentAstralOriginPos = new();
    PhaseManager phaseManager;
    // 기절
    public List<GameObject> stunnedP = new();
    public List<GameObject> stunnedO = new();
    // 봉인
    public List<GameObject> sealedP = new();
    public List<GameObject> sealedO = new();
    // 쇠락
    public List<GameObject> declainedP = new();
    public List<GameObject> declainedO = new();
    // 침식
    public List<GameObject> encroachmentedP = new();
    public List<GameObject> encroachmentedO = new();
    // 공증
    public List<GameObject> increaseDamagedP = new();
    public List<GameObject> increaseDamagedO = new();
    // 무적
    public List<GameObject> invincibilityP = new();
    public List<GameObject> invincibilityO = new();
    // 처치
    public List<CardData> slainedPThisGame = new();
    public List<CardData> slainedOThisGame = new();
    // 처형
    public List<CardData> punishedPThisGame = new();
    public List<CardData> punishedOThisGame = new();

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
        if (astral.GetComponent<AstralBody>().masterPlayerTag == "Player")
        {
            PlayerAstral.Add(astral);
            PlayerAstralOriginPos[astral] = astral.GetComponent<AstralBody>().thisGridVertex;
        }
        else if (astral.GetComponent<AstralBody>().masterPlayerTag == "Opponent")
        {
            OpponentAstral.Add(astral);
            OpponentAstralOriginPos[astral] = astral.GetComponent<AstralBody>().thisGridVertex;
        }
    }
    public void RemoveAstralInList(GameObject astral)
    {
        if (astral.GetComponent<AstralBody>().masterPlayerTag == "Player")
        {
            PlayerAstral.Remove(astral);
            PlayerAstralOriginPos.Remove(astral);
        }
        else if (astral.GetComponent<AstralBody>().masterPlayerTag == "Opponent")
        {
            OpponentAstral.Remove(astral);
            OpponentAstralOriginPos.Remove(astral);
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
                else if (sufferedAstral.tag == "Opponent")
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
                else if (sufferedAstral.tag == "Opponent")
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
                else if (sufferedAstral.tag == "Opponent")
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
                else if (sufferedAstral.tag == "Opponent")
                {
                    encroachmentedO.Add(sufferedAstral);
                }
                break;
            case StatusEffect.IncreaseDamage: // 공증 역시 침식처럼 단순 디스펠 시 버프가 있는 영체를 쉽게 찾기 위한 용도.
                if (sufferedAstral.tag == "Player")
                {
                    increaseDamagedP.Add(sufferedAstral);
                }
                else if (sufferedAstral.tag == "Opponent")
                {
                    increaseDamagedO.Add(sufferedAstral);
                }
                break;
            case StatusEffect.Invincibility: // 무적 역시 침식처럼 단순 디스펠 시 버프가 있는 영체를 쉽게 찾기 위한 용도.
                if (sufferedAstral.tag == "Player")
                {
                    invincibilityP.Add(sufferedAstral);
                }
                else if (sufferedAstral.tag == "Opponent")
                {
                    invincibilityO.Add(sufferedAstral);
                }
                break;
            case StatusEffect.Slain:
                if (sufferedAstral.tag == "Player")
                {
                    slainedPThisGame.Add(sufferedAstral.GetComponent<AstralBody>().cardData);
                    SlainingOTurn?.Invoke();
                }
                else if (sufferedAstral.tag == "Opponent")
                {
                    slainedOThisGame.Add(sufferedAstral.GetComponent<AstralBody>().cardData);
                    SlainingPTurn?.Invoke();
                }
                break;

            case StatusEffect.Punish:
                if (sufferedAstral.tag == "Player")
                {
                    punishedPThisGame.Add(sufferedAstral.GetComponent<AstralBody>().cardData);
                    PunishingOTurn?.Invoke();
                }
                else if (sufferedAstral.tag == "Opponent")
                {
                    punishedOThisGame.Add(sufferedAstral.GetComponent<AstralBody>().cardData);
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
                else if (sufferedAstral.tag == "Opponent")
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
                else if (sufferedAstral.tag == "Opponent")
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
                else if (sufferedAstral.tag == "Opponent")
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
                else if (sufferedAstral.tag == "Opponent")
                {
                    if (encroachmentedO.Contains(sufferedAstral))
                    {
                        encroachmentedO.Remove(sufferedAstral);
                    }
                }
                break;
            case StatusEffect.IncreaseDamage: // 뺴줘야 디스펠할 영체를 또 찾을 수 있겠지?
                if (sufferedAstral.tag == "Player")
                {
                    if (increaseDamagedP.Contains(sufferedAstral))
                    {
                        increaseDamagedP.Remove(sufferedAstral);
                    }
                }
                else if (sufferedAstral.tag == "Opponent")
                {
                    if (increaseDamagedO.Contains(sufferedAstral))
                    {
                        increaseDamagedO.Remove(sufferedAstral);
                    }
                }
                break;
            case StatusEffect.Invincibility: // 뺴줘야 디스펠할 영체를 또 찾을 수 있겠지?
                if (sufferedAstral.tag == "Player")
                {
                    if (invincibilityP.Contains(sufferedAstral))
                    {
                        invincibilityP.Remove(sufferedAstral);
                    }
                }
                else if (sufferedAstral.tag == "Opponent")
                {
                    if (invincibilityO.Contains(sufferedAstral))
                    {
                        invincibilityO.Remove(sufferedAstral);
                    }
                }
                break;
        }

    }

}

