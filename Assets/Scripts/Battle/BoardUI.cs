using System.Collections.Generic;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    [Header("Board Slots")]
    public Transform meleeSlot;
    public Transform rangedSlot;
    public Transform siegeSlot;

    [Header("Settings")]
    public int maxCardsPerRow = 10;

    private List<CardInstance> cardsOnBoard = new List<CardInstance>();
    private Dictionary<CardType, List<CardInstance>> rows = new Dictionary<CardType, List<CardInstance>>();

    private void Awake()
    {
        rows[CardType.Melee] = new List<CardInstance>();
        rows[CardType.Ranged] = new List<CardInstance>();
        rows[CardType.Siege] = new List<CardInstance>();
    }

    public void AddCard(CardInstance card)
    {
        cardsOnBoard.Add(card);
        card.isOnBoard = true;

        Transform slot = GetSlotForCardType(card.baseData.cardType);
        if (slot != null)
        {
            SpawnCardOnBoard(card, slot);
        }

        ApplyCardAbility(card);
        RecalculateScores();
    }

    private Transform GetSlotForCardType(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Melee: return meleeSlot;
            case CardType.Ranged: return rangedSlot;
            case CardType.Siege: return siegeSlot;
            default: return meleeSlot;
        }
    }

    private void SpawnCardOnBoard(CardInstance card, Transform parentSlot)
    {
        GameObject cardPrefab = Resources.Load<GameObject>("Prefabs/Card");
        if (cardPrefab != null)
        {
            GameObject cardObj = Instantiate(cardPrefab, parentSlot);
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.Initialize(card);
            }
        }
    }

    private void ApplyCardAbility(CardInstance card)
    {
        switch (card.baseData.ability)
        {
            case AbilityType.Bond:
                ApplyBondAbility(card);
                break;
            case AbilityType.Morale:
                ApplyMoraleAbility(card);
                break;
        }
    }

    private void ApplyBondAbility(CardInstance card)
    {
        int bondCount = 0;
        foreach (CardInstance c in cardsOnBoard)
        {
            if (c.baseData.cardName == card.baseData.cardName)
            {
                bondCount++;
            }
        }

        if (bondCount > 1)
        {
            foreach (CardInstance c in cardsOnBoard)
            {
                if (c.baseData.cardName == card.baseData.cardName)
                {
                    c.currentPower = card.baseData.power * bondCount;
                }
            }
        }
    }

    private void ApplyMoraleAbility(CardInstance card)
    {
        foreach (CardInstance c in cardsOnBoard)
        {
            if (c != card)
            {
                c.currentPower += card.baseData.abilityValue;
            }
        }
    }

    public int CalculateScore()
    {
        int totalScore = 0;
        foreach (CardInstance card in cardsOnBoard)
        {
            totalScore += card.currentPower;
        }
        return totalScore;
    }

    public bool ContainsCard(CardInstance card)
    {
        return cardsOnBoard.Contains(card);
    }

    public void ClearBoard()
    {
        foreach (CardInstance card in cardsOnBoard)
        {
            card.isOnBoard = false;
        }
        cardsOnBoard.Clear();

        rows[CardType.Melee].Clear();
        rows[CardType.Ranged].Clear();
        rows[CardType.Siege].Clear();

        foreach (Transform child in meleeSlot)
            Destroy(child.gameObject);
        foreach (Transform child in rangedSlot)
            Destroy(child.gameObject);
        foreach (Transform child in siegeSlot)
            Destroy(child.gameObject);
    }

    private void RecalculateScores()
    {
        // Reset all cards to base power first
        foreach (CardInstance card in cardsOnBoard)
        {
            card.ResetPower();
        }

        // Apply abilities
        foreach (CardInstance card in cardsOnBoard)
        {
            ApplyCardAbility(card);
        }
    }
}
