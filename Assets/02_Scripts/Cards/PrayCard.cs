using UnityEngine;

public class PrayCard : InteractableCard
{
    public override void SetupCardData(CardData cardData)
    {
        this.cardData = cardData;
        Name.text = cardData.Name;
        Cost.text = cardData.Cost.ToString();
        Thumbnail.sprite = cardData.Thumbnail;
        Ability.text = cardData.Ability;
    }
}
