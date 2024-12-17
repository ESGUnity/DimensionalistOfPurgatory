using UnityEngine;

public class P21002 : Pray
{
    public override void CastPray()
    {
        GameObject go = CastedGridVertex.AstralOnGrid;

        if (go != null)
        {
            go.GetComponent<AstralBody>().astralStatusEffect.OnStun(5);
        }
    }
}
