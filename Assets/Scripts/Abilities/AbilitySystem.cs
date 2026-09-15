using System;
using System.Collections.Generic;
using UnityEngine;

public enum NewAbilityType
{
    None,
    // Existing abilities
    Bond,
    Spy,
    Medic,
    Muster,
    TightBond,
    Morale,
    Scorch,
    Weather,
    Leader,
    // New abilities
    Poison,
    Lock,
    Boost,
    Drain,
    Shield,
    Barrier,
    Resilience,
    Consume,
    Deathwish,
    Deploy,
    Zeal,
    Order,
    Cooldown,
    Purify,
    Banish,
    Reapply,
    Insight,
    Provoke,
    Defender
}

[CreateAssetMenu(fileName = "NewAbility", menuName = "Card Game/Ability Data")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    public string description;
    public Sprite icon;
    public NewAbilityType abilityType;
    public int baseValue;
    public bool isTargeted;
    public bool isRowEffect;
}

public class AbilitySystem : MonoBehaviour
{
    public static AbilitySystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ActivateAbility(CardInstance card, BoardUI playerBoard, BoardUI enemyBoard, List<CardInstance> hand)
    {
        switch (card.baseData.abilityType)
        {
            case NewAbilityType.Poison:
                ActivatePoison(card, enemyBoard);
                break;
            case NewAbilityType.Lock:
                ActivateLock(card, enemyBoard);
                break;
            case NewAbilityType.Boost:
                ActivateBoost(card, playerBoard);
                break;
            case NewAbilityType.Drain:
                ActivateDrain(card, playerBoard, enemyBoard);
                break;
            case NewAbilityType.Shield:
                ActivateShield(card);
                break;
            case NewAbilityType.Barrier:
                ActivateBarrier(card, playerBoard);
                break;
            case NewAbilityType.Resilience:
                ActivateResilience(card);
                break;
            case NewAbilityType.Consume:
                ActivateConsume(card, playerBoard);
                break;
            case NewAbilityType.Deathwish:
                ActivateDeathwish(card, playerBoard);
                break;
            case NewAbilityType.Deploy:
                ActivateDeploy(card, playerBoard, enemyBoard);
                break;
            case NewAbilityType.Zeal:
                ActivateZeal(card);
                break;
            case NewAbilityType.Order:
                ActivateOrder(card, playerBoard, enemyBoard);
                break;
            case NewAbilityType.Purify:
                ActivatePurify(card, playerBoard);
                break;
            case NewAbilityType.Banish:
                ActivateBanish(card, enemyBoard);
                break;
            case NewAbilityType.Insight:
                ActivateInsight(card, hand);
                break;
            case NewAbilityType.Provoke:
                ActivateProvoke(card, enemyBoard);
                break;
            case NewAbilityType.Defender:
                ActivateDefender(card, playerBoard);
                break;
        }
    }

    private void ActivatePoison(CardInstance card, BoardUI enemyBoard)
    {
        // Poison destroys a unit after 2 turns
        CardInstance target = enemyBoard.GetStrongestUnit();
        if (target != null)
        {
            target.poisonCount++;
            if (target.poisonCount >= 2)
            {
                enemyBoard.RemoveCard(target);
            }
        }
    }

    private void ActivateLock(CardInstance card, BoardUI enemyBoard)
    {
        // Lock disables a unit's ability
        CardInstance target = enemyBoard.GetRandomUnit();
        if (target != null)
        {
            target.isLocked = true;
        }
    }

    private void ActivateBoost(CardInstance card, BoardUI playerBoard)
    {
        // Boost increases a unit's power
        card.currentPower += card.baseData.abilityValue;
    }

    private void ActivateDrain(CardInstance card, BoardUI playerBoard, BoardUI enemyBoard)
    {
        // Drain steals power from enemy
        CardInstance enemyStrongest = enemyBoard.GetStrongestUnit();
        if (enemyStrongest != null)
        {
            int drainAmount = Mathf.Min(card.baseData.abilityValue, enemyStrongest.currentPower - 1);
            enemyStrongest.currentPower -= drainAmount;
            card.currentPower += drainAmount;
        }
    }

    private void ActivateShield(CardInstance card)
    {
        // Shield protects from next damage
        card.hasShield = true;
    }

    private void ActivateBarrier(CardInstance card, BoardUI playerBoard)
    {
        // Barrier protects all adjacent units
        List<CardInstance> adjacentCards = playerBoard.GetAdjacentCards(card);
        foreach (CardInstance adjacent in adjacentCards)
        {
            adjacent.hasBarrier = true;
        }
    }

    private void ActivateResilience(CardInstance card)
    {
        // Resilience keeps card on board between rounds
        card.hasResilience = true;
    }

    private void ActivateConsume(CardInstance card, BoardUI playerBoard)
    {
        // Consume destroys an ally to boost self
        CardInstance weakest = playerBoard.GetWeakestUnit();
        if (weakest != null && weakest != card)
        {
            int boostAmount = weakest.currentPower;
            playerBoard.RemoveCard(weakest);
            card.currentPower += boostAmount;
        }
    }

    private void ActivateDeathwish(CardInstance card, BoardUI playerBoard)
    {
        // Deathwish triggers effect when destroyed
        // This is checked when card is destroyed
        card.hasDeathwish = true;
    }

    private void ActivateDeploy(CardInstance card, BoardUI playerBoard, BoardUI enemyBoard)
    {
        // Deploy can target any unit
        CardInstance target = enemyBoard.GetRandomUnit();
        if (target != null)
        {
            target.currentPower -= card.baseData.abilityValue;
        }
    }

    private void ActivateZeal(CardInstance card)
    {
        // Zeal allows Order to be used immediately
        card.hasZeal = true;
        card.orderReady = true;
    }

    private void ActivateOrder(CardInstance card, BoardUI playerBoard, BoardUI enemyBoard)
    {
        // Order is a manual activated ability
        if (card.orderReady)
        {
            // Example: Deal damage to an enemy
            CardInstance target = enemyBoard.GetRandomUnit();
            if (target != null)
            {
                target.currentPower -= card.baseData.abilityValue;
            }
            card.orderReady = false;
        }
    }

    private void ActivatePurify(CardInstance card, BoardUI playerBoard)
    {
        // Purify removes all status effects
        CardInstance target = playerBoard.GetRandomUnit();
        if (target != null)
        {
            target.isLocked = false;
            target.hasShield = false;
            target.hasBarrier = false;
            target.poisonCount = 0;
        }
    }

    private void ActivateBanish(CardInstance card, BoardUI enemyBoard)
    {
        // Banish removes a card from game permanently
        CardInstance target = enemyBoard.GetRandomUnit();
        if (target != null)
        {
            enemyBoard.BanishCard(target);
        }
    }

    private void ActivateInsight(CardInstance card, List<CardInstance> hand)
    {
        // Insight draws cards based on hand size
        int drawCount = hand.Count / 3;
        for (int i = 0; i < drawCount; i++)
        {
            // Draw card logic
        }
    }

    private void ActivateProvoke(CardInstance card, BoardUI enemyBoard)
    {
        // Provoke forces enemy to attack this unit
        card.isProvoked = true;
    }

    private void ActivateDefender(CardInstance card, BoardUI playerBoard)
    {
        // Defender forces enemies to attack this unit first
        card.isDefender = true;
        playerBoard.SetDefender(card);
    }
}
