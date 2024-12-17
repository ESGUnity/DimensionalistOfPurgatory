using DG.Tweening;
using System.Collections;
using UnityEngine;

public class A70001 : AstralBody
{
    public override void ManaAbility()
    {
        StartCoroutine(ManaAbilityCoroutine());
    }


    IEnumerator ManaAbilityCoroutine()
    {
        yield return new WaitForSeconds((1f / astralAnimStateMachine.manaAbilityAnimationClipLength) * 1f / 5f);

        int minHealth = int.MaxValue;
        GameObject minHealthAstral = null;

        if (masterPlayerTag == "Player")
        {
            foreach (var astral in PhaseManager.Instance.phaseStorageBattleInfo.OpponentAstral)
            {
                if (astral.GetComponent<AstralBody>().astralStats.CurrentHealth < minHealth)
                {
                    minHealth = astral.GetComponent<AstralBody>().astralStats.CurrentHealth;
                    minHealthAstral = astral;
                }
            }

            Vertex MoveVertex = GridManager.Instance.FindSpawnVertex(minHealthAstral.GetComponent<AstralBody>().thisGridVertex); // Spawn이긴 하지만 사실 빈 버텍스를 찾는 부분

            if (MoveVertex == null || MoveVertex == thisGridVertex)
            {
                StartCoroutine(ManaAbilityHitCoroutine(minHealthAstral));
            }
            else
            {
                MoveVertex.AstralOnGrid = gameObject;
                thisGridVertex.AstralOnGrid = null;
                thisGridVertex = MoveVertex;
                transform.DOMove(MoveVertex.Coordinate, (1f / astralAnimStateMachine.manaAbilityAnimationClipLength) * 3f / 5f).SetEase(Ease.Linear);
                StartCoroutine(ManaAbilityHitCoroutine(minHealthAstral));
            }
        }
        else if (masterPlayerTag == "Opponent")
        {
            foreach (var astral in PhaseManager.Instance.phaseStorageBattleInfo.PlayerAstral)
            {
                if (astral.GetComponent<AstralBody>().astralStats.CurrentHealth < minHealth)
                {
                    minHealth = astral.GetComponent<AstralBody>().astralStats.CurrentHealth;
                    minHealthAstral = astral;
                }
            }

            Vertex MoveVertex = GridManager.Instance.FindSpawnVertex(minHealthAstral.GetComponent<AstralBody>().thisGridVertex); // Spawn이긴 하지만 사실 빈 버텍스를 찾는 부분

            if (MoveVertex == null || MoveVertex == thisGridVertex)
            {
                StartCoroutine(ManaAbilityHitCoroutine(minHealthAstral));
            }
            else
            {
                MoveVertex.AstralOnGrid = gameObject;
                thisGridVertex.AstralOnGrid = null;
                thisGridVertex = MoveVertex;
                transform.DOMove(MoveVertex.Coordinate, (1f / astralAnimStateMachine.manaAbilityAnimationClipLength) * 3f / 5f).SetEase(Ease.Linear);
                StartCoroutine(ManaAbilityHitCoroutine(minHealthAstral));
            }
        }
    }

    IEnumerator ManaAbilityHitCoroutine(GameObject astral)
    {
        yield return new WaitForSeconds((1f / astralAnimStateMachine.manaAbilityAnimationClipLength) * 3f / 5f); // 이동하고 공격을 시작하는 시점

        if (astral != null) // 고유 능력이 즉시 시전이 아닌 시간이 걸린 뒤 목표 영체에게 시전함으로 null 체크가 필요하다.
        {
            if ((float)astral.GetComponent<AstralBody>().astralStats.CurrentHealth / (float)astral.GetComponent<AstralBody>().astralStats.MaxHealth <= 0.5f)
            {
                astral.GetComponent<AstralBody>().astralStats.Damaged(300);
            }
            else
            {
                astral.GetComponent<AstralBody>().astralStats.Damaged(100);
            }
        }
    }
}
