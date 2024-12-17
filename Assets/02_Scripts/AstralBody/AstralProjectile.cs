using DG.Tweening;
using UnityEngine;

public class AstralProjectile : MonoBehaviour
{
    [SerializeField] bool isNotDamage = false;
    GameObject target;
    AstralBody casterAstralBody;
    [HideInInspector] public float projectileDuration;
    private void Update()
    {
        if (target != null) //Update는 Instantiate 이후 실행된다.
        {
            if (Vector3.Distance(target.transform.GetChild(0).position, transform.position) < 0.1f && target != null) // Distance기에 잘 작동하지 못할 가능성이 있다.
            {
                if (isNotDamage)
                {
                    Destroy(gameObject);
                }
                else
                {
                    target.GetComponent<AstralBody>().astralStats.Damaged(casterAstralBody.astralStats.Damage);
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }


    }
    public void SetTarget(GameObject target, AstralBody casterAstralBody)
    {
        this.target = target;
        this.casterAstralBody = casterAstralBody;

        projectileDuration = casterAstralBody.astralStats.Range * 0.15f;

        transform.position = casterAstralBody.gameObject.transform.GetChild(0).position;
        transform.DOMove(target.transform.GetChild(0).position, projectileDuration).SetEase(Ease.Linear);
        transform.LookAt(target.transform.GetChild(0).position);
        PhaseManager.Instance.SetAstralActionTerm(projectileDuration + 0.1f);
    }
}
