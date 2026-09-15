using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card Game/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;
    public string description;
    public Sprite artwork;
    public Sprite cardBack;

    [Header("Stats")]
    public int power;
    public int cost;
    public CardFaction faction;
    public CardType cardType;
    public CardRarity rarity;

    [Header("Abilities")]
    public AbilityType ability;
    public NewAbilityType abilityType;
    public int abilityValue;

    [Header("Lore")]
    public string flavorText;
    public string artist;
}

public enum CardFaction
{
    NorthernRealms,
    Nilfgaard,
    ScoiaTael,
    Monsters,
    Skellige,
    Neutral
}

public enum CardType
{
    Melee,
    Ranged,
    Siege,
    Special
}

public enum CardRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

public enum AbilityType
{
    None,
    Bond,
    Spy,
    Medic,
    Muster,
    TightBond,
    Morale,
    Scorch,
    Weather,
    Leader
}
