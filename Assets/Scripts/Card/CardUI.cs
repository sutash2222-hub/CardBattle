using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Elements")]
    public Image artworkImage;
    public Image cardBackImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI descriptionText;
    public Image factionBorder;

    private CardInstance cardInstance;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private Vector2 originalPosition;
    private bool isDragging;

    public CardInstance CardInstance => cardInstance;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void Initialize(CardInstance card)
    {
        cardInstance = card;

        if (card.baseData.artwork != null)
            artworkImage.sprite = card.baseData.artwork;

        if (card.baseData.cardBack != null)
            cardBackImage.sprite = card.baseData.cardBack;

        nameText.text = card.baseData.cardName;
        powerText.text = card.currentPower.ToString();
        costText.text = card.baseData.cost.ToString();
        descriptionText.text = card.baseData.description;
    }

    public void UpdatePower(int newPower)
    {
        cardInstance.currentPower = newPower;
        powerText.text = newPower.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnCardClicked(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (BattleManager.Instance != null && BattleManager.Instance.CanPlayCard(cardInstance))
        {
            isDragging = true;
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
            originalPosition = rectTransform.anchoredPosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            if (!isOnBoard())
            {
                rectTransform.anchoredPosition = originalPosition;
            }
        }
    }

    private bool isOnBoard()
    {
        if (BattleManager.Instance != null)
        {
            return BattleManager.Instance.IsCardOnBoard(cardInstance);
        }
        return false;
    }
}
