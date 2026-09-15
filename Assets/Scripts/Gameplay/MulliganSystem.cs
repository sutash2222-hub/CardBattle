using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MulliganSystem : MonoBehaviour
{
    public static MulliganSystem Instance { get; private set; }

    [Header("Settings")]
    public int maxMulligans = 3;
    public float selectionTimeLimit = 30f;

    [Header("UI References")]
    public GameObject mulliganPanel;
    public Transform cardContainer;
    public TextMeshProUGUI mulliganCountText;
    public TextMeshProUGUI timerText;
    public Button confirmButton;
    public Button skipButton;
    public GameObject cardPrefab;

    private List<CardInstance> currentHand = new List<CardInstance>();
    private List<CardInstance> selectedCards = new List<CardInstance>();
    private int remainingMulligans;
    private float selectionTimer;
    private bool isSelectionActive;

    public event Action<List<CardInstance>> OnMulliganComplete;
    public event Action OnMulliganSkipped;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmClicked);
        if (skipButton != null)
            skipButton.onClick.AddListener(OnSkipClicked);
    }

    public void StartMulligan(List<CardInstance> hand)
    {
        currentHand = new List<CardInstance>(hand);
        selectedCards.Clear();
        remainingMulligans = maxMulligans;
        selectionTimer = selectionTimeLimit;
        isSelectionActive = true;

        ShowMulliganUI();
        DisplayCards();
        UpdateUI();
    }

    private void ShowMulliganUI()
    {
        if (mulliganPanel != null)
        {
            mulliganPanel.SetActive(true);
        }
    }

    public void HideMulliganUI()
    {
        if (mulliganPanel != null)
        {
            mulliganPanel.SetActive(false);
        }
        isSelectionActive = false;
    }

    private void DisplayCards()
    {
        // Clear existing cards
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        // Display current hand
        foreach (CardInstance card in currentHand)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.Initialize(card);
            }

            // Add click handler
            Button cardButton = cardObj.GetComponent<Button>();
            if (cardButton != null)
            {
                CardInstance cardInstance = card;
                cardButton.onClick.AddListener(() => OnCardClicked(cardInstance));
            }

            // Highlight selected cards
            if (selectedCards.Contains(card))
            {
                cardObj.GetComponent<Image>().color = Color.yellow;
            }
        }
    }

    private void OnCardClicked(CardInstance card)
    {
        if (!isSelectionActive) return;

        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
        }
        else
        {
            if (selectedCards.Count < remainingMulligans)
            {
                selectedCards.Add(card);
            }
        }

        DisplayCards();
        UpdateUI();
    }

    private void OnConfirmClicked()
    {
        if (selectedCards.Count == 0) return;
        if (selectedCards.Count > remainingMulligans) return;

        // Replace selected cards
        List<CardInstance> newCards = new List<CardInstance>();
        foreach (CardInstance card in selectedCards)
        {
            int index = currentHand.IndexOf(card);
            if (index >= 0)
            {
                // Get new card from deck
                CardInstance newCard = DeckManager.Instance.DrawCardFromDeck();
                if (newCard != null)
                {
                    currentHand[index] = newCard;
                    newCards.Add(newCard);
                }
            }
        }

        remainingMulligans -= selectedCards.Count;
        selectedCards.Clear();

        if (remainingMulligans <= 0)
        {
            CompleteMulligan();
        }
        else
        {
            DisplayCards();
            UpdateUI();
        }
    }

    private void OnSkipClicked()
    {
        CompleteMulligan();
    }

    private void CompleteMulligan()
    {
        isSelectionActive = false;
        HideMulliganUI();
        OnMulliganComplete?.Invoke(currentHand);
    }

    private void UpdateUI()
    {
        if (mulliganCountText != null)
        {
            mulliganCountText.text = $"Mulligans: {remainingMulligans}";
        }

        if (confirmButton != null)
        {
            confirmButton.interactable = selectedCards.Count > 0 && selectedCards.Count <= remainingMulligans;
        }
    }

    private void Update()
    {
        if (isSelectionActive && selectionTimer > 0)
        {
            selectionTimer -= Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = $"Time: {Mathf.CeilToInt(selectionTimer)}";
            }

            if (selectionTimer <= 0)
            {
                CompleteMulligan();
            }
        }
    }

    public List<CardInstance> GetCurrentHand()
    {
        return new List<CardInstance>(currentHand);
    }

    public int GetRemainingMulligans()
    {
        return remainingMulligans;
    }
}
