using System.Collections;
using UnityEngine;

public class A30001 : AstralBody
{
    public override void ManaAbility()
    {
        StartCoroutine(ManaAbilityCoroutine());
    }

    IEnumerator ManaAbilityCoroutine()
    {
        yield return new WaitForSeconds((1f / astralAnimStateMachine.manaAbilityAnimationClipLength) * 1f / 2f);

        int totalAstral;

        if (masterPlayerTag == "Player")
        {
            totalAstral = PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral.Count;
            GameObject astral = PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral[Random.Range(0, totalAstral)];

            GameObject go = Instantiate(AstralVFXManager.Instance.StunVFX);
            go.transform.position = astral.transform.position;
            SetAbilityTimeToAstralTurnInChild(go);
            AutoDestroyVFXInChild(go);
            StartCoroutine(ManaAbilityHitCoroutine(go, astral));
        }
        else if (masterPlayerTag == "Opponent")
        {
            totalAstral = PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral.Count;
            GameObject astral = PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral[Random.Range(0, totalAstral)];

            GameObject go = Instantiate(AstralVFXManager.Instance.StunVFX);
            go.transform.position = astral.transform.position;
            SetAbilityTimeToAstralTurnInChild(go);
            AutoDestroyVFXInChild(go);
            StartCoroutine(ManaAbilityHitCoroutine(go, astral));
        }
    }

    IEnumerator ManaAbilityHitCoroutine(GameObject go, GameObject astral)
    {
        yield return new WaitForSeconds(GetVFXLengthInChild(go)); // ���� VFX�� �� �������� ����.

        if (astral != null) // ���� �ɷ��� ��� ������ �ƴ� �ð��� �ɸ� �� ��ǥ ��ü���� ���������� null üũ�� �ʿ��ϴ�.
        {
            astral.GetComponent<AstralBody>().astralStatusEffect.OnStun(3);
        }
    }
}
