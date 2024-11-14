using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/CardDataBase")]
public class CardDataBase : ScriptableObject
{
    [SerializeField]
    private List<CardData> cardDataList = new();
    public List<CardData> CardDataList
    {
        get
        {
            return cardDataList;
        }
    }
}

[Serializable]
public class CardData : ICloneable
{
    [field : SerializeField]
    public int Id { get; private set; }
    [field: SerializeField]
    public string Name { get; private set; }
    [field : SerializeField]
    public int Cost { get; private set; }
    [field: SerializeField]
    public int Health { get; private set; }
    [field: SerializeField]
    public int Mana { get; private set; }
    [field: SerializeField]
    public int Damage { get; private set; }
    [field: SerializeField]
    public float Range { get; private set; }
    [field: SerializeField]
    public string Ability { get; private set; }
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
    [field: SerializeField]
    public Sprite Thumbnail { get; private set; }
    public object Clone() // Ŭ�� �޼��带 ���� �ʱⰪ�� ���� ���� �ٸ� �ν��Ͻ��� ���� �� �ִ�.
    {
        return new CardData 
        { Id = this.Id, 
          Name = this.Name, 
          Cost = this.Cost,
          Health = this.Health,
          Mana = this.Mana,
          Damage = this.Damage, 
          Range = this.Range,
          Ability = this.Ability,
          Prefab = this.Prefab,
          Thumbnail = this.Thumbnail }; 
    }
}