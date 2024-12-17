using System.Collections;
using UnityEngine;

public class A60001 : AstralBody
{
    public override void ManaAbility()
    {
        StartCoroutine(ManaAbilityCoroutine());
    }

    IEnumerator ManaAbilityCoroutine()
    {
        yield return new WaitForSeconds((1f / astralAnimStateMachine.manaAbilityAnimationClipLength) * 1f / 2f);  // ���� �۾����� �� �����ϰ� ����. ������ 3D ���̳� VFX�� ���� ������⿡ �ý��۸� �����ϴ� ����

        GameObject go1 = Instantiate(AstralVFXManager.Instance.InvincibilityVFX);
        go1.transform.position = transform.position;
        SetAbilityTimeToAstralTurn(go1);
        AutoDestroyVFX(go1);

        if (masterPlayerTag == "Player")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral)
            {
                astral.GetComponent<AstralBody>().astralStatusEffect.OnInvincibility(3);
            }
        }
        else if (masterPlayerTag == "Opponent")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral)
            {
                astral.GetComponent<AstralBody>().astralStatusEffect.OnInvincibility(3);
            }
        }
    }


}
