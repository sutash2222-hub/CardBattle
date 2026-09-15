using System;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("Deck Settings")]
    public List<CardData> deckList = new List<CardData>();
    public int maxDeckSize = 30;

    private List<CardInstance> deck = new List<CardInstance>();
    private List<CardInstance> hand = new List<CardInstance>();
    private List<CardInstance> graveyard = new List<CardInstance>();

    public int CurrentMana => GetComponent<TurnManager>()?.CurrentMana ?? 0;
    public List<CardInstance> Hand => hand;
    public int DeckCount => deck.Count;

    public event Action<CardInstance> OnCardDrawn;
    public event Action<CardInstance> OnCardDiscarded;
    public event Action OnDeckEmpty;

    public void InitializeDeck()
    {
        deck.Clear();
        hand.Clear();
        graveyard.Clear();

        foreach (CardData cardData in deckList)
        {
            CardInstance card = new CardInstance(cardData);
            deck.Add(card);
        }

        ShuffleDeck();
    }

    private void ShuffleDeck()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            CardInstance temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public void DrawCard(HandUI handUI)
    {
        if (deck.Count == 0)
        {
            OnDeckEmpty?.Invoke();
            return;
        }

        CardInstance card = deck[0];
        deck.RemoveAt(0);
        hand.Add(card);

        handUI.AddCard(card);
        OnCardDrawn?.Invoke(card);
    }

    public void DrawCardToHand()
    {
        if (deck.Count == 0) return;

        CardInstance card = deck[0];
        deck.RemoveAt(0);
        hand.Add(card);

        OnCardDrawn?.Invoke(card);
    }

    public bool PlayCardFromHand(CardInstance card, BoardUI board)
    {
        if (!hand.Contains(card)) return false;
        if (card.baseData.cost > CurrentMana) return false;

        hand.Remove(card);
        board.AddCard(card);
        return true;
    }

    public void DiscardCard(CardInstance card)
    {
        hand.Remove(card);
        graveyard.Add(card);
        OnCardDiscarded?.Invoke(card);
    }

    public void SpendMana(int amount)
    {
        TurnManager turnManager = GetComponent<TurnManager>();
        if (turnManager != null)
        {
            // Mana is handled by TurnManager
        }
    }

    public void ReturnToHand(CardInstance card)
    {
        graveyard.Remove(card);
        hand.Add(card);
    }

    public void ClearGraveyard()
    {
        graveyard.Clear();
    }
}
