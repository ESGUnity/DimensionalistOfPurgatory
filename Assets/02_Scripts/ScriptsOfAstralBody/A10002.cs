using UnityEngine;

public class A10002 : AstralBody
{
    public override void SetProjectileAndAbilityVFXAndObserving()
    {
        projectile = AstralVFXManager.Instance.P10002;
    }

    public override void DeadAbility()
    {
        int totalAstral;

        if (masterPlayerTag == "Player")
        {
            totalAstral = PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral.Count;
            GameObject go = PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral[Random.Range(0, totalAstral)];
            go.GetComponent<AstralBody>().astralStatusEffect.OnDeclain(10);
        }
        else if (masterPlayerTag == "Opponent")
        {
            totalAstral = PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral.Count;
            GameObject go = PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral[Random.Range(0, totalAstral)];
            go.GetComponent<AstralBody>().astralStatusEffect.OnDeclain(10);
        }
    }
}
