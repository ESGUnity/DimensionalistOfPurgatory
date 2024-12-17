using System.Collections;
using UnityEngine;

public class A30003 : AstralBody
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
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral)
            {
                if (astral.GetComponent<AstralBody>().astralStatusEffect.IsDisrupted()) // ���� ���� �̻� ȿ���� �ִٸ�
                {
                    astral.GetComponent<AstralBody>().astralStatusEffect.OnCleanse();
                    break;
                }
            }
        }
        else if (masterPlayerTag == "Opponent")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral)
            {
                if (astral.GetComponent<AstralBody>().astralStatusEffect.IsDisrupted()) // ���� ���� �̻� ȿ���� �ִٸ�
                {
                    astral.GetComponent<AstralBody>().astralStatusEffect.OnCleanse();
                    break;
                }
            }
        }
    }
}
