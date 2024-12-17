using UnityEngine;

public class AstralStatusEffect : MonoBehaviour
{
    AstralBody astralBody;

    [HideInInspector] public int stunDuration;
    [HideInInspector] public int sealDuration;
    [HideInInspector] public int declainDuration;
    [HideInInspector] public int encroachmentDuration;
    [HideInInspector] public int encroachmentDamage;
    [HideInInspector] public int increaseDamageDuration;
    [HideInInspector] public int invincibilityDuration;
    [HideInInspector] public int punishDuration;
    [HideInInspector] public int punishLimit;

    GameObject stunEffect;
    GameObject sealEffect;
    GameObject declainEffect;
    GameObject invincibilityEffect;
    private void Start()
    {
        astralBody = GetComponent<AstralBody>();
    }
    private void Update()
    {
        if (astralBody.state == State.Dead)
        {
            OffStun();
            OffSeal();
            OffDeclain();
            OffPunish();
            OffEncroachment();
            //OffInvincibility도 하기
        }

        if (stunDuration <= 0)
        {
            OffStun();
            stunDuration = 0;
        }
        if (sealDuration <= 0)
        {
            OffSeal();
            sealDuration = 0;
        }
        if (declainDuration <= 0)
        {
            OffDeclain();
            declainDuration = 0;
        }
        if (encroachmentDuration <= 0)
        {
            OffEncroachment();
            encroachmentDuration = 0;
        }
        if (punishDuration <= 0)
        {
            OffPunish();
            punishDuration = 0;
        }
        if (increaseDamageDuration <= 0)
        {
            OffIncreaseDamage();
            increaseDamageDuration = 0;
        }
        if (invincibilityDuration <= 0)
        {
            OffInvincibility();
            invincibilityDuration = 0;
        }
    }

    public void OnStun(int duration)
    {
        if (astralBody.state == State.Live)
        {
            if (stunDuration <= 0) // 중복 방지
            {
                stunEffect = Instantiate(AstralVFXManager.Instance.StunEffect);
                stunEffect.GetComponent<VFXFollowParent>().IsStun = true;
                stunEffect.GetComponent<VFXFollowParent>().SetParentObject(astralBody.gameObject);

                PhaseManager.Instance.phaseStorageBattleInfo.NotifyStatusEffect(StatusEffect.Stun, astralBody.gameObject);
            }

            stunDuration += duration;
        }
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
        if (astralBody.state == State.Live)
        {
            if (sealDuration <= 0)
            {
                sealEffect = Instantiate(AstralVFXManager.Instance.SealEffect);
                sealEffect.GetComponent<VFXFollowParent>().SetParentObject(astralBody.gameObject);

                PhaseManager.Instance.phaseStorageBattleInfo.NotifyStatusEffect(StatusEffect.Seal, astralBody.gameObject);
            }

            sealDuration += duration;
        }
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
        if (astralBody.state == State.Live)
        {
            if (declainDuration <= 0)
            {
                declainEffect = Instantiate(AstralVFXManager.Instance.DeclainEffect);
                declainEffect.GetComponent<VFXFollowParent>().SetParentObject(astralBody.gameObject);

                PhaseManager.Instance.phaseStorageBattleInfo.NotifyStatusEffect(StatusEffect.Declain, astralBody.gameObject);
            }

            declainDuration += duration;
        }
    }
    public void OffDeclain()
    {
        PhaseManager.Instance.phaseStorageBattleInfo.NotifyReleaseStatusEffect(StatusEffect.Declain, astralBody.gameObject);

        if (declainEffect != null)
        {
            Destroy(declainEffect);
        }
    }
    public void OnEncroachment(int duration, int damage) // PhaseManager에 Notify하는 이유는 이후 정화할 대상을 쉽게 찾기 위함(All Duration = 0)
    {
        if (astralBody.state == State.Live)
        {
            if (encroachmentDuration <= 0)
            {
                // 침식도 이후 VFX 추가하자.
                PhaseManager.Instance.phaseStorageBattleInfo.NotifyStatusEffect(StatusEffect.Encroachment, astralBody.gameObject);
            }

            encroachmentDamage = damage;
            encroachmentDuration += duration; // 침식의 중첩 처리는 어떻게 할지 고민하자.
        }
    }
    public void OffEncroachment()
    {
        PhaseManager.Instance.phaseStorageBattleInfo.NotifyReleaseStatusEffect(StatusEffect.Encroachment, astralBody.gameObject);
        // VFX 파괴도 꼭 넣기
        encroachmentDamage = 0;
    }
    public void OnIncreaseDamage(int damage) // PhaseManager에 Notify하는 이유는 이후 디스펠할 대상을 쉽게 찾기 위함(All Duration = 0)
    {
        if (astralBody.state == State.Live)
        {
            if (encroachmentDuration <= 0)
            {
                PhaseManager.Instance.phaseStorageBattleInfo.NotifyStatusEffect(StatusEffect.IncreaseDamage, astralBody.gameObject); // 상태이상에 등록하는건 한 번만 되야하기에 다음과 같은 코드 작성
            }

            GameObject increaseDamageEffect = Instantiate(AstralVFXManager.Instance.IncreaseDamageEffect);
            increaseDamageEffect.GetComponent<VFXFollowParent>().SetParentObject(astralBody.gameObject);

            astralBody.SetAbilityTimeToAstralTurn(increaseDamageEffect);
            astralBody.AutoDestroyVFX(increaseDamageEffect);

            astralBody.astralStats.Damage += damage;
            increaseDamageDuration = int.MaxValue; // 공격력 증가는 영구적이다. 또한 계속해서 존재하는 VFX는 없다. duration을 0으로 하면 Off 된다. (디스펠 및 정화)
        }
    }
    public void OffIncreaseDamage()
    {
        PhaseManager.Instance.phaseStorageBattleInfo.NotifyReleaseStatusEffect(StatusEffect.IncreaseDamage, astralBody.gameObject);

        astralBody.astralStats.Damage = astralBody.astralStats.cardData.Damage; // 공격력을 원래대로 되돌리기
    }
    public void OnInvincibility(int duration)
    {
        if (astralBody.state == State.Live)
        {
            if (invincibilityDuration <= 0)
            {
                invincibilityEffect = Instantiate(AstralVFXManager.Instance.InvincibilityEffect);
                invincibilityEffect.GetComponent<VFXFollowParent>().SetParentObject(astralBody.gameObject);

                PhaseManager.Instance.phaseStorageBattleInfo.NotifyStatusEffect(StatusEffect.Invincibility, astralBody.gameObject);
            }

            invincibilityDuration += duration;
        }
    }
    public void OffInvincibility()
    {
        PhaseManager.Instance.phaseStorageBattleInfo.NotifyReleaseStatusEffect(StatusEffect.Invincibility, astralBody.gameObject);

        if (invincibilityEffect != null)
        {
            Destroy(invincibilityEffect);
        }
    }
    public void OnHeal(int heal) // PhaseManager에 Notify하는 이유는 이후 디스펠할 대상을 쉽게 찾기 위함(All Duration = 0)
    {
        if (astralBody.state == State.Live)
        {
            astralBody.astralStats.CurrentHealth += heal;
            if (astralBody.astralStats.CurrentHealth > astralBody.astralStats.MaxHealth)
            {
                astralBody.astralStats.CurrentHealth = astralBody.astralStats.MaxHealth;
            }
            GameObject healEffect = Instantiate(AstralVFXManager.Instance.HealEffect);
            healEffect.GetComponent<VFXFollowParent>().SetParentObject(astralBody.gameObject);

            astralBody.SetAbilityTimeToAstralTurn(healEffect);
            astralBody.AutoDestroyVFX(healEffect);
        }
    }

    public void OnPunish(int duration, int limit)
    {
        if (astralBody.state == State.Live)
        {
            punishDuration += duration;
            punishLimit = limit; // encroach처럼 이 수치를 어떻게 할지 고민되네
        }
    }
    public void OffPunish()
    {
        punishLimit = 0;
    }
    public void DonePunish()
    {
        GameObject punishEffect = Instantiate(AstralVFXManager.Instance.PunishEffect);
        punishEffect.GetComponent<VFXFollowParent>().SetParentObject(astralBody.gameObject);

        astralBody.SetAbilityTimeToAstralTurn(punishEffect);
        astralBody.AutoDestroyVFX(punishEffect);

        PhaseManager.Instance.phaseStorageBattleInfo.NotifyStatusEffect(StatusEffect.Punish, astralBody.gameObject);
    }
    public void DoneSlain()
    {
        GameObject deadEffect = Instantiate(AstralVFXManager.Instance.DeadEffect);
        deadEffect.GetComponent<VFXFollowParent>().SetParentObject(astralBody.gameObject);

        astralBody.SetAbilityTimeToAstralTurn(deadEffect);
        astralBody.AutoDestroyVFX(deadEffect);

        PhaseManager.Instance.phaseStorageBattleInfo.NotifyStatusEffect(StatusEffect.Slain, astralBody.gameObject);

        // 처치된 후 상태이상 VFX를 끄고 페이즈 전투 정보에 상태 이상 정보를 빼는 코드
        OffStun();
        OffSeal();
        OffDeclain();
        OffEncroachment();
        OffIncreaseDamage();
        OffInvincibility();
        OffPunish();
    }
    public void OnCleanse()
    {
        stunDuration = 0;
        sealDuration = 0;
        declainDuration = 0;
        encroachmentDuration = 0;
        punishDuration = 0;
        OffStun();
        OffSeal();
        OffDeclain();
        OffEncroachment();
        OffPunish();

        GameObject cleanseEffect = Instantiate(AstralVFXManager.Instance.CleanseEffect);
        cleanseEffect.GetComponent<VFXFollowParent>().SetParentObject(astralBody.gameObject);

        astralBody.SetAbilityTimeToAstralTurn(cleanseEffect);
        astralBody.AutoDestroyVFX(cleanseEffect);
    }
    public bool IsDisrupted()
    {
        if (stunDuration > 0 || sealDuration > 0 || declainDuration > 0 || encroachmentDuration > 0 || punishDuration > 0)
        {
            return true;
        }

        return false;
    }
    public void DurationDecrease()
    {
        stunDuration--;
        sealDuration--;
        declainDuration--;
        punishDuration--;
        encroachmentDuration--;
        invincibilityDuration--;

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
        if (encroachmentDuration < 0)
        {
            encroachmentDuration = 0;
        }
        if (increaseDamageDuration < 0)
        {
            increaseDamageDuration = 0;
        }
        if (invincibilityDuration < 0)
        {
            invincibilityDuration = 0;
        }
        if (punishDuration < 0)
        {
            punishDuration = 0;
        }
    }
}
