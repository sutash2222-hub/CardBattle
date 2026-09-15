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

    [Header("Abilities")]
    public AbilityType ability;
    public int abilityValue;
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
