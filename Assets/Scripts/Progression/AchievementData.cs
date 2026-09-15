using UnityEngine;

[CreateAssetMenu(fileName = "NewAchievement", menuName = "Card Game/Achievement Data")]
public class AchievementData : ScriptableObject
{
    public string id;
    public string name;
    public string description;
    public Sprite icon;
    public AchievementCategory category;

    [Header("Requirements")]
    public int requiredAmount;
    public bool isSecret;

    [Header("Rewards")]
    public int goldReward;
    public int gemReward;
    public int xpReward;

    [Header("Progress")]
    public string progressDescription;
}

public enum AchievementCategory
{
    Battle,
    Collection,
    Social,
    Exploration,
    Special
}
