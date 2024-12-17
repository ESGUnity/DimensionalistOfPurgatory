using System.Collections;
using UnityEngine;

public class A50001 : AstralBody
{
    public override void ManaAbility()
    {
        StartCoroutine(ManaAbilityCoroutine());
    }

    IEnumerator ManaAbilityCoroutine()
    {
        yield return new WaitForSeconds((1f / astralAnimStateMachine.manaAbilityAnimationClipLength) * 1f / 2f);

        if (masterPlayerTag == "Player")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral)
            {
                if ((float)astral.GetComponent<AstralBody>().astralStats.CurrentHealth / (float)astral.GetComponent<AstralBody>().astralStats.MaxHealth <= 0.5f) // 체력이 50% 이하라면
                {
                    GameObject go = Instantiate(AstralVFXManager.Instance.StunVFX);
                    go.transform.position = astral.transform.position;
                    SetAbilityTimeToAstralTurnInChild(go);
                    AutoDestroyVFXInChild(go);
                    StartCoroutine(ManaAbilityHitCoroutine(go, astral));
                }
            }
        }
        else if (masterPlayerTag == "Opponent")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral)
            {
                if ((float)astral.GetComponent<AstralBody>().astralStats.CurrentHealth / (float)astral.GetComponent<AstralBody>().astralStats.MaxHealth <= 0.5f) // 체력이 50% 이하라면
                {
                    GameObject go = Instantiate(AstralVFXManager.Instance.StunVFX);
                    go.transform.position = astral.transform.position;
                    SetAbilityTimeToAstralTurnInChild(go);
                    AutoDestroyVFXInChild(go);
                    StartCoroutine(ManaAbilityHitCoroutine(go, astral));
                }
            }
        }
    }

    IEnumerator ManaAbilityHitCoroutine(GameObject go, GameObject astral)
    {
        yield return new WaitForSeconds(GetVFXLengthInChild(go)); // 기절 VFX가 다 떨어지는 순간.

        if (astral != null) // 고유 능력이 즉시 시전이 아닌 시간이 걸린 뒤 목표 영체에게 시전함으로 null 체크가 필요하다.
        {
            astral.GetComponent<AstralBody>().astralStatusEffect.OnStun(3);
        }
    }
}
