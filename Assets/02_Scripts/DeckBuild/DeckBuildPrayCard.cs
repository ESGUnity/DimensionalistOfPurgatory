using System.Xml.Linq;
using UnityEditor.Playables;
using UnityEngine;

public class DeckBuildPrayCard : DeckBuildInteractableCard
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
