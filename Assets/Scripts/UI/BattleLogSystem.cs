using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleLogSystem : MonoBehaviour
{
    public static BattleLogSystem Instance { get; private set; }

    [Header("UI References")]
    public GameObject logPanel;
    public Transform logContainer;
    public TextMeshProUGUI logEntryPrefab;
    public ScrollRect scrollRect;
    public Button toggleButton;
    public TextMeshProUGUI toggleButtonText;

    [Header("Settings")]
    public int maxLogEntries = 50;
    public float displayDuration = 3f;

    private List<LogEntry> logEntries = new List<LogEntry>();
    private bool isExpanded = false;

    public event Action<LogEntry> OnLogAdded;

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
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleLog);
        }

        if (logPanel != null)
        {
            logPanel.SetActive(false);
        }
    }

    public void Log(string message, LogType type = LogType.Info)
    {
        LogEntry entry = new LogEntry
        {
            message = message,
            type = type,
            timestamp = DateTime.Now
        };

        logEntries.Add(entry);

        // Limit log entries
        if (logEntries.Count > maxLogEntries)
        {
            logEntries.RemoveAt(0);
        }

        // Create log entry UI
        CreateLogEntryUI(entry);

        OnLogAdded?.Invoke(entry);

        // Auto-show for important events
        if (type == LogType.Ability || type == LogType.Damage || type == LogType.Heal)
        {
            ShowLog();
        }
    }

    public void LogCardPlayed(string cardName, bool isPlayer)
    {
        string player = isPlayer ? "You" : "Enemy";
        Log($"{player} played {cardName}", LogType.CardPlayed);
    }

    public void LogDamage(string source, string target, int amount)
    {
        Log($"{source} dealt {amount} damage to {target}", LogType.Damage);
    }

    public void LogHeal(string target, int amount)
    {
        Log($"{target} healed for {amount}", LogType.Heal);
    }

    public void LogAbility(string cardName, string abilityName)
    {
        Log($"{cardName} activated {abilityName}", LogType.Ability);
    }

    public void LogTurnStart(bool isPlayerTurn)
    {
        string turn = isPlayerTurn ? "Your" : "Enemy's";
        Log($"{turn} turn started", LogType.Turn);
    }

    public void LogRoundEnd(int playerScore, int enemyScore)
    {
        Log($"Round ended! Player: {playerScore} - Enemy: {enemyScore}", LogType.Round);
    }

    public void LogGameEnd(bool playerWon)
    {
        string result = playerWon ? "Victory!" : "Defeat!";
        Log($"Game Over! {result}", LogType.Game);
    }

    private void CreateLogEntryUI(LogEntry entry)
    {
        if (logContainer == null || logEntryPrefab == null) return;

        TextMeshProUGUI logText = Instantiate(logEntryPrefab, logContainer);
        logText.text = $"[{entry.timestamp:HH:mm:ss}] {entry.message}";
        logText.color = GetLogColor(entry.type);

        // Remove old entries
        if (logContainer.childCount > maxLogEntries)
        {
            Destroy(logContainer.GetChild(0).gameObject);
        }

        // Scroll to bottom
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    private Color GetLogColor(LogType type)
    {
        switch (type)
        {
            case LogType.Info: return Color.white;
            case LogType.CardPlayed: return Color.cyan;
            case LogType.Damage: return Color.red;
            case LogType.Heal: return Color.green;
            case LogType.Ability: return Color.yellow;
            case LogType.Turn: return Color.blue;
            case LogType.Round: return Color.magenta;
            case LogType.Game: return Color.green;
            default: return Color.white;
        }
    }

    public void ToggleLog()
    {
        isExpanded = !isExpanded;
        if (logPanel != null)
        {
            logPanel.SetActive(isExpanded);
        }
        if (toggleButtonText != null)
        {
            toggleButtonText.text = isExpanded ? "Hide Log" : "Show Log";
        }
    }

    public void ShowLog()
    {
        isExpanded = true;
        if (logPanel != null)
        {
            logPanel.SetActive(true);
        }
    }

    public void HideLog()
    {
        isExpanded = false;
        if (logPanel != null)
        {
            logPanel.SetActive(false);
        }
    }

    public void ClearLog()
    {
        logEntries.Clear();
        foreach (Transform child in logContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public List<LogEntry> GetLogEntries()
    {
        return new List<LogEntry>(logEntries);
    }

    public string GetLogAsString()
    {
        string log = "";
        foreach (LogEntry entry in logEntries)
        {
            log += $"[{entry.timestamp:HH:mm:ss}] {entry.message}\n";
        }
        return log;
    }
}

[System.Serializable]
public class LogEntry
{
    public string message;
    public LogType type;
    public DateTime timestamp;
}

public enum LogType
{
    Info,
    CardPlayed,
    Damage,
    Heal,
    Ability,
    Turn,
    Round,
    Game
}
