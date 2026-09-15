using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDeck", menuName = "Card Game/Deck Data")]
public class DeckData : ScriptableObject
{
    public string deckName;
    public string description;
    public List<CardData> cards = new List<CardData>();

    public int CardCount => cards.Count;

    public bool IsValid()
    {
        return cards.Count >= 20 && cards.Count <= 30;
    }

    public DeckData Clone()
    {
        DeckData newDeck = CreateInstance<DeckData>();
        newDeck.deckName = deckName;
        newDeck.description = description;
        newDeck.cards = new List<CardData>(cards);
        return newDeck;
    }

    public void AddCard(CardData card)
    {
        if (!cards.Contains(card))
        {
            cards.Add(card);
        }
    }

    public void RemoveCard(CardData card)
    {
        cards.Remove(card);
    }

    public List<CardData> GetRandomDeck(int count)
    {
        List<CardData> randomDeck = new List<CardData>(cards);
        for (int i = randomDeck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CardData temp = randomDeck[i];
            randomDeck[i] = randomDeck[randomIndex];
            randomDeck[randomIndex] = temp;
        }
        return randomDeck.GetRange(0, Mathf.Min(count, randomDeck.Count));
    }
}
