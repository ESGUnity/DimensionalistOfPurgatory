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
        yield return new WaitForSeconds((1f / astralAnimStateMachine.manaAbilityAnimationClipLength) * 1f / 2f);  // 실제 작업에선 더 신중하게 하자. 지금은 3D 모델이나 VFX를 대충 만들었기에 시스템만 구축하는 느낌

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
