using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A80001 : AstralBody
{
    public override void AssignProjectile()
    {
        projectile = AstralVFXManager.Instance.P80001;
    }

    public override void ManaAbility()
    {
        StartCoroutine(ManaAbilityCoroutine());
    }

    IEnumerator ManaAbilityCoroutine()
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

        if (masterPlayerTag == "Player")
        {
            List<GameObject> tempList = PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral.FindAll(a => a.GetComponent<AstralBody>().cardData.Id == this.cardData.Id);

            if (tempList.Count >= 4)
            {
                foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral)
                {
                    GameObject go2 = Instantiate(AstralVFXManager.Instance.M80001);
                    go2.transform.position = astral.transform.position;
                    SetAbilityTimeToAstralTurn(go2);
                    AutoDestroyVFX(go2);

                    StartCoroutine(ManaAbilityHitCoroutine(go1, astral));
                }
            }
        }
        else if (masterPlayerTag == "Opponent")
        {
            List<GameObject> tempList = PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral.FindAll(a => a.GetComponent<AstralBody>().cardData.Id == this.cardData.Id);

            if (tempList.Count >= 4)
            {
                foreach (GameObject astral in PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral)
                {
                    GameObject go2 = Instantiate(AstralVFXManager.Instance.M80001);
                    go2.transform.position = astral.transform.position;
                    SetAbilityTimeToAstralTurn(go2);
                    AutoDestroyVFX(go2);

                    StartCoroutine(ManaAbilityHitCoroutine(go2, astral));
                }
            }
        }
    }

    IEnumerator ManaAbilityHitCoroutine(GameObject VFX, GameObject astral)
    {
        yield return new WaitForSeconds(0.3f); // VFX 빔이 영체에 도착할 때까지의 시간. 도착하고 나서 능력이 발동되야 자연스럽다.

        if (astral != null) // 고유 능력이 즉시 시전이 아닌 시간이 걸린 뒤 목표 영체에게 시전함으로 null 체크가 필요하다.
        {
            astral.GetComponent<AstralBody>().astralStats.Damaged(1004);
        }
    }
}
