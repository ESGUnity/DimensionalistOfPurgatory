using System.Collections;
using UnityEngine;

public class A50003 : AstralBody
{
    public override void AssignProjectile()
    {
        projectile = AstralVFXManager.Instance.P50003;
    }
    public override void ManaAbility()
    {
        StartCoroutine(ManaAbilityCoroutine());
    }

    IEnumerator ManaAbilityCoroutine()
    {
        yield return new WaitForSeconds((1f / astralAnimStateMachine.manaAbilityAnimationClipLength) * 2f / 3f); // 이 영체의 마나능력 애니메이션이 기를 모으다가 2 / 3 지점에서 무언가 날리는 모션을 취한다. // 실제 작업에선 더 신중하게 하자. 지금은 3D 모델이나 VFX를 대충 만들었기에 시스템만 구축하는 느낌

        if (masterPlayerTag == "Player")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral)
            {
                if (astral.GetComponent<AstralBody>().astralStats.MaxMana != 0) // 충만 오브젝트일 때만
                {
                    GameObject go = Instantiate(AstralVFXManager.Instance.SealVFX);
                    go.transform.position = transform.position;
                    SetAbilityTimeToAstralTurn(go);
                    AutoDestroyVFX(go);
                    astral.GetComponent<AstralBody>().astralStatusEffect.OnSeal(3);
                }
            }
        }
        else if (masterPlayerTag == "Opponent")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral)
            {
                if (astral.GetComponent<AstralBody>().astralStats.MaxMana != 0)
                {
                    GameObject go = Instantiate(AstralVFXManager.Instance.SealVFX);
                    go.transform.position = transform.position;
                    SetAbilityTimeToAstralTurn(go);
                    AutoDestroyVFX(go);
                    astral.GetComponent<AstralBody>().astralStatusEffect.OnSeal(3);
                }
            }
        }
    }


}
