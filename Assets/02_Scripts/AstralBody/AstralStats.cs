using UnityEngine;

public class AstralStats
{
    public CardData cardData;
    public int MaxHealth;
    public int CurrentHealth;
    public int MaxMana;
    public int CurrentMana;
    public int Damage;
    public int Range;
    public int MaxRitual;
    public int CurrentRitual;
    public int MaxCondition;
    public int CurrentCondition;

    AstralBody astralBody;

    public AstralStats(CardData cardData, AstralBody astralBody)
    {
        this.cardData = cardData;
        this.astralBody = astralBody;

        MaxHealth = cardData.Health;
        CurrentHealth = MaxHealth;
        MaxMana = cardData.Mana;
        CurrentMana = 0;
        Damage = cardData.Damage;
        Range = cardData.Range;
        MaxRitual = cardData.Ritual;
        CurrentRitual = 0;
        MaxCondition = cardData.Condition;
        CurrentCondition = 0;
    }


    public void Damaged(int damage, int punishDamage = 0)
    {
        if (astralBody.state == State.Live && astralBody.astralStatusEffect.invincibilityDuration <= 0) // ����ְ� ���� ���°� �ƴ϶��
        {
            if (astralBody.astralStatusEffect.declainDuration > 0)
            {
                CurrentHealth -= damage * 2;
            }
            else
            {
                CurrentHealth -= damage;
            }

            CurrentMana += 5; // �������� ������ 5�� ������ ����.

            if (astralBody.astralStatusEffect.punishDuration > 0 && CurrentHealth <= astralBody.astralStatusEffect.punishLimit)
            {
                astralBody.astralStatusEffect.DonePunish();
                astralBody.DoDead();
                return;
            }

            if (punishDamage != 0 && CurrentHealth <= punishDamage)
            {
                astralBody.astralStatusEffect.DonePunish();
                astralBody.DoDead();
                return;
            }

            if (CurrentHealth <= 0)
            {
                astralBody.DoDead();
                return;
            }
        }
    }
}
