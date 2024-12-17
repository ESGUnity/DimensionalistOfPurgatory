using System.Collections.Generic;
using UnityEngine;

public class A10003 : AstralBody
{
    public override void SubcribeCondition()
    {
        // 모든 영체 중 둘 이상 처치될 때의 조건
        PhaseManager.Instance.phaseStorageBattleInfo.SlainingPTurn += AddCondition;
        PhaseManager.Instance.phaseStorageBattleInfo.SlainingOTurn += AddCondition;
    }
    public override void UnsubscribeCondition()
    {
        PhaseManager.Instance.phaseStorageBattleInfo.SlainingPTurn -= AddCondition;
        PhaseManager.Instance.phaseStorageBattleInfo.SlainingOTurn -= AddCondition;
    }
    public override void AddCondition()
    {
        base.AddCondition();
    }
    public override void ConditionAbility()
    {
        GameObject go1 = Instantiate(AstralVFXManager.Instance.C10003);
        go1.transform.position = transform.position;
        SetAbilityTimeToAstralTurn(go1);
        AutoDestroyVFX(go1);

        List<GameObject> tempList = GridManager.Instance.GetAstralsInRange(thisGridVertex, 2);

        if (tempList.Count > 0)
        {
            foreach (GameObject go2 in tempList)
            {
                if (masterPlayerTag == "Player")
                {
                    if (go2.GetComponent<AstralBody>().masterPlayerTag == "Opponent")
                    {
                        go2.GetComponent<AstralBody>().astralStats.Damaged(50);
                    }
                }
                else if (masterPlayerTag == "Opponent")
                {
                    if (go2.GetComponent<AstralBody>().masterPlayerTag == "Player")
                    {
                        go2.GetComponent<AstralBody>().astralStats.Damaged(50);
                    }
                }
            }
        }
    }
}
