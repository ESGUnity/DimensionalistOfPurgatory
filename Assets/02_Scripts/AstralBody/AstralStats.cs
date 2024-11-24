using UnityEngine;

public class AstralStats : MonoBehaviour
{
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
    public CardData cardData;

    AstralBody astralBody;

    private void Awake()
    {
        astralBody = GetComponent<AstralBody>();
    }
    public void SetAstralStats(CardData cardData)
    {
        this.cardData = cardData;
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

    public void Damaged(int damage)
    {
        CurrentHealth -= damage;
        CurrentMana += 5; // 데미지를 입으면 5의 마나가 찬다.

        if (CurrentHealth <= 0)
        {

        }
    }
}
