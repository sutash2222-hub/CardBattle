using UnityEngine;

[CreateAssetMenu(fileName = "NewPack", menuName = "Card Game/Card Pack Data")]
public class CardPackData : ScriptableObject
{
    public string packName;
    public string description;
    public Sprite packArtwork;
    public int price;
    public CurrencyType currencyType;

    [Header("Rarity Chances (%)")]
    public float legendaryChance = 1f;
    public float epicChance = 5f;
    public float rareChance = 20f;

    [Header("Pack Contents")]
    public int cardCount = 5;
    public bool guaranteedRare = true;

    [Header("Promotions")]
    public bool isPromotional;
    public string promotionText;
    public float discountPercent;
}
