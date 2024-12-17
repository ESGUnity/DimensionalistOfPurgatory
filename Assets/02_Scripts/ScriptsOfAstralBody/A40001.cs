using UnityEngine;

public class A40001 : AstralBody
{
    public override void AssignProjectile()
    {
        projectile = AstralVFXManager.Instance.P40001;
    }
    public override void SubcribeCondition()
    {
        // 적 영체 중 셋 이상 기절될 때의 조건
        if (masterPlayerTag == "Player")
        {
            PhaseManager.Instance.phaseStorageBattleInfo.StunningPTurn += AddCondition;
        }
        else if (masterPlayerTag == "Opponent")
        {
            PhaseManager.Instance.phaseStorageBattleInfo.StunningOTurn += AddCondition;
        }
    }
    public override void UnsubscribeCondition()
    {
        if (masterPlayerTag == "Player")
        {
            PhaseManager.Instance.phaseStorageBattleInfo.StunningPTurn -= AddCondition;
        }
        else if (masterPlayerTag == "Opponent")
        {
            PhaseManager.Instance.phaseStorageBattleInfo.StunningOTurn -= AddCondition;
        }
    }
    public override void AddCondition()
    {
        base.AddCondition();
    }
    public override void ConditionAbility()
    {
        astralStatusEffect.OnIncreaseDamage(20);
    }
}
