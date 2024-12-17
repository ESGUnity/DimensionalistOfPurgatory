using System.Collections;
using UnityEngine;

public class A20001 : AstralBody
{
    public override void AssignProjectile()
    {
        projectile = AstralVFXManager.Instance.P20001;
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
                GameObject go = Instantiate(AstralVFXManager.Instance.DeclainVFX);
                go.GetComponent<AstralProjectile>().SetTarget(astral, this);
                StartCoroutine(ManaAbilityHitCoroutine(go, astral));
            }
        }
        else if (masterPlayerTag == "Opponent")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral)
            {
                GameObject go = Instantiate(AstralVFXManager.Instance.DeclainVFX);
                go.GetComponent<AstralProjectile>().SetTarget(astral, this);
                StartCoroutine(ManaAbilityHitCoroutine(go, astral));
            }
        }
    }

    IEnumerator ManaAbilityHitCoroutine(GameObject go, GameObject astral)
    {
        yield return new WaitForSeconds(go.GetComponent<AstralProjectile>().projectileDuration); // 프로젝타일이 영체에 도착할 때까지의 시간. 도착하고 나서 능력이 발동되야 자연스럽다.

        if (astral != null) // 고유 능력이 즉시 시전이 아닌 시간이 걸린 뒤 목표 영체에게 시전함으로 null 체크가 필요하다.
        {
            astral.GetComponent<AstralBody>().astralStatusEffect.OnDeclain(2);
        }
    }
}
