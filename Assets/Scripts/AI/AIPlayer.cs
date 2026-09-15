using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    [Header("AI Settings")]
    public float thinkTime = 1f;
    public int difficultyLevel = 1;

    private DeckManager deckManager;
    private BoardUI enemyBoard;
    private TurnManager turnManager;

    private void Awake()
    {
        deckManager = GetComponent<DeckManager>();
        turnManager = GetComponent<TurnManager>();
    }

    public IEnumerator MakeMove()
    {
        yield return new WaitForSeconds(thinkTime);

        List<CardInstance> playableCards = GetPlayableCards();

        while (playableCards.Count > 0 && turnManager.CurrentMana > 0)
        {
            CardInstance bestCard = SelectBestCard(playableCards);

            if (bestCard != null && bestCard.baseData.cost <= turnManager.CurrentMana)
            {
                PlayCard(bestCard);
                yield return new WaitForSeconds(0.5f);
                playableCards = GetPlayableCards();
            }
            else
            {
                break;
            }
        }
    }

    private List<CardInstance> GetPlayableCards()
    {
        List<CardInstance> playable = new List<CardInstance>();

        foreach (CardInstance card in deckManager.Hand)
        {
            if (card.baseData.cost <= turnManager.CurrentMana)
            {
                playable.Add(card);
            }
        }

        return playable;
    }

    private CardInstance SelectBestCard(List<CardInstance> availableCards)
    {
        if (availableCards.Count == 0) return null;

        CardInstance bestCard = availableCards[0];
        float bestScore = EvaluateCard(bestCard);

        foreach (CardInstance card in availableCards)
        {
            float score = EvaluateCard(card);
            if (score > bestScore)
            {
                bestCard = card;
                bestScore = score;
            }
        }

        return bestCard;
    }

    private float EvaluateCard(CardInstance card)
    {
        float score = 0f;

        score += card.baseData.power * 2f;
        score += card.baseData.cost * 0.5f;

        switch (card.baseData.ability)
        {
            case AbilityType.Bond:
                score += 5f;
                break;
            case AbilityType.Medic:
                score += 4f;
                break;
            case AbilityType.Spy:
                score += 3f;
                break;
            case AbilityType.Scorch:
                score += 6f;
                break;
            case AbilityType.Morale:
                score += 3f;
                break;
        }

        return score;
    }

    private void PlayCard(CardInstance card)
    {
        deckManager.PlayCardFromHand(card, enemyBoard);
        turnManager.OnCardPlayed();
    }
}
