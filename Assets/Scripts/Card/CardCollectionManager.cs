using System;
using System.Collections.Generic;
using UnityEngine;

public class CardCollectionManager : MonoBehaviour
{
    public static CardCollectionManager Instance { get; private set; }

    [Header("All Cards in Game")]
    public List<CardData> allCards = new List<CardData>();

    private Dictionary<string, int> ownedCards = new Dictionary<string, int>();
    private List<CardData> favoriteCards = new List<CardData>();

    public Dictionary<string, int> OwnedCards => ownedCards;
    public List<CardData> FavoriteCards => favoriteCards;

    public event Action<string, int> OnCardCountChanged;
    public event Action<CardData> OnFavoriteToggled;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCollection();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCard(CardData card, int count = 1)
    {
        if (ownedCards.ContainsKey(card.cardName))
        {
            ownedCards[card.cardName] += count;
        }
        else
        {
            ownedCards[card.cardName] = count;
        }

        SaveCollection();
        OnCardCountChanged?.Invoke(card.cardName, ownedCards[card.cardName]);
    }

    public bool RemoveCard(CardData card, int count = 1)
    {
        if (!ownedCards.ContainsKey(card.cardName)) return false;
        if (ownedCards[card.cardName] < count) return false;

        ownedCards[card.cardName] -= count;
        if (ownedCards[card.cardName] <= 0)
        {
            ownedCards.Remove(card.cardName);
            favoriteCards.Remove(card);
        }

        SaveCollection();
        OnCardCountChanged?.Invoke(card.cardName, GetCardCount(card));
        return true;
    }

    public int GetCardCount(CardData card)
    {
        return ownedCards.ContainsKey(card.cardName) ? ownedCards[card.cardName] : 0;
    }

    public bool HasCard(CardData card)
    {
        return ownedCards.ContainsKey(card.cardName) && ownedCards[card.cardName] > 0;
    }

    public List<CardData> GetOwnedCards()
    {
        List<CardData> owned = new List<CardData>();
        foreach (var kvp in ownedCards)
        {
            CardData card = allCards.Find(c => c.cardName == kvp.Key);
            if (card != null)
            {
                for (int i = 0; i < kvp.Value; i++)
                {
                    owned.Add(card);
                }
            }
        }
        return owned;
    }

    public List<CardData> GetCardsByFaction(CardFaction faction)
    {
        return allCards.FindAll(c => c.faction == faction && HasCard(c));
    }

    public List<CardData> GetCardsByType(CardType type)
    {
        return allCards.FindAll(c => c.cardType == type && HasCard(c));
    }

    public void ToggleFavorite(CardData card)
    {
        if (favoriteCards.Contains(card))
        {
            favoriteCards.Remove(card);
        }
        else
        {
            if (favoriteCards.Count < 10)
            {
                favoriteCards.Add(card);
            }
        }

        SaveCollection();
        OnFavoriteToggled?.Invoke(card);
    }

    public bool IsFavorite(CardData card)
    {
        return favoriteCards.Contains(card);
    }

    public int GetTotalCards()
    {
        int total = 0;
        foreach (var count in ownedCards.Values)
        {
            total += count;
        }
        return total;
    }

    public int GetUniqueCardsCount()
    {
        return ownedCards.Count;
    }

    private void SaveCollection()
    {
        // Save to PlayerPrefs or file
        foreach (var kvp in ownedCards)
        {
            PlayerPrefs.SetInt($"Card_{kvp.Key}", kvp.Value);
        }
        PlayerPrefs.Save();
    }

    private void LoadCollection()
    {
        ownedCards.Clear();
        foreach (CardData card in allCards)
        {
            int count = PlayerPrefs.GetInt($"Card_{card.cardName}", 0);
            if (count > 0)
            {
                ownedCards[card.cardName] = count;
            }
        }
    }

    public void GiveStarterDeck()
    {
        // Give player some starter cards
        foreach (CardData card in allCards)
        {
            if (card.faction == CardFaction.Neutral && card.cost <= 3)
            {
                AddCard(card, 2);
            }
        }
        SaveCollection();
    }
}
