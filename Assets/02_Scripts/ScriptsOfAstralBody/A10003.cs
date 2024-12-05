using UnityEngine;

public class A10003 : AstralBody
{
    public override void SetProjectileAndAbilityVFXAndObserving()
    {

        if (masterPlayerTag == "Player")
        {
            PhaseManager.Instance.phaseStorageBattleInfo.SlainingPTurn += AddSlainCondition;
        }
        else if (masterPlayerTag == "Opponent")
        {
            PhaseManager.Instance.phaseStorageBattleInfo.SlainingOTurn += AddSlainCondition;
        }
    }
    public override void ConditionAbility()
    {
        GameObject go = Instantiate(AstralVFXManager.Instance.C10003);
        SetAstralActionTurnForAbility(go);
    }

    public void AddSlainCondition()
    {
        astralStats.CurrentCondition++;
    }
}
