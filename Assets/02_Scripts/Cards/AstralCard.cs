using UnityEngine;

public class AstralCard : InteractableCard
{
    public override void SetupCardData(CardData cardData)
    {
        this.cardData = cardData;
        Name.text = cardData.Name;
        Cost.text = cardData.Cost.ToString();
        Thumbnail.sprite = cardData.Thumbnail;
        Health.text = cardData.Health.ToString();

        if (cardData.Mana != 0)
        {
            Mana.text = cardData.Mana.ToString();
        }
        else
        {
            Mana.text = "X";
        }

        Damage.text = cardData.Damage.ToString();
        Range.text = cardData.Range.ToString();
        Ability.text = cardData.Ability;
    }
}
