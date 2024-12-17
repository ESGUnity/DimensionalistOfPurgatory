using UnityEngine;

public class A70002 : AstralBody
{
    public override void SubcribeCondition()
    {
        // 모든 영체 중 둘 이상 처치될 때의 조건
        PhaseManager.Instance.phaseStorageBattleInfo.SlainingPTurn += AddCondition;
        PhaseManager.Instance.phaseStorageBattleInfo.SlainingOTurn += AddCondition;
    }
    public override void UnsubscribeCondition()
    {
        PhaseManager.Instance.phaseStorageBattleInfo.SlainingPTurn -= AddCondition;
        PhaseManager.Instance.phaseStorageBattleInfo.SlainingOTurn -= AddCondition;
    }
    public override void AddCondition()
    {
        base.AddCondition();
    }
    public override void ConditionAbility()
    {
        if (masterPlayerTag == "Player")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral)
            {
                if ((float)astral.GetComponent<AstralBody>().astralStats.CurrentHealth / (float)astral.GetComponent<AstralBody>().astralStats.MaxHealth <= 0.3f)
                {
                    astral.GetComponent<AstralBody>().astralStats.Damaged(9999, 9999);
                }
            }
        }
        else if (masterPlayerTag == "Opponent")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral)
            {
                if ((float)astral.GetComponent<AstralBody>().astralStats.CurrentHealth / (float)astral.GetComponent<AstralBody>().astralStats.MaxHealth <= 0.3f)
                {
                    astral.GetComponent<AstralBody>().astralStats.Damaged(9999, 9999);
                }
            }
        }
    }
}
