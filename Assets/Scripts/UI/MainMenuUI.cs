using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject collectionPanel;
    public GameObject deckBuilderPanel;
    public GameObject settingsPanel;
    public GameObject statsPanel;

    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button collectionButton;
    public Button deckBuilderButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Collection UI")]
    public Transform collectionGrid;
    public TextMeshProUGUI totalCardsText;
    public TextMeshProUGUI uniqueCardsText;
    public GameObject cardPrefab;

    [Header("Stats UI")]
    public TextMeshProUGUI gamesPlayedText;
    public TextMeshProUGUI gamesWonText;
    public TextMeshProUGUI winRateText;
    public TextMeshProUGUI favoriteCardText;

    private void Start()
    {
        InitializeMainMenu();
        ShowPanel(mainPanel);
    }

    private void InitializeMainMenu()
    {
        // Setup button listeners
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);
        if (collectionButton != null)
            collectionButton.onClick.AddListener(OnCollectionClicked);
        if (deckBuilderButton != null)
            deckBuilderButton.onClick.AddListener(OnDeckBuilderClicked);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        // Load stats
        UpdateStats();
    }

    private void ShowPanel(GameObject panel)
    {
        mainPanel?.SetActive(false);
        collectionPanel?.SetActive(false);
        deckBuilderPanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        statsPanel?.SetActive(false);

        panel?.SetActive(true);
    }

    public void OnPlayClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartBattle();
        }
    }

    public void OnCollectionClicked()
    {
        ShowPanel(collectionPanel);
        RefreshCollection();
    }

    public void OnDeckBuilderClicked()
    {
        ShowPanel(deckBuilderPanel);
    }

    public void OnSettingsClicked()
    {
        ShowPanel(settingsPanel);
    }

    public void OnQuitClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnBackClicked()
    {
        ShowPanel(mainPanel);
    }

    private void RefreshCollection()
    {
        if (CardCollectionManager.Instance == null) return;

        // Clear grid
        foreach (Transform child in collectionGrid)
        {
            Destroy(child.gameObject);
        }

        // Display all owned cards
        var ownedCards = CardCollectionManager.Instance.GetOwnedCards();
        foreach (CardData card in ownedCards)
        {
            GameObject cardObj = Instantiate(cardPrefab, collectionGrid);
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.Initialize(new CardInstance(card));
            }
        }

        // Update stats
        if (totalCardsText != null)
            totalCardsText.text = $"Total Cards: {CardCollectionManager.Instance.GetTotalCards()}";

        if (uniqueCardsText != null)
            uniqueCardsText.text = $"Unique Cards: {CardCollectionManager.Instance.GetUniqueCardsCount()}";
    }

    private void UpdateStats()
    {
        int gamesPlayed = PlayerPrefs.GetInt("Stats_GamesPlayed", 0);
        int gamesWon = PlayerPrefs.GetInt("Stats_GamesWon", 0);
        float winRate = gamesPlayed > 0 ? (float)gamesWon / gamesPlayed * 100f : 0f;

        if (gamesPlayedText != null)
            gamesPlayedText.text = $"Games Played: {gamesPlayed}";

        if (gamesWonText != null)
            gamesWonText.text = $"Games Won: {gamesWon}";

        if (winRateText != null)
            winRateText.text = $"Win Rate: {winRate:F1}%";

        if (favoriteCardText != null)
        {
            string favCard = PlayerPrefs.GetString("FavoriteCard", "None");
            favoriteCardText.text = $"Favorite Card: {favCard}";
        }
    }

    public void UpdateStatsDisplay()
    {
        UpdateStats();
    }
}
