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
        if (target != null) //Update�� Instantiate ���� ����ȴ�.
        {
            if (Vector3.Distance(target.transform.GetChild(0).position, transform.position) < 0.1f && target != null) // Distance�⿡ �� �۵����� ���� ���ɼ��� �ִ�.
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
