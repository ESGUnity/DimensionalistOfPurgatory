using DG.Tweening;
using UnityEngine;

public class AstralProjectile : MonoBehaviour
{
    GameObject target;
    AstralBody casterAstralBody;
    float projectileSpeed;
    private void Update()
    {
        if (Vector3.Distance(target.transform.GetChild(0).position, transform.position) < 0.1f && target != null) // Distance기에 잘 작동하지 못할 가능성이 있다.
        {
            target.GetComponent<AstralBody>().astralStats.Damaged(casterAstralBody.astralStats.Damage);
            Destroy(gameObject);
        }
        if (target == null) // 제대로 작동하지 않을 수 있다.
        {
            Destroy(gameObject);
        }
    }
    public void SetTarget(GameObject target, AstralBody casterAstralBody)
    {
        this.target = target;
        this.casterAstralBody = casterAstralBody;

        projectileSpeed = casterAstralBody.astralStats.Range * 0.2f;

        transform.position = casterAstralBody.gameObject.transform.GetChild(0).position;
        transform.DOMove(target.transform.GetChild(0).position, projectileSpeed).SetEase(Ease.Linear);
        PhaseManager.Instance.SetAstralActionTerm(projectileSpeed + 0.2f);
    }
}
