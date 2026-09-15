using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckBuilderUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform cardGrid;
    public Transform deckSlots;
    public TextMeshProUGUI deckCountText;
    public TextMeshProUGUI deckNameText;
    public GameObject cardPrefab;
    public GameObject deckSlotPrefab;

    [Header("Filters")]
    public TMP_Dropdown factionFilter;
    public TMP_Dropdown typeFilter;
    public TMP_InputField searchInput;

    private List<CardData> availableCards = new List<CardData>();
    private List<CardData> currentDeck = new List<CardData>();
    private int maxDeckSize = 30;
    private int minDeckSize = 20;

    private void Start()
    {
        InitializeDeckBuilder();
        SetupFilters();
    }

    private void InitializeDeckBuilder()
    {
        if (CardCollectionManager.Instance != null)
        {
            availableCards = CardCollectionManager.Instance.GetOwnedCards();
        }

        RefreshCardGrid();
        UpdateDeckCount();
    }

    private void SetupFilters()
    {
        if (factionFilter != null)
        {
            factionFilter.ClearOptions();
            List<string> factions = new List<string> { "All", "Northern Realms", "Nilfgaard", "Scoia'Tael", "Monsters", "Skellige", "Neutral" };
            factionFilter.AddOptions(factions);
            factionFilter.onValueChanged.AddListener(OnFactionFilterChanged);
        }

        if (typeFilter != null)
        {
            typeFilter.ClearOptions();
            List<string> types = new List<string> { "All", "Melee", "Ranged", "Siege", "Special" };
            typeFilter.AddOptions(types);
            typeFilter.onValueChanged.AddListener(OnTypeFilterChanged);
        }

        if (searchInput != null)
        {
            searchInput.onValueChanged.AddListener(OnSearchChanged);
        }
    }

    private void RefreshCardGrid()
    {
        foreach (Transform child in cardGrid)
        {
            Destroy(child.gameObject);
        }

        foreach (CardData card in availableCards)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardGrid);
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.Initialize(new CardInstance(card));
            }

            Button addBtn = cardObj.GetComponent<Button>();
            if (addBtn != null)
            {
                CardData cardData = card;
                addBtn.onClick.AddListener(() => AddCardToDeck(cardData));
            }
        }
    }

    public void AddCardToDeck(CardData card)
    {
        if (currentDeck.Count >= maxDeckSize) return;

        if (CardCollectionManager.Instance != null)
        {
            int owned = CardCollectionManager.Instance.GetCardCount(card);
            int inDeck = currentDeck.FindAll(c => c.cardName == card.cardName).Count;

            if (inDeck >= owned) return;
        }

        currentDeck.Add(card);
        RefreshDeckSlots();
        UpdateDeckCount();
    }

    public void RemoveCardFromDeck(int index)
    {
        if (index >= 0 && index < currentDeck.Count)
        {
            currentDeck.RemoveAt(index);
            RefreshDeckSlots();
            UpdateDeckCount();
        }
    }

    private void RefreshDeckSlots()
    {
        foreach (Transform child in deckSlots)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < currentDeck.Count; i++)
        {
            GameObject slotObj = Instantiate(deckSlotPrefab, deckSlots);
            CardUI cardUI = slotObj.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.Initialize(new CardInstance(currentDeck[i]));
            }

            Button removeBtn = slotObj.GetComponent<Button>();
            if (removeBtn != null)
            {
                int index = i;
                removeBtn.onClick.AddListener(() => RemoveCardFromDeck(index));
            }
        }
    }

    private void UpdateDeckCount()
    {
        if (deckCountText != null)
        {
            deckCountText.text = $"{currentDeck.Count}/{maxDeckSize}";

            if (currentDeck.Count < minDeckSize)
            {
                deckCountText.color = Color.red;
            }
            else
            {
                deckCountText.color = Color.green;
            }
        }
    }

    private void OnFactionFilterChanged(int index)
    {
        ApplyFilters();
    }

    private void OnTypeFilterChanged(int index)
    {
        ApplyFilters();
    }

    private void OnSearchChanged(string search)
    {
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        List<CardData> filtered = new List<CardData>(availableCards);

        // Faction filter
        if (factionFilter != null && factionFilter.value > 0)
        {
            CardFaction faction = (CardFaction)(factionFilter.value - 1);
            filtered = filtered.FindAll(c => c.faction == faction);
        }

        // Type filter
        if (typeFilter != null && typeFilter.value > 0)
        {
            CardType type = (CardType)(typeFilter.value - 1);
            filtered = filtered.FindAll(c => c.cardType == type);
        }

        // Search filter
        if (searchInput != null && !string.IsNullOrEmpty(searchInput.text))
        {
            string search = searchInput.text.ToLower();
            filtered = filtered.FindAll(c => c.cardName.ToLower().Contains(search));
        }

        availableCards = filtered;
        RefreshCardGrid();
    }

    public void ClearDeck()
    {
        currentDeck.Clear();
        RefreshDeckSlots();
        UpdateDeckCount();
    }

    public void SaveDeck()
    {
        if (currentDeck.Count < minDeckSize)
        {
            Debug.Log($"Deck needs at least {minDeckSize} cards!");
            return;
        }

        // Save deck to PlayerPrefs
        for (int i = 0; i < currentDeck.Count; i++)
        {
            PlayerPrefs.SetString($"Deck_Card_{i}", currentDeck[i].cardName);
        }
        PlayerPrefs.SetInt("Deck_Size", currentDeck.Count);
        PlayerPrefs.Save();

        Debug.Log("Deck saved!");
    }

    public void LoadDeck()
    {
        currentDeck.Clear();
        int deckSize = PlayerPrefs.GetInt("Deck_Size", 0);

        for (int i = 0; i < deckSize; i++)
        {
            string cardName = PlayerPrefs.GetString($"Deck_Card_{i}", "");
            CardData card = availableCards.Find(c => c.cardName == cardName);
            if (card != null)
            {
                currentDeck.Add(card);
            }
        }

        RefreshDeckSlots();
        UpdateDeckCount();
    }

    public List<CardData> GetCurrentDeck()
    {
        return new List<CardData>(currentDeck);
    }
}
