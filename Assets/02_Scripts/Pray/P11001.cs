using UnityEngine;

public class P11001 : Pray
{
    public GameObject A10001;
    public override void CastPray()
    {
        if (CastedGridVertex.AstralOnGrid == null)
        {
            if (thisPlayerTag == "Player")
            {
                GameObject go;

                if (PhaseManager.Instance.phaseStorageBattleInfo.slainedPThisGame.Count > 0)
                {
                    int x = Random.Range(0, PhaseManager.Instance.phaseStorageBattleInfo.slainedPThisGame.Count);
                    go = Instantiate(PhaseManager.Instance.phaseStorageBattleInfo.slainedPThisGame[x].Prefab);
                    go.GetComponent<AstralBody>().SetAstralInfo(CastedGridVertex, PhaseManager.Instance.phaseStorageBattleInfo.slainedPThisGame[x], thisPlayerTag); // ��ü ���� ���� // ���� �ݵ�� Ȱ��ȭ
                    go.GetComponent<AstralBody>().IsSpawned = true;
                    go.transform.position = go.GetComponent<AstralBody>().thisGridVertex.Coordinate; // ��ü ��ġ ����

                    foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>()) // ��Ƽ���� ����
                    {
                        renderer.material = AstralVFXManager.Instance.SpawnedMaterial;
                    }
                    go.GetComponent<AstralBody>().astralStats.CurrentHealth = 1;

                    GameObject go1 = Instantiate(AstralVFXManager.Instance.SpawnEffect);
                    go1.transform.position = go.transform.position;
                    go.GetComponent<AstralBody>().SetAbilityTimeToAstralTurn(go1);
                    go.GetComponent<AstralBody>().AutoDestroyVFX(go1);
                }
            }
            else if (thisPlayerTag == "Opponent")
            {
                GameObject go;

                if (PhaseManager.Instance.phaseStorageBattleInfo.slainedOThisGame.Count > 0)
                {
                    int x = Random.Range(0, PhaseManager.Instance.phaseStorageBattleInfo.slainedOThisGame.Count);
                    go = Instantiate(PhaseManager.Instance.phaseStorageBattleInfo.slainedOThisGame[x].Prefab);
                    go.GetComponent<AstralBody>().SetAstralInfo(CastedGridVertex, PhaseManager.Instance.phaseStorageBattleInfo.slainedOThisGame[x], thisPlayerTag); // ��ü ���� ���� // ���� �ݵ�� Ȱ��ȭ
                    go.GetComponent<AstralBody>().IsSpawned = true;
                    go.transform.position = go.GetComponent<AstralBody>().thisGridVertex.Coordinate; // ��ü ��ġ ����

                    foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>()) // ��Ƽ���� ����
                    {
                        renderer.material = AstralVFXManager.Instance.SpawnedMaterial;
                    }
                    go.GetComponent<AstralBody>().astralStats.CurrentHealth = 1;

                    GameObject go1 = Instantiate(AstralVFXManager.Instance.SpawnEffect);
                    go1.transform.position = go.transform.position;
                    go.GetComponent<AstralBody>().SetAbilityTimeToAstralTurn(go1);
                    go.GetComponent<AstralBody>().AutoDestroyVFX(go1);
                }
            }
        }
    }
}
