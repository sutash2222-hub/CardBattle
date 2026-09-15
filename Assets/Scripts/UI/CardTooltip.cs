using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tooltip UI")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI abilityText;
    public TextMeshProUGUI loreText;
    public TextMeshProUGUI statsText;
    public Image artworkImage;
    public Image factionIcon;
    public Image rarityBorder;

    [Header("Tooltip Settings")]
    public Vector2 tooltipOffset = new Vector2(10, -10);
    public float showDelay = 0.5f;

    private CardInstance cardInstance;
    private float hoverTimer;
    private bool isHovering;

    private void Awake()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    public void SetCard(CardInstance card)
    {
        cardInstance = card;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        hoverTimer = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        HideTooltip();
    }

    private void Update()
    {
        if (isHovering && cardInstance != null)
        {
            hoverTimer += Time.deltaTime;
            if (hoverTimer >= showDelay)
            {
                ShowTooltip();
            }
        }
    }

    private void ShowTooltip()
    {
        if (tooltipPanel == null || cardInstance == null) return;

        // Set card info
        if (nameText != null)
            nameText.text = cardInstance.baseData.cardName;

        if (descriptionText != null)
            descriptionText.text = cardInstance.baseData.description;

        if (abilityText != null)
        {
            if (cardInstance.baseData.abilityType != NewAbilityType.None)
            {
                abilityText.text = GetAbilityDescription(cardInstance.baseData.abilityType);
                abilityText.gameObject.SetActive(true);
            }
            else
            {
                abilityText.gameObject.SetActive(false);
            }
        }

        if (loreText != null)
        {
            if (!string.IsNullOrEmpty(cardInstance.baseData.flavorText))
            {
                loreText.text = $"\"{cardInstance.baseData.flavorText}\"";
                loreText.gameObject.SetActive(true);
            }
            else
            {
                loreText.gameObject.SetActive(false);
            }
        }

        if (statsText != null)
        {
            statsText.text = $"Power: {cardInstance.currentPower}\nCost: {cardInstance.baseData.cost}";
        }

        if (artworkImage != null && cardInstance.baseData.artwork != null)
        {
            artworkImage.sprite = cardInstance.baseData.artwork;
        }

        // Set colors based on rarity
        SetRarityColors();

        // Position tooltip
        PositionTooltip();

        tooltipPanel.SetActive(true);
    }

    private void HideTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    private void PositionTooltip()
    {
        if (tooltipPanel == null) return;

        Vector3 mousePos = Input.mousePosition;
        tooltipPanel.transform.position = mousePos + (Vector3)tooltipOffset;

        // Keep tooltip on screen
        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        if (tooltipRect != null)
        {
            Vector3[] corners = new Vector3[4];
            tooltipRect.GetWorldCorners(corners);

            float maxX = Screen.width - corners[2].x;
            if (maxX < 0)
            {
                tooltipRect.position += new Vector3(maxX, 0, 0);
            }

            float minY = corners[0].y;
            if (minY < 0)
            {
                tooltipRect.position += new Vector3(0, -minY, 0);
            }
        }
    }

    private void SetRarityColors()
    {
        Color borderColor = Color.white;

        switch (cardInstance.baseData.rarity)
        {
            case CardRarity.Common:
                borderColor = new Color(0.7f, 0.7f, 0.7f); // Gray
                break;
            case CardRarity.Rare:
                borderColor = new Color(0.2f, 0.4f, 1f); // Blue
                break;
            case CardRarity.Epic:
                borderColor = new Color(0.6f, 0.2f, 0.8f); // Purple
                break;
            case CardRarity.Legendary:
                borderColor = new Color(1f, 0.8f, 0f); // Gold
                break;
        }

        if (rarityBorder != null)
        {
            rarityBorder.color = borderColor;
        }
    }

    private string GetAbilityDescription(NewAbilityType ability)
    {
        switch (ability)
        {
            case NewAbilityType.Poison:
                return "Poison: Destroys a unit after 2 turns";
            case NewAbilityType.Lock:
                return "Lock: Disables a unit's ability";
            case NewAbilityType.Boost:
                return "Boost: Increases this unit's power";
            case NewAbilityType.Drain:
                return "Drain: Steals power from an enemy";
            case NewAbilityType.Shield:
                return "Shield: Protects from next damage";
            case NewAbilityType.Barrier:
                return "Barrier: Protects adjacent units";
            case NewAbilityType.Resilience:
                return "Resilience: Stays on board between rounds";
            case NewAbilityType.Consume:
                return "Consume: Destroys an ally to boost self";
            case NewAbilityType.Deathwish:
                return "Deathwish: Triggers effect when destroyed";
            case NewAbilityType.Deploy:
                return "Deploy: Triggers effect when played";
            case NewAbilityType.Zeal:
                return "Zeal: Order can be used immediately";
            case NewAbilityType.Order:
                return "Order: Manually activated ability";
            case NewAbilityType.Purify:
                return "Purify: Removes all status effects";
            case NewAbilityType.Banish:
                return "Banish: Removes a card from game";
            case NewAbilityType.Insight:
                return "Insight: Draws cards based on hand size";
            case NewAbilityType.Provoke:
                return "Provoke: Forces enemy to attack this unit";
            case NewAbilityType.Defender:
                return "Defender: Forces enemies to attack this first";
            default:
                return "";
        }
    }
}
