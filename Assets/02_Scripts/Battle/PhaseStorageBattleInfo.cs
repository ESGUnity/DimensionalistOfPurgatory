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
    // ����
    List<GameObject> stunnedP = new();
    List<GameObject> stunnedO = new();
    // ����
    List<GameObject> sealedP = new();
    List<GameObject> sealedO = new();
    // ���
    List<GameObject> declainedP = new();
    List<GameObject> declainedO = new();
    // ħ��
    List<GameObject> encroachmentedP = new();
    List<GameObject> encroachmentedO = new();
    // óġ
    List<GameObject> slainedPThisGame = new();
    List<GameObject> slainedOThisGame = new();
    // ó��
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

            case StatusEffect.Encroachment: // ħ���� ħ�� ��ų �����ٿ� ���� ���ǿ� ������ �ʴ´�. �ܼ� ��ȭ �� �����Ǳ� ���� �뵵.
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

