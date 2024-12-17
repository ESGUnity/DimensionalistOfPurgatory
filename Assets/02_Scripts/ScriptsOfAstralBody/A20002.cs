using System.Collections;
using UnityEngine;

public class A20002 : AstralBody
{
    public override void ManaAbility()
    {
        StartCoroutine(ManaAbilityCoroutine());
    }

    IEnumerator ManaAbilityCoroutine()
    {
        yield return new WaitForSeconds((1f / astralAnimStateMachine.manaAbilityAnimationClipLength) * 1f / 3f); 

        if (masterPlayerTag == "Player")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral)
            {
                GameObject go = Instantiate(AstralVFXManager.Instance.M20002);
                go.GetComponent<AstralProjectile>().SetTarget(astral, this);
                StartCoroutine(ManaAbilityHitCoroutine(go, astral));
            }
        }
        else if (masterPlayerTag == "Opponent")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral)
            {
                GameObject go = Instantiate(AstralVFXManager.Instance.M20002);
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
            astral.GetComponent<AstralBody>().astralStats.Damaged(15, 15);
        }
    }
}
