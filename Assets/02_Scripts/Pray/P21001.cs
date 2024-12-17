using UnityEngine;

public class P21001 : Pray
{
    public GameObject P21001VFX;
    public override void CastPray()
    {
        GameObject go = Instantiate(P21001VFX);
        go.transform.position = CastedGridVertex.Coordinate;

        foreach (GameObject astral in GridManager.Instance.GetAstralsInRange(CastedGridVertex, 2))
        {
            astral.GetComponent<AstralBody>().astralStats.Damaged(100);
        }

        PhaseManager.Instance.SetAstralActionTerm(1f); // �ϴ� �⵵�� ���ؼ� �� �߸ŷ�.. �ٽ� ���� �պ����Ѵ�. ������ �ִ°Ŷ� �ϴ� ������ġ ��� �ص�
    }
}
