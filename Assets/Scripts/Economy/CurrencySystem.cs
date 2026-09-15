using System;
using UnityEngine;

public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem Instance { get; private set; }

    [Header("Starting Currency")]
    public int startingGold = 1000;
    public int startingGems = 50;

    private int gold;
    private int gems;

    public int Gold => gold;
    public int Gems => gems;

    public event Action<int> OnGoldChanged;
    public event Action<int> OnGemsChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCurrency();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("CurrencyLoaded"))
        {
            gold = startingGold;
            gems = startingGems;
            SaveCurrency();
            PlayerPrefs.SetInt("CurrencyLoaded", 1);
        }
    }

    // Gold Methods
    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        gold += amount;
        OnGoldChanged?.Invoke(gold);
        SaveCurrency();
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0 || gold < amount) return false;
        gold -= amount;
        OnGoldChanged?.Invoke(gold);
        SaveCurrency();
        return true;
    }

    public bool HasGold(int amount)
    {
        return gold >= amount;
    }

    // Gems Methods
    public void AddGems(int amount)
    {
        if (amount <= 0) return;
        gems += amount;
        OnGemsChanged?.Invoke(gems);
        SaveCurrency();
    }

    public bool SpendGems(int amount)
    {
        if (amount <= 0 || gems < amount) return false;
        gems -= amount;
        OnGemsChanged?.Invoke(gems);
        SaveCurrency();
        return true;
    }

    public bool HasGems(int amount)
    {
        return gems >= amount;
    }

    // Exchange Methods
    public bool ExchangeGoldForGems(int goldAmount, int gemsAmount)
    {
        if (!SpendGold(goldAmount)) return false;
        AddGems(gemsAmount);
        return true;
    }

    public bool ExchangeGemsForGold(int gemsAmount, int goldAmount)
    {
        if (!SpendGems(gemsAmount)) return false;
        AddGold(goldAmount);
        return true;
    }

    // Get Methods
    public int GetGold() => gold;
    public int GetGems() => gems;

    // Reset Methods
    public void ResetCurrency()
    {
        gold = startingGold;
        gems = startingGems;
        SaveCurrency();
    }

    // Save/Load
    private void SaveCurrency()
    {
        PlayerPrefs.SetInt("Gold", gold);
        PlayerPrefs.SetInt("Gems", gems);
        PlayerPrefs.Save();
    }

    private void LoadCurrency()
    {
        gold = PlayerPrefs.GetInt("Gold", startingGold);
        gems = PlayerPrefs.GetInt("Gems", startingGems);
    }
}
