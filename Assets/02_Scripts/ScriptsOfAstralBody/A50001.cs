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
                if ((float)astral.GetComponent<AstralBody>().astralStats.CurrentHealth / (float)astral.GetComponent<AstralBody>().astralStats.MaxHealth <= 0.5f) // ü���� 50% ���϶��
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
                if ((float)astral.GetComponent<AstralBody>().astralStats.CurrentHealth / (float)astral.GetComponent<AstralBody>().astralStats.MaxHealth <= 0.5f) // ü���� 50% ���϶��
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
        yield return new WaitForSeconds(GetVFXLengthInChild(go)); // ���� VFX�� �� �������� ����.

        if (astral != null) // ���� �ɷ��� ��� ������ �ƴ� �ð��� �ɸ� �� ��ǥ ��ü���� ���������� null üũ�� �ʿ��ϴ�.
        {
            astral.GetComponent<AstralBody>().astralStatusEffect.OnStun(3);
        }
    }
}
