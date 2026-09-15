using System;
using System.Collections.Generic;
using UnityEngine;

public class CardPackSystem : MonoBehaviour
{
    public static CardPackSystem Instance { get; private set; }

    [Header("Pack Settings")]
    public int cardsPerPack = 5;
    public int guaranteedRareInPacks = 10;

    [Header("Pack Types")]
    public CardPackData[] packTypes;

    private int packsOpened;
    private int packsSinceLastRare;

    public int PacksOpened => packsOpened;

    public event Action<CardPackData, List<CardInstance>> OnPackOpened;
    public event Action<CardInstance> OnLegendaryPulled;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPackData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool OpenPack(CardPackData packType)
    {
        // Check if player has enough currency
        if (CurrencySystem.Instance != null)
        {
            if (packType.currencyType == CurrencyType.Gold)
            {
                if (!CurrencySystem.Instance.HasGold(packType.price))
                {
                    Debug.Log("Not enough gold!");
                    return false;
                }
                CurrencySystem.Instance.SpendGold(packType.price);
            }
            else if (packType.currencyType == CurrencyType.Gems)
            {
                if (!CurrencySystem.Instance.HasGems(packType.price))
                {
                    Debug.Log("Not enough gems!");
                    return false;
                }
                CurrencySystem.Instance.SpendGems(packType.price);
            }
        }

        // Generate cards
        List<CardInstance> packCards = GeneratePackCards(packType);

        // Add cards to collection
        foreach (CardInstance card in packCards)
        {
            if (CardCollectionManager.Instance != null)
            {
                CardCollectionManager.Instance.AddCard(card.baseData);
            }
        }

        // Update stats
        packsOpened++;
        SavePackData();

        // Check for legendary
        foreach (CardInstance card in packCards)
        {
            if (card.baseData.rarity == CardRarity.Legendary)
            {
                OnLegendaryPulled?.Invoke(card);
            }
        }

        OnPackOpened?.Invoke(packType, packCards);

        Debug.Log($"Opened {packType.packName}! Got {packCards.Count} cards.");
        return true;
    }

    private List<CardInstance> GeneratePackCards(CardPackData packType)
    {
        List<CardInstance> cards = new List<CardInstance>();
        List<CardData> allCards = GetAllAvailableCards();

        // Guarantee rare in every 10 packs
        bool guaranteedRare = packsSinceLastRare >= guaranteedRareInPacks;

        for (int i = 0; i < cardsPerPack; i++)
        {
            CardRarity rarity = GetRandomRarity(packType, i == 0, guaranteedRare);
            CardData card = GetRandomCardByRarity(allCards, rarity);

            if (card != null)
            {
                cards.Add(new CardInstance(card));

                if (rarity == CardRarity.Rare || rarity == CardRarity.Epic || rarity == CardRarity.Legendary)
                {
                    packsSinceLastRare = 0;
                }
            }
        }

        packsSinceLastRare++;
        return cards;
    }

    private CardRarity GetRandomRarity(CardPackData packType, bool isFirstCard, bool guaranteedRare)
    {
        if (isFirstCard && guaranteedRare)
        {
            // Guaranteed rare or better on first card
            float rand = UnityEngine.Random.value;
            if (rand < 0.05f) return CardRarity.Legendary;
            if (rand < 0.25f) return CardRarity.Epic;
            return CardRarity.Rare;
        }

        // Normal rarity distribution
        float random = UnityEngine.Random.value * 100f;

        if (random < packType.legendaryChance) return CardRarity.Legendary;
        if (random < packType.legendaryChance + packType.epicChance) return CardRarity.Epic;
        if (random < packType.legendaryChance + packType.epicChance + packType.rareChance) return CardRarity.Rare;
        return CardRarity.Common;
    }

    private CardData GetRandomCardByRarity(List<CardData> cards, CardRarity rarity)
    {
        List<CardData> filteredCards = cards.FindAll(c => c.rarity == rarity);
        if (filteredCards.Count == 0) return null;
        return filteredCards[UnityEngine.Random.Range(0, filteredCards.Count)];
    }

    private List<CardData> GetAllAvailableCards()
    {
        if (CardCollectionManager.Instance != null)
        {
            return CardCollectionManager.Instance.allCards;
        }
        return new List<CardData>();
    }

    public CardPackData GetPackData(string packName)
    {
        if (packTypes == null) return null;
        foreach (CardPackData pack in packTypes)
        {
            if (pack.packName == packName) return pack;
        }
        return null;
    }

    public int GetPackCount(string packName)
    {
        return PlayerPrefs.GetInt($"Pack_{packName}", 0);
    }

    public void AddPack(string packName, int count = 1)
    {
        int current = GetPackCount(packName);
        PlayerPrefs.SetInt($"Pack_{packName}", current + count);
        PlayerPrefs.Save();
    }

    private void SavePackData()
    {
        PlayerPrefs.SetInt("PacksOpened", packsOpened);
        PlayerPrefs.Save();
    }

    private void LoadPackData()
    {
        packsOpened = PlayerPrefs.GetInt("PacksOpened", 0);
    }
}

public enum CurrencyType
{
    Gold,
    Gems
}
