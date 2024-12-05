using UnityEngine;

public class AstralStatusEffect : MonoBehaviour
{
    AstralBody astralBody;

    [HideInInspector] public int stunDuration;
    [HideInInspector] public int sealDuration;
    [HideInInspector] public int declainDuration;
    [HideInInspector] public int encroachmentDuration;
    [HideInInspector] public int encroachmentDamage;
    [HideInInspector] public int punishDuration;
    [HideInInspector] public int punishLimit;

    GameObject stunEffect;
    GameObject sealEffect;
    GameObject declainEffect;
    private void Start()
    {
        astralBody = GetComponent<AstralBody>();
    }
    private void Update()
    {
        if (stunDuration <= 0)
        {
            OffStun();
        }
        if (sealDuration <= 0)
        {
            OffSeal();
        }
        if (declainDuration <= 0)
        {
            OffDeclain();
        }
        if (encroachmentDuration <= 0)
        {
            OffEncroachment();
        }
        if (punishDuration <= 0)
        {
            OffPunish();
        }
    }

    public void OnStun(int duration)
    {
        stunDuration = duration;
        stunEffect = Instantiate(AstralVFXManager.Instance.StunEffect);
        stunEffect.transform.SetParent(astralBody.gameObject.transform.GetChild(2)); // ���� ����Ʈ�� ������ ���� �������� 3��° �ڽ� ������Ʈ�� �����Ѵ�.
        stunEffect.transform.localPosition = new Vector3(0, 0, 0);

        PhaseManager.Instance.phaseStorageBattleInfo.NotifyStatusEffect(StatusEffect.Stun, astralBody.gameObject);
    }
    public void OffStun()
    {
        PhaseManager.Instance.phaseStorageBattleInfo.NotifyReleaseStatusEffect(StatusEffect.Stun, astralBody.gameObject);

        if (stunEffect != null)
        {
            Destroy(stunEffect);
        }
    }
    public void OnSeal(int duration)
    {
        sealDuration = duration;
        sealEffect = Instantiate(AstralVFXManager.Instance.SealEffect);
        sealEffect.transform.SetParent(astralBody.gameObject.transform); // ���� ����Ʈ�� ������ ���� �������� 3��° �ڽ� ������Ʈ�� �����Ѵ�.
        sealEffect.transform.localPosition = new Vector3(0, 0, 0);

        PhaseManager.Instance.phaseStorageBattleInfo.NotifyStatusEffect(StatusEffect.Stun, astralBody.gameObject);
    }
    public void OffSeal()
    {
        PhaseManager.Instance.phaseStorageBattleInfo.NotifyReleaseStatusEffect(StatusEffect.Seal, astralBody.gameObject);

        if (sealEffect != null)
        {
            Destroy(sealEffect);
        }
    }
    public void OnDeclain(int duration)
    {
        declainDuration = duration;
        declainEffect = Instantiate(AstralVFXManager.Instance.DeclainEffect);
        declainEffect.transform.SetParent(astralBody.gameObject.transform);
        declainEffect.transform.localPosition = new Vector3(0, 0, 0);

        PhaseManager.Instance.phaseStorageBattleInfo.NotifyStatusEffect(StatusEffect.Declain, astralBody.gameObject);
    }
    public void OffDeclain()
    {
        PhaseManager.Instance.phaseStorageBattleInfo.NotifyReleaseStatusEffect(StatusEffect.Declain, astralBody.gameObject);

        if (declainEffect != null)
        {
            Destroy(declainEffect);
        }
    }
    public void OnEncroachment(int duration, int damage) // PhaseManager�� Notify�� �ʿ䰡 ���� �����̻�
    {
        encroachmentDuration = duration;
        encroachmentDamage = damage;

        PhaseManager.Instance.phaseStorageBattleInfo.NotifyStatusEffect(StatusEffect.Encroachment, astralBody.gameObject);
    }
    public void OffEncroachment()
    {
        PhaseManager.Instance.phaseStorageBattleInfo.NotifyReleaseStatusEffect(StatusEffect.Encroachment, astralBody.gameObject);

        encroachmentDamage = 0;
    }
    public void OnPunish(int duration, int limit)
    {
        punishDuration = duration;
        punishLimit = limit;
    }
    public void OffPunish()
    {
        punishLimit = 0;
    }
    public void DonePunish()
    {
        GameObject punishEffect = Instantiate(AstralVFXManager.Instance.PunishEffect);
        punishEffect.transform.position = astralBody.gameObject.transform.position;
        PhaseManager.Instance.SetAstralActionTerm(punishEffect.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length); // ���� �ð��� �ִ� �ִϸ��̼��̶� ��ü �ൿ �� ���� �ð��� ������ �� �� �־ SetAstralActionTerm�� ȣ���ؾ��Ѵ�.

        PhaseManager.Instance.phaseStorageBattleInfo.NotifyStatusEffect(StatusEffect.Punish, astralBody.gameObject);
    }
    public void DoneSlain()
    {
        GameObject deadEffect = Instantiate(AstralVFXManager.Instance.DeadEffect);
        deadEffect.transform.position = astralBody.gameObject.transform.position;
        PhaseManager.Instance.SetAstralActionTerm(deadEffect.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);

        PhaseManager.Instance.phaseStorageBattleInfo.NotifyStatusEffect(StatusEffect.Slain, astralBody.gameObject);
    }

    public void DurationDecrease()
    {
        stunDuration--;
        sealDuration--;
        declainDuration--;
        punishDuration--;
        encroachmentDuration--;

        if (stunDuration < 0)
        {
            stunDuration = 0;
        }
        if (sealDuration < 0)
        {
            sealDuration = 0;
        }
        if (declainDuration < 0)
        {
            declainDuration = 0;
        }
        if (punishDuration < 0)
        {
            punishDuration = 0;
        }
        if (encroachmentDuration < 0)
        {
            encroachmentDuration = 0;
        }
    }
}
