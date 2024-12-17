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

        PhaseManager.Instance.SetAstralActionTerm(1f); // 일단 기도는 급해서 다 야매로.. 다시 전부 손봐야한다. 데미지 주는거라 일단 안전장치 삼아 해둠
    }
}
