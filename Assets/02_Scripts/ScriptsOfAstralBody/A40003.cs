using System.Collections;
using UnityEngine;

public class A40003 : AstralBody
{
    public override void AssignProjectile()
    {
        projectile = AstralVFXManager.Instance.P40003;
    }

    public override void SubcribeCondition()
    {
        // ��� ��ü �� �� �̻� óġ�� ���� ����
        PhaseManager.Instance.phaseStorageBattleInfo.PunishingPTurn += AddCondition;
        PhaseManager.Instance.phaseStorageBattleInfo.PunishingOTurn += AddCondition;
    }
    public override void UnsubscribeCondition()
    {
        PhaseManager.Instance.phaseStorageBattleInfo.PunishingPTurn -= AddCondition;
        PhaseManager.Instance.phaseStorageBattleInfo.PunishingOTurn -= AddCondition;
    }
    public override void AddCondition()
    {
        base.AddCondition();
    }

    public override void ConditionAbility()
    {
        StartCoroutine(ConditionAbilityCoroutine());
    }

    IEnumerator ConditionAbilityCoroutine()
    {
        yield return new WaitForSeconds((1f / astralAnimStateMachine.manaAbilityAnimationClipLength) + 0.1f); 

        GameObject go = Instantiate(cardData.Prefab);
        go.GetComponent<AstralBody>().SetAstralInfo(GridManager.Instance.FindSpawnVertex(thisGridVertex), cardData, masterPlayerTag); // ��ü ���� ���� // ���� �ݵ�� Ȱ��ȭ
        go.GetComponent<AstralBody>().IsSpawned = true;
        go.transform.position = go.GetComponent<AstralBody>().thisGridVertex.Coordinate; // ��ü ��ġ ����
        foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>()) // ��Ƽ���� ����
        {
            renderer.material = AstralVFXManager.Instance.SpawnedMaterial;
        }


        GameObject go1 = Instantiate(AstralVFXManager.Instance.SpawnEffect);
        go1.transform.position = go.transform.position;
        SetAbilityTimeToAstralTurn(go1);
        AutoDestroyVFX(go1);
    }
}
