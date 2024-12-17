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
        yield return new WaitForSeconds(0.3f); // VFX ���� ��ü�� ������ �������� �ð�. �����ϰ� ���� �ɷ��� �ߵ��Ǿ� �ڿ�������.

        if (astral != null) // ���� �ɷ��� ��� ������ �ƴ� �ð��� �ɸ� �� ��ǥ ��ü���� ���������� null üũ�� �ʿ��ϴ�.
        {
            astral.GetComponent<AstralBody>().astralStats.Damaged(1004);
        }
    }
}
