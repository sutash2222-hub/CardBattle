using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsSystem : MonoBehaviour
{
    public static AchievementsSystem Instance { get; private set; }

    [Header("Achievements")]
    public AchievementData[] achievements;

    private Dictionary<string, AchievementProgress> progress = new Dictionary<string, AchievementProgress>();

    public event Action<AchievementData> OnAchievementUnlocked;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAchievements();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateProgress(string achievementId, int amount = 1)
    {
        if (!progress.ContainsKey(achievementId))
        {
            progress[achievementId] = new AchievementProgress { currentAmount = 0 };
        }

        progress[achievementId].currentAmount += amount;
        SaveAchievements();

        // Check if achievement is unlocked
        AchievementData achievement = GetAchievement(achievementId);
        if (achievement != null && progress[achievementId].currentAmount >= achievement.requiredAmount)
        {
            if (!progress[achievementId].isUnlocked)
            {
                UnlockAchievement(achievement);
            }
        }
    }

    public void SetProgress(string achievementId, int amount)
    {
        if (!progress.ContainsKey(achievementId))
        {
            progress[achievementId] = new AchievementProgress { currentAmount = 0 };
        }

        progress[achievementId].currentAmount = amount;
        SaveAchievements();

        AchievementData achievement = GetAchievement(achievementId);
        if (achievement != null && progress[achievementId].currentAmount >= achievement.requiredAmount)
        {
            if (!progress[achievementId].isUnlocked)
            {
                UnlockAchievement(achievement);
            }
        }
    }

    private void UnlockAchievement(AchievementData achievement)
    {
        progress[achievement.id].isUnlocked = true;
        progress[achievement.id].unlockTime = DateTime.Now;
        SaveAchievements();

        // Grant rewards
        GrantReward(achievement);

        OnAchievementUnlocked?.Invoke(achievement);

        Debug.Log($"Achievement Unlocked: {achievement.name}!");
    }

    private void GrantReward(AchievementData achievement)
    {
        if (CurrencySystem.Instance != null)
        {
            CurrencySystem.Instance.AddGold(achievement.goldReward);
            CurrencySystem.Instance.AddGems(achievement.gemReward);
        }

        if (LevelSystem.Instance != null)
        {
            LevelSystem.Instance.AddXP(achievement.xpReward);
        }
    }

    public AchievementData GetAchievement(string id)
    {
        if (achievements == null) return null;
        foreach (AchievementData achievement in achievements)
        {
            if (achievement.id == id) return achievement;
        }
        return null;
    }

    public AchievementProgress GetProgress(string id)
    {
        if (progress.ContainsKey(id))
        {
            return progress[id];
        }
        return null;
    }

    public bool IsUnlocked(string id)
    {
        return progress.ContainsKey(id) && progress[id].isUnlocked;
    }

    public float GetProgressPercent(string id)
    {
        AchievementData achievement = GetAchievement(id);
        if (achievement == null) return 0f;

        AchievementProgress prog = GetProgress(id);
        if (prog == null) return 0f;

        return (float)prog.currentAmount / achievement.requiredAmount;
    }

    public int GetUnlockedCount()
    {
        int count = 0;
        foreach (var kvp in progress)
        {
            if (kvp.Value.isUnlocked) count++;
        }
        return count;
    }

    public int GetTotalAchievements()
    {
        return achievements != null ? achievements.Length : 0;
    }

    public AchievementData[] GetAllAchievements()
    {
        return achievements;
    }

    public AchievementData[] GetUnlockedAchievements()
    {
        List<AchievementData> unlocked = new List<AchievementData>();
        foreach (AchievementData achievement in achievements)
        {
            if (IsUnlocked(achievement.id))
            {
                unlocked.Add(achievement);
            }
        }
        return unlocked.ToArray();
    }

    public AchievementData[] GetLockedAchievements()
    {
        List<AchievementData> locked = new List<AchievementData>();
        foreach (AchievementData achievement in achievements)
        {
            if (!IsUnlocked(achievement.id))
            {
                locked.Add(achievement);
            }
        }
        return locked.ToArray();
    }

    private void SaveAchievements()
    {
        foreach (var kvp in progress)
        {
            PlayerPrefs.SetInt($"Achievement_{kvp.Key}_Amount", kvp.Value.currentAmount);
            PlayerPrefs.SetInt($"Achievement_{kvp.Key}_Unlocked", kvp.Value.isUnlocked ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    private void LoadAchievements()
    {
        progress.Clear();
        if (achievements == null) return;

        foreach (AchievementData achievement in achievements)
        {
            AchievementProgress prog = new AchievementProgress
            {
                currentAmount = PlayerPrefs.GetInt($"Achievement_{achievement.id}_Amount", 0),
                isUnlocked = PlayerPrefs.GetInt($"Achievement_{achievement.id}_Unlocked", 0) == 1
            };
            progress[achievement.id] = prog;
        }
    }
}

[System.Serializable]
public class AchievementProgress
{
    public int currentAmount;
    public bool isUnlocked;
    public DateTime unlockTime;
}
