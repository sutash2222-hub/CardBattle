using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    [Header("Score Display")]
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI enemyScoreText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI turnText;

    [Header("Mana Display")]
    public TextMeshProUGUI playerManaText;
    public TextMeshProUGUI enemyManaText;
    public Image manaFillImage;

    [Header("Buttons")]
    public Button endTurnButton;
    public Button forfeitButton;
    public Button settingsButton;

    [Header("Panels")]
    public GameObject roundResultPanel;
    public GameObject gameOverPanel;
    public GameObject settingsPanel;

    [Header("Round Result")]
    public TextMeshProUGUI roundWinnerText;
    public TextMeshProUGUI finalScoreText;
    public Button continueButton;

    [Header("Game Over")]
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI rewardsText;
    public Button mainMenuButton;
    public Button playAgainButton;

    [Header("Animations")]
    public Animator turnAnimator;
    public Animator scoreAnimator;

    private int displayedPlayerScore = 0;
    private int displayedEnemyScore = 0;

    private void Start()
    {
        SetupButtons();
        HideAllPanels();
    }

    private void SetupButtons()
    {
        if (endTurnButton != null)
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
        if (forfeitButton != null)
            forfeitButton.onClick.AddListener(OnForfeitClicked);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);
    }

    private void HideAllPanels()
    {
        roundResultPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        settingsPanel?.SetActive(false);
    }

    public void UpdateScores(int playerScore, int enemyScore)
    {
        if (playerScoreText != null)
        {
            playerScoreText.text = playerScore.ToString();
            if (playerScore != displayedPlayerScore)
            {
                triggerScoreAnimation(playerScoreText);
                displayedPlayerScore = playerScore;
            }
        }

        if (enemyScoreText != null)
        {
            enemyScoreText.text = enemyScore.ToString();
            if (enemyScore != displayedEnemyScore)
            {
                triggerScoreAnimation(enemyScoreText);
                displayedEnemyScore = enemyScore;
            }
        }
    }

    public void UpdateMana(int currentMana, int maxMana)
    {
        if (playerManaText != null)
            playerManaText.text = $"{currentMana}/{maxMana}";

        if (manaFillImage != null)
            manaFillImage.fillAmount = (float)currentMana / maxMana;
    }

    public void UpdateRound(int currentRound, int maxRounds)
    {
        if (roundText != null)
            roundText.text = $"Round {currentRound}/{maxRounds}";
    }

    public void UpdateTurn(bool isPlayerTurn)
    {
        if (turnText != null)
            turnText.text = isPlayerTurn ? "YOUR TURN" : "ENEMY TURN";

        if (turnAnimator != null)
        {
            turnAnimator.SetTrigger("NewTurn");
        }

        if (endTurnButton != null)
            endTurnButton.interactable = isPlayerTurn;
    }

    private void triggerScoreAnimation(TextMeshProUGUI scoreText)
    {
        if (scoreAnimator != null)
        {
            scoreAnimator.SetTrigger("ScoreChanged");
        }

        // Flash effect
        StartCoroutine(FlashText(scoreText));
    }

    private System.Collections.IEnumerator FlashText(TextMeshProUGUI text)
    {
        Color originalColor = text.color;
        text.color = Color.yellow;

        yield return new WaitForSeconds(0.2f);

        text.color = originalColor;
    }

    public void ShowRoundResult(int playerScore, int enemyScore, bool playerWon)
    {
        HideAllPanels();
        roundResultPanel?.SetActive(true);

        if (roundWinnerText != null)
        {
            roundWinnerText.text = playerWon ? "YOU WIN!" : "YOU LOSE!";
            roundWinnerText.color = playerWon ? Color.green : Color.red;
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = $"Player: {playerScore} - Enemy: {enemyScore}";
        }
    }

    public void ShowGameOver(bool playerWon, int totalScore)
    {
        HideAllPanels();
        gameOverPanel?.SetActive(true);

        if (gameOverText != null)
        {
            gameOverText.text = playerWon ? "VICTORY!" : "DEFEAT";
            gameOverText.color = playerWon ? Color.green : Color.red;
        }

        if (rewardsText != null)
        {
            int goldReward = playerWon ? 100 : 25;
            int xpReward = playerWon ? 50 : 10;
            rewardsText.text = $"Gold: +{goldReward}\nXP: +{xpReward}";
        }
    }

    public void ShowSettings()
    {
        settingsPanel?.SetActive(true);
    }

    public void HideSettings()
    {
        settingsPanel?.SetActive(false);
    }

    public void SetEndTurnButtonInteractable(bool interactable)
    {
        if (endTurnButton != null)
            endTurnButton.interactable = interactable;
    }

    private void OnEndTurnClicked()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.EndTurn();
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }
    }

    private void OnForfeitClicked()
    {
        // Show confirmation dialog
        Debug.Log("Forfeit clicked");
    }

    private void OnSettingsClicked()
    {
        ShowSettings();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }
    }

    private void OnContinueClicked()
    {
        HideAllPanels();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }
    }

    private void OnMainMenuClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }
    }

    private void OnPlayAgainClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }
    }
}
