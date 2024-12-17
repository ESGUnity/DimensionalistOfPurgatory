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
        yield return new WaitForSeconds((1f / astralAnimStateMachine.manaAbilityAnimationClipLength) * 2f / 3f); // �� ��ü�� �����ɷ� �ִϸ��̼��� �⸦ �����ٰ� 2 / 3 �������� ���� ������ ����� ���Ѵ�. // ���� �۾����� �� �����ϰ� ����. ������ 3D ���̳� VFX�� ���� ������⿡ �ý��۸� �����ϴ� ����

        if (masterPlayerTag == "Player")
        {
            foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral)
            {
                if (astral.GetComponent<AstralBody>().astralStats.MaxMana != 0) // �游 ������Ʈ�� ����
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
