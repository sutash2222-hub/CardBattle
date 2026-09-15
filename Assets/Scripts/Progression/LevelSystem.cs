using System;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    public static LevelSystem Instance { get; private set; }

    [Header("Level Settings")]
    public int maxLevel = 100;
    public int startingXP = 0;
    public int xpPerWin = 50;
    public int xpPerLoss = 10;
    public int xpPerCardPlayed = 5;

    [Header("Level Rewards")]
    public LevelReward[] levelRewards;

    private int currentLevel;
    private int currentXP;
    private int xpToNextLevel;

    public int CurrentLevel => currentLevel;
    public int CurrentXP => currentXP;
    public int XPToNextLevel => xpToNextLevel;
    public float XPProgress => (float)currentXP / xpToNextLevel;

    public event Action<int> OnLevelUp;
    public event Action<int> OnXPChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLevelData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CalculateXPToNextLevel();
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        OnXPChanged?.Invoke(currentXP);

        while (currentXP >= xpToNextLevel && currentLevel < maxLevel)
        {
            LevelUp();
        }

        SaveLevelData();
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;

        CalculateXPToNextLevel();
        OnLevelUp?.Invoke(currentLevel);

        // Check for level rewards
        CheckLevelRewards();

        Debug.Log($"Level Up! Now level {currentLevel}");
    }

    private void CalculateXPToNextLevel()
    {
        // Formula: XP required = 100 * level^1.5
        xpToNextLevel = Mathf.RoundToInt(100 * Mathf.Pow(currentLevel, 1.5f));
    }

    private void CheckLevelRewards()
    {
        if (levelRewards == null) return;

        foreach (LevelReward reward in levelRewards)
        {
            if (reward.level == currentLevel)
            {
                GrantReward(reward);
            }
        }
    }

    private void GrantReward(LevelReward reward)
    {
        // Grant rewards based on type
        switch (reward.rewardType)
        {
            case RewardType.Gold:
                if (CurrencySystem.Instance != null)
                {
                    CurrencySystem.Instance.AddGold(reward.amount);
                }
                break;
            case RewardType.CardPack:
                // Grant card pack
                break;
            case RewardType.Card:
                // Grant specific card
                break;
            case RewardType.Title:
                // Grant title
                break;
        }

        Debug.Log($"Received reward: {reward.rewardName} (Level {reward.level})");
    }

    public int GetXPForMatch(bool isWinner, int cardsPlayed)
    {
        int baseXP = isWinner ? xpPerWin : xpPerLoss;
        int cardXP = cardsPlayed * xpPerCardPlayed;
        return baseXP + cardXP;
    }

    public int GetLevelFromXP(int totalXP)
    {
        int level = 1;
        int xpNeeded = 100;

        while (totalXP >= xpNeeded)
        {
            totalXP -= xpNeeded;
            level++;
            xpNeeded = Mathf.RoundToInt(100 * Mathf.Pow(level, 1.5f));
        }

        return level;
    }

    public string GetLevelTitle()
    {
        if (currentLevel < 10) return "Novice";
        if (currentLevel < 25) return "Apprentice";
        if (currentLevel < 50) return "Journeyman";
        if (currentLevel < 75) return "Expert";
        if (currentLevel < 90) return "Master";
        return "Grandmaster";
    }

    private void SaveLevelData()
    {
        PlayerPrefs.SetInt("Level", currentLevel);
        PlayerPrefs.SetInt("XP", currentXP);
        PlayerPrefs.Save();
    }

    private void LoadLevelData()
    {
        currentLevel = PlayerPrefs.GetInt("Level", 1);
        currentXP = PlayerPrefs.GetInt("XP", 0);
        CalculateXPToNextLevel();
    }

    public void ResetLevel()
    {
        currentLevel = 1;
        currentXP = 0;
        CalculateXPToNextLevel();
        SaveLevelData();
    }
}

[System.Serializable]
public class LevelReward
{
    public int level;
    public string rewardName;
    public string description;
    public RewardType rewardType;
    public int amount;
    public Sprite icon;
}

public enum RewardType
{
    Gold,
    CardPack,
    Card,
    Title,
    Emote,
    CardBack,
    Board
}
