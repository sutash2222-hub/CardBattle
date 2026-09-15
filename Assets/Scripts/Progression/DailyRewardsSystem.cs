using System;
using UnityEngine;

public class DailyRewardsSystem : MonoBehaviour
{
    public static DailyRewardsSystem Instance { get; private set; }

    [Header("Daily Reward Settings")]
    public DailyReward[] dailyRewards;
    public int streakDaysForBonus = 7;

    private DateTime lastClaimTime;
    private int currentStreak;
    private int lastClaimedDay;

    public int CurrentStreak => currentStreak;
    public bool CanClaimToday => CanClaim();

    public event Action<DailyReward> OnRewardClaimed;
    public event Action<int> OnStreakUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadDailyData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CheckStreak();
    }

    public bool CanClaim()
    {
        if (dailyRewards == null || dailyRewards.Length == 0) return false;

        DateTime today = DateTime.Now;
        DateTime lastClaim = DateTime.Parse(PlayerPrefs.GetString("LastClaimTime", DateTime.MinValue.ToString()));

        return today.Date > lastClaim.Date;
    }

    public bool ClaimReward()
    {
        if (!CanClaim()) return false;

        int dayIndex = (lastClaimedDay + 1) % dailyRewards.Length;
        DailyReward reward = dailyRewards[dayIndex];

        // Grant reward
        GrantReward(reward);

        // Update streak
        DateTime lastClaim = DateTime.Parse(PlayerPrefs.GetString("LastClaimTime", DateTime.MinValue.ToString()));
        DateTime today = DateTime.Now;

        if ((today - lastClaim).TotalDays == 1)
        {
            currentStreak++;
        }
        else if ((today - lastClaim).TotalDays > 1)
        {
            currentStreak = 1;
        }

        // Check for streak bonus
        if (currentStreak % streakDaysForBonus == 0)
        {
            GrantStreakBonus();
        }

        // Save data
        lastClaimedDay = dayIndex;
        lastClaimTime = DateTime.Now;
        PlayerPrefs.SetString("LastClaimTime", lastClaimTime.ToString());
        PlayerPrefs.SetInt("LastClaimedDay", lastClaimedDay);
        PlayerPrefs.SetInt("CurrentStreak", currentStreak);
        PlayerPrefs.Save();

        OnRewardClaimed?.Invoke(reward);
        OnStreakUpdated?.Invoke(currentStreak);

        Debug.Log($"Claimed daily reward: {reward.rewardName} (Day {dayIndex + 1})");
        return true;
    }

    private void GrantReward(DailyReward reward)
    {
        switch (reward.rewardType)
        {
            case DailyRewardType.Gold:
                if (CurrencySystem.Instance != null)
                {
                    CurrencySystem.Instance.AddGold(reward.amount);
                }
                break;
            case DailyRewardType.Gems:
                if (CurrencySystem.Instance != null)
                {
                    CurrencySystem.Instance.AddGems(reward.amount);
                }
                break;
            case DailyRewardType.CardPack:
                if (CardPackSystem.Instance != null)
                {
                    CardPackSystem.Instance.AddPack(reward.packName, reward.amount);
                }
                break;
            case DailyRewardType.XP:
                if (LevelSystem.Instance != null)
                {
                    LevelSystem.Instance.AddXP(reward.amount);
                }
                break;
        }
    }

    private void GrantStreakBonus()
    {
        // Bonus for maintaining streak
        int bonusGold = 500 * (currentStreak / streakDaysForBonus);
        if (CurrencySystem.Instance != null)
        {
            CurrencySystem.Instance.AddGold(bonusGold);
        }

        Debug.Log($"Streak bonus! {bonusGold} gold for {currentStreak} day streak!");
    }

    private void CheckStreak()
    {
        DateTime lastClaim = DateTime.Parse(PlayerPrefs.GetString("LastClaimTime", DateTime.MinValue.ToString()));
        DateTime today = DateTime.Now;

        if ((today - lastClaim).TotalDays > 1)
        {
            currentStreak = 0;
            PlayerPrefs.SetInt("CurrentStreak", 0);
            PlayerPrefs.Save();
        }
    }

    public DailyReward GetTodayReward()
    {
        if (dailyRewards == null || dailyRewards.Length == 0) return null;
        int dayIndex = (lastClaimedDay + 1) % dailyRewards.Length;
        return dailyRewards[dayIndex];
    }

    public int GetDaysUntilStreakBonus()
    {
        return streakDaysForBonus - (currentStreak % streakDaysForBonus);
    }

    private void LoadDailyData()
    {
        lastClaimedDay = PlayerPrefs.GetInt("LastClaimedDay", -1);
        currentStreak = PlayerPrefs.GetInt("CurrentStreak", 0);
        string lastClaimStr = PlayerPrefs.GetString("LastClaimTime", DateTime.MinValue.ToString());
        lastClaimTime = DateTime.Parse(lastClaimStr);
    }
}

[System.Serializable]
public class DailyReward
{
    public string rewardName;
    public string description;
    public Sprite icon;
    public DailyRewardType rewardType;
    public int amount;
    public string packName;
}

public enum DailyRewardType
{
    Gold,
    Gems,
    CardPack,
    XP
}
