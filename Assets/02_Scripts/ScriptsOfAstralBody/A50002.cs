using UnityEngine;

public class A50002 : AstralBody
{
    public override void AssignProjectile()
    {
        projectile = AstralVFXManager.Instance.P50002;
    }

    public override void DeadAbility()
    {
        int totalAstral;

        if (masterPlayerTag == "Player")
        {
            totalAstral = PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral.Count;
            GameObject go = PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral[Random.Range(0, totalAstral)];
            go.GetComponent<AstralBody>().astralStats.Damaged(9999);
        }
        else if (masterPlayerTag == "Opponent")
        {
            totalAstral = PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral.Count;
            GameObject go = PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral[Random.Range(0, totalAstral)];
            go.GetComponent<AstralBody>().astralStats.Damaged(9999);
        }
    }
}
