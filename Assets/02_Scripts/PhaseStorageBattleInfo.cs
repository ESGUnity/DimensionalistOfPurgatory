using System.Collections.Generic;
using UnityEngine;

public class PhaseStorageBattleInfo
{
    public List<Vertex> PlayerAstral;
    public List<Vertex> OpponentAstral;

    PhaseManager phaseManager;
    int stunAstralOnField; // 전장에 존재하는 기절한 영체의 총 숫자
    int stunAstralThisTurn;

    public PhaseStorageBattleInfo()
    {
        phaseManager = PhaseManager.Instance;
    }

    public string EndBattlePhase()
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

        return "Resume";
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
            PlayerAstral.Add(astral.GetComponent<AstralBody>().thisGridVertex);
        }
        else if (astral.tag == "Opponent")
        {
            OpponentAstral.Add(astral.GetComponent<AstralBody>().thisGridVertex);
        }
    }
    public void RemoveAstralInList(GameObject astral)
    {
        if (astral.tag == "Player")
        {
            PlayerAstral.Remove(astral.GetComponent<AstralBody>().thisGridVertex);
        }
        else if (astral.tag == "Opponent")
        {
            OpponentAstral.Remove(astral.GetComponent<AstralBody>().thisGridVertex);
        }
    }

    public void SetBattlePhaseAndFieldInfo()
    {

    }
}

