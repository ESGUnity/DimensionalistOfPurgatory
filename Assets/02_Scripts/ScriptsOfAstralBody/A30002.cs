using UnityEngine;

public class A30002 : AstralBody
{
    public override void DeadAbility()
    {
        GameObject go1 = Instantiate(AstralVFXManager.Instance.InvincibilityVFX);
        go1.transform.position = transform.position;
        SetAbilityTimeToAstralTurn(go1);
        AutoDestroyVFX(go1);

        if (masterPlayerTag == "Player")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral)
            {
                if (astral != gameObject) // 자기 자신이 아니라면(소멸의 부름이어서 스스로에게 시전할 가능성이 있기 때문이다.
                {
                    astral.GetComponent<AstralBody>().astralStatusEffect.OnInvincibility(4);
                    break; // 를 해야 영체 하나만 찾는다.
                }
            }
        }
        else if (masterPlayerTag == "Opponent")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral)
            {
                if (astral != gameObject) // 자기 자신이 아니라면
                {
                    astral.GetComponent<AstralBody>().astralStatusEffect.OnInvincibility(4);
                    break;
                }
            }
        }
    }
}
