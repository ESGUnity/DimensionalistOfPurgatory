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
        // 모든 영체 중 둘 이상 처치될 때의 조건
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
        go.GetComponent<AstralBody>().SetAstralInfo(GridManager.Instance.FindSpawnVertex(thisGridVertex), cardData, masterPlayerTag); // 영체 정보 설정 // 추후 반드시 활성화
        go.GetComponent<AstralBody>().IsSpawned = true;
        go.transform.position = go.GetComponent<AstralBody>().thisGridVertex.Coordinate; // 영체 위치 설정
        foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>()) // 머티리얼 설정
        {
            renderer.material = AstralVFXManager.Instance.SpawnedMaterial;
        }


        GameObject go1 = Instantiate(AstralVFXManager.Instance.SpawnEffect);
        go1.transform.position = go.transform.position;
        SetAbilityTimeToAstralTurn(go1);
        AutoDestroyVFX(go1);
    }
}
