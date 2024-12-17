using UnityEngine;

public class P61001 : Pray
{
    public override void CastPray()
    {
        if (thisPlayerTag == "Player")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral)
            {
                GameObject go = Instantiate(AstralVFXManager.Instance.SealVFX);
                go.transform.position = transform.position;
                astral.GetComponent<AstralBody>().SetAbilityTimeToAstralTurn(go);
                astral.GetComponent<AstralBody>().AutoDestroyVFX(go);
                astral.GetComponent<AstralBody>().astralStatusEffect.OnSeal(6);
            }
        }
        else if (thisPlayerTag == "Opponent")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral)
            {
                GameObject go = Instantiate(AstralVFXManager.Instance.SealVFX);
                go.transform.position = transform.position;
                astral.GetComponent<AstralBody>().SetAbilityTimeToAstralTurn(go);
                astral.GetComponent<AstralBody>().AutoDestroyVFX(go);
                astral.GetComponent<AstralBody>().astralStatusEffect.OnSeal(6);
            }
        }
    }
}
