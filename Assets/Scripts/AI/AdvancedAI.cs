using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float thinkTime = 1f;
    public int difficultyLevel = 1;
    public bool useComboStrategy = true;
    public bool useCounterStrategy = true;

    private DeckManager deckManager;
    private BoardUI enemyBoard;
    private BoardUI playerBoard;
    private TurnManager turnManager;

    private void Awake()
    {
        deckManager = GetComponent<DeckManager>();
        turnManager = GetComponent<TurnManager>();
    }

    public IEnumerator MakeMove()
    {
        yield return new WaitForSeconds(thinkTime);

        // Analyze board state
        BoardState playerState = AnalyzeBoard(playerBoard);
        BoardState enemyState = AnalyzeBoard(enemyBoard);

        // Choose strategy based on difficulty
        switch (difficultyLevel)
        {
            case 1:
                yield return StartCoroutine(EasyStrategy());
                break;
            case 2:
                yield return StartCoroutine(MediumStrategy());
                break;
            case 3:
                yield return StartCoroutine(HardStrategy(playerState, enemyState));
                break;
        }
    }

    private IEnumerator EasyStrategy()
    {
        List<CardInstance> playableCards = GetPlayableCards();

        // Play random playable cards
        while (playableCards.Count > 0 && turnManager.CurrentMana > 0)
        {
            int randomIndex = Random.Range(0, playableCards.Count);
            CardInstance card = playableCards[randomIndex];

            if (card.baseData.cost <= turnManager.CurrentMana)
            {
                PlayCard(card);
                yield return new WaitForSeconds(0.5f);
                playableCards = GetPlayableCards();
            }
            else
            {
                break;
            }
        }
    }

    private IEnumerator MediumStrategy()
    {
        List<CardInstance> playableCards = GetPlayableCards();

        // Sort by power/cost ratio
        playableCards.Sort((a, b) =>
        {
            float ratioA = (float)a.baseData.power / a.baseData.cost;
            float ratioB = (float)b.baseData.power / b.baseData.cost;
            return ratioB.CompareTo(ratioA);
        });

        while (playableCards.Count > 0 && turnManager.CurrentMana > 0)
        {
            CardInstance bestCard = playableCards[0];

            if (bestCard.baseData.cost <= turnManager.CurrentMana)
            {
                PlayCard(bestCard);
                yield return new WaitForSeconds(0.5f);
                playableCards = GetPlayableCards();
                playableCards.Sort((a, b) =>
                {
                    float ratioA = (float)a.baseData.power / a.baseData.cost;
                    float ratioB = (float)b.baseData.power / b.baseData.cost;
                    return ratioB.CompareTo(ratioA);
                });
            }
            else
            {
                break;
            }
        }
    }

    private IEnumerator HardStrategy(BoardState playerState, BoardState playerState)
    {
        List<CardInstance> playableCards = GetPlayableCards();

        // Calculate threat level
        int playerScore = playerBoard.CalculateScore();
        int enemyScore = enemyBoard.CalculateScore();
        bool isLosing = playerScore > enemyScore;

        // Prioritize cards based on situation
        playableCards.Sort((a, b) =>
        {
            float scoreA = EvaluateCardForSituation(a, isLosing);
            float scoreB = EvaluateCardForSituation(b, isLosing);
            return scoreB.CompareTo(scoreA);
        });

        while (playableCards.Count > 0 && turnManager.CurrentMana > 0)
        {
            CardInstance bestCard = SelectBestCardForSituation(playableCards, isLosing);

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

    private float EvaluateCardForSituation(CardInstance card, bool isLosing)
    {
        float score = 0f;

        // Base power score
        score += card.baseData.power * 2f;

        // Cost efficiency
        score += (float)card.baseData.power / card.baseData.cost * 3f;

        // Ability bonuses
        if (isLosing)
        {
            // Prioritize high power and removal when losing
            switch (card.baseData.ability)
            {
                case AbilityType.Scorch:
                    score += 10f; // High priority when losing
                    break;
                case AbilityType.Bond:
                    score += 8f; // Big combos
                    break;
                case AbilityType.Morale:
                    score += 5f;
                    break;
                case AbilityType.Medic:
                    score += 6f;
                    break;
            }
        }
        else
        {
            // Prioritize defensive cards when winning
            switch (card.baseData.ability)
            {
                case AbilityType.Medic:
                    score += 8f;
                    break;
                case AbilityType.Morale:
                    score += 6f;
                    break;
                case AbilityType.Bond:
                    score += 4f;
                    break;
            }
        }

        // Combo detection
        if (useComboStrategy)
        {
            int sameCardsInHand = CountSameCardsInHand(card);
            int sameCardsOnBoard = CountSameCardsOnBoard(card);

            if (card.baseData.ability == AbilityType.Bond)
            {
                score += (sameCardsInHand + sameCardsOnBoard) * 3f;
            }
        }

        return score;
    }

    private CardInstance SelectBestCardForSituation(List<CardInstance> cards, bool isLosing)
    {
        if (cards.Count == 0) return null;

        CardInstance bestCard = cards[0];
        float bestScore = EvaluateCardForSituation(bestCard, isLosing);

        foreach (CardInstance card in cards)
        {
            if (card.baseData.cost <= turnManager.CurrentMana)
            {
                float score = EvaluateCardForSituation(card, isLosing);
                if (score > bestScore)
                {
                    bestCard = card;
                    bestScore = score;
                }
            }
        }

        return bestCard;
    }

    private int CountSameCardsInHand(CardInstance card)
    {
        int count = 0;
        foreach (CardInstance c in deckManager.Hand)
        {
            if (c.baseData.cardName == card.baseData.cardName)
            {
                count++;
            }
        }
        return count;
    }

    private int CountSameCardsOnBoard(CardInstance card)
    {
        int count = 0;
        foreach (CardInstance c in enemyBoard.GetCardsOnBoard())
        {
            if (c.baseData.cardName == card.baseData.cardName)
            {
                count++;
            }
        }
        return count;
    }

    private BoardState AnalyzeBoard(BoardUI board)
    {
        BoardState state = new BoardState();
        state.totalPower = board.CalculateScore();
        state.cardCount = board.GetCardsOnBoard().Count;
        state.hasMedic = board.HasCardWithAbility(AbilityType.Medic);
        state.hasSpy = board.HasCardWithAbility(AbilityType.Spy);
        return state;
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

    private void PlayCard(CardInstance card)
    {
        deckManager.PlayCardFromHand(card, enemyBoard);
        turnManager.OnCardPlayed();
    }

    public void SetDifficulty(int level)
    {
        difficultyLevel = Mathf.Clamp(level, 1, 3);
    }
}

[System.Serializable]
public class BoardState
{
    public int totalPower;
    public int cardCount;
    public bool hasMedic;
    public bool hasSpy;
    public bool hasScorch;
}
