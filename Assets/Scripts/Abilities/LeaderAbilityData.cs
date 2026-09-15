using UnityEngine;

[CreateAssetMenu(fileName = "NewLeader", menuName = "Card Game/Leader Ability Data")]
public class LeaderAbilityData : ScriptableObject
{
    public string leaderName;
    public string abilityName;
    public string description;
    public Sprite portrait;
    public Sprite abilityIcon;
    public CardFaction faction;

    [Header("Ability Settings")]
    public LeaderAbilityType abilityType;
    public int abilityValue;
    public int cooldown;
    public bool isTargeted;
    public bool isPassive;

    [Header("Visual Effects")]
    public GameObject abilityVFX;
    public AudioClip abilitySound;
}

public enum LeaderAbilityType
{
    // Northern Realms
    Reinforce,      // Boost all units by 1
    Rally,          // Draw 2 cards
    WarHorn,        // Double row power
    TightBond,      // Double same cards

    // Nilfgaard
    ImperialGrip,   // Look at top 3 cards of deck
    Emperor'sCall,  // Play a card from deck
    BlackIris,      // Boost a unit by 5

    // Scoia'Tael
    Guerrilla,      // Return a card to hand
    Ambush,         // Set a unit's power to 0
    Waylay,         // Deal 3 damage

    // Monsters
    Crones,         // Summon all copies
    Consume,        // Destroy an ally, boost self
    Frenzy,         // Double this round's power

    // Skellige
    Storm,          // Deal 1 damage to all enemies
    Berserk,        // Boost damaged units
    Resilience,     // Keep strongest unit

    // Neutral
    None
}
