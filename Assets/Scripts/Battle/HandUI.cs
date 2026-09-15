using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [Header("Hand Settings")]
    public Transform handContainer;
    public float cardSpacing = 100f;
    public float maxHandWidth = 800f;

    private List<CardUI> cardsInHand = new List<CardUI>();

    public void AddCard(CardInstance card)
    {
        GameObject cardPrefab = Resources.Load<GameObject>("Prefabs/Card");
        if (cardPrefab != null)
        {
            GameObject cardObj = Instantiate(cardPrefab, handContainer);
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.Initialize(card);
                cardsInHand.Add(cardUI);
                UpdateHandLayout();
            }
        }
    }

    public void RemoveCard(CardUI cardUI)
    {
        cardsInHand.Remove(cardUI);
        Destroy(cardUI.gameObject);
        UpdateHandLayout();
    }

    public void RemoveCard(CardInstance card)
    {
        CardUI cardUI = cardsInHand.Find(c => c.CardInstance == card);
        if (cardUI != null)
        {
            RemoveCard(cardUI);
        }
    }

    private void UpdateHandLayout()
    {
        float totalWidth = (cardsInHand.Count - 1) * cardSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            Vector2 newPos = new Vector2(startX + (i * cardSpacing), 0);
            cardsInHand[i].GetComponent<RectTransform>().anchoredPosition = newPos;
        }
    }

    public void ClearHand()
    {
        foreach (CardUI card in cardsInHand)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }
        cardsInHand.Clear();
    }

    public CardUI FindCardByInstance(CardInstance card)
    {
        return cardsInHand.Find(c => c.CardInstance == card);
    }
}
