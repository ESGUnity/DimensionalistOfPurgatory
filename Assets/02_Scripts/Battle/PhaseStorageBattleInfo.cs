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
    // ����
    public List<GameObject> stunnedP = new();
    public List<GameObject> stunnedO = new();
    // ����
    public List<GameObject> sealedP = new();
    public List<GameObject> sealedO = new();
    // ���
    public List<GameObject> declainedP = new();
    public List<GameObject> declainedO = new();
    // ħ��
    public List<GameObject> encroachmentedP = new();
    public List<GameObject> encroachmentedO = new();
    // ����
    public List<GameObject> increaseDamagedP = new();
    public List<GameObject> increaseDamagedO = new();
    // ����
    public List<GameObject> invincibilityP = new();
    public List<GameObject> invincibilityO = new();
    // óġ
    public List<CardData> slainedPThisGame = new();
    public List<CardData> slainedOThisGame = new();
    // ó��
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
    public void NotifyStatusEffect(StatusEffect se, GameObject sufferedAstral) // �̰� �����̻� ���� ��ü�� ȣ���� ���̴�.
    {
        switch (se)
        {
            case StatusEffect.Stun:
                if (sufferedAstral.tag == "Player")
                {
                    stunnedP.Add(sufferedAstral); // �÷��̾� ��ü�� �����Ǿ��ٸ�
                    StunningOTurn?.Invoke(); // ��� ��ü���� ������������ �˸���.
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

            case StatusEffect.Encroachment: // ħ���� ħ�� ��ų �����ٿ� ���� ���ǿ� ������ �ʴ´�. �ܼ� ��ȭ �� �����Ǳ� ���� �뵵.
                if (sufferedAstral.tag == "Player")
                {
                    encroachmentedP.Add(sufferedAstral);
                }
                else if (sufferedAstral.tag == "Opponent")
                {
                    encroachmentedO.Add(sufferedAstral);
                }
                break;
            case StatusEffect.IncreaseDamage: // ���� ���� ħ��ó�� �ܼ� ���� �� ������ �ִ� ��ü�� ���� ã�� ���� �뵵.
                if (sufferedAstral.tag == "Player")
                {
                    increaseDamagedP.Add(sufferedAstral);
                }
                else if (sufferedAstral.tag == "Opponent")
                {
                    increaseDamagedO.Add(sufferedAstral);
                }
                break;
            case StatusEffect.Invincibility: // ���� ���� ħ��ó�� �ܼ� ���� �� ������ �ִ� ��ü�� ���� ã�� ���� �뵵.
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
    public void NotifyReleaseStatusEffect(StatusEffect se, GameObject sufferedAstral) // �̰� �����̻� ���� ��ü�� ȣ���� ���̴�.
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
            case StatusEffect.IncreaseDamage: // ����� ������ ��ü�� �� ã�� �� �ְ���?
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
            case StatusEffect.Invincibility: // ����� ������ ��ü�� �� ã�� �� �ְ���?
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

