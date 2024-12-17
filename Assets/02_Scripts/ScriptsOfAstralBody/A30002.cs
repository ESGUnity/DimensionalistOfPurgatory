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
                if (astral != gameObject) // �ڱ� �ڽ��� �ƴ϶��(�Ҹ��� �θ��̾ �����ο��� ������ ���ɼ��� �ֱ� �����̴�.
                {
                    astral.GetComponent<AstralBody>().astralStatusEffect.OnInvincibility(4);
                    break; // �� �ؾ� ��ü �ϳ��� ã�´�.
                }
            }
        }
        else if (masterPlayerTag == "Opponent")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral)
            {
                if (astral != gameObject) // �ڱ� �ڽ��� �ƴ϶��
                {
                    astral.GetComponent<AstralBody>().astralStatusEffect.OnInvincibility(4);
                    break;
                }
            }
        }
    }
}
