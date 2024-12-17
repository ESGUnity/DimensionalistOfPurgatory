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
        yield return new WaitForSeconds(go.GetComponent<AstralProjectile>().projectileDuration); // ������Ÿ���� ��ü�� ������ �������� �ð�. �����ϰ� ���� �ɷ��� �ߵ��Ǿ� �ڿ�������.

        if (astral != null) // ���� �ɷ��� ��� ������ �ƴ� �ð��� �ɸ� �� ��ǥ ��ü���� ���������� null üũ�� �ʿ��ϴ�.
        {
            astral.GetComponent<AstralBody>().astralStats.Damaged(15, 15);
        }
    }
}
