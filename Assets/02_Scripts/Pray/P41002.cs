using UnityEngine;

public class P41002 : Pray
{
    public override void CastPray()
    {
        foreach (GameObject astral in GridManager.Instance.GetAstralsInRange(CastedGridVertex, 2))
        {
            astral.GetComponent<AstralBody>().astralStatusEffect.OnIncreaseDamage(20);
        }
    }
}
