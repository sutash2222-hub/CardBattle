using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI enemyScoreText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI turnIndicatorText;

    [Header("Panels")]
    public GameObject roundResultPanel;
    public GameObject gameOverPanel;

    private void OnEnable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnRoundStarted += UpdateRoundDisplay;
            BattleManager.Instance.OnRoundEnded += ShowRoundResult;
            BattleManager.Instance.OnBattleEnded += ShowGameOver;
        }

        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnManaChanged += UpdateManaDisplay;
            TurnManager.Instance.OnTurnStarted += UpdateTurnIndicator;
        }
    }

    private void OnDisable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnRoundStarted -= UpdateRoundDisplay;
            BattleManager.Instance.OnRoundEnded -= ShowRoundResult;
            BattleManager.Instance.OnBattleEnded -= ShowGameOver;
        }

        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnManaChanged -= UpdateManaDisplay;
            TurnManager.Instance.OnTurnStarted -= UpdateTurnIndicator;
        }
    }

    private void UpdateRoundDisplay(int currentRound, int maxRounds)
    {
        if (roundText != null)
        {
            roundText.text = $"Round {currentRound}/{maxRounds}";
        }
    }

    private void UpdateManaDisplay(int mana)
    {
        if (manaText != null)
        {
            manaText.text = $"Mana: {mana}";
        }
    }

    private void UpdateTurnIndicator(bool isPlayerTurn)
    {
        if (turnIndicatorText != null)
        {
            turnIndicatorText.text = isPlayerTurn ? "Your Turn" : "Enemy Turn";
        }
    }

    private void ShowRoundResult(int playerScore, int enemyScore)
    {
        if (roundResultPanel != null)
        {
            roundResultPanel.SetActive(true);
        }

        if (playerScoreText != null)
        {
            playerScoreText.text = $"Player: {playerScore}";
        }

        if (enemyScoreText != null)
        {
            enemyScoreText.text = $"Enemy: {enemyScore}";
        }
    }

    private void ShowGameOver(int winner)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void OnEndTurnButtonClicked()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.EndTurn();
        }
    }

    public void OnRestartButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
    }

    public void OnMainMenuButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
    }
}
