using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("References")]
    public HandUI playerHand;
    public BoardUI playerBoard;
    public BoardUI enemyBoard;
    public TurnManager turnManager;
    public DeckManager playerDeck;
    public DeckManager enemyDeck;

    [Header("Battle Settings")]
    public int maxRounds = 3;
    public int cardsToDraw = 10;

    private int playerScore;
    private int enemyScore;
    private int currentRound;
    private bool isBattleActive;

    public int PlayerScore => playerScore;
    public int EnemyScore => enemyScore;
    public int CurrentRound => currentRound;
    public bool IsBattleActive => isBattleActive;

    public event Action<int, int> OnRoundStarted;
    public event Action<int, int> OnRoundEnded;
    public event Action<int> OnBattleEnded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartBattle();
    }

    public void StartBattle()
    {
        isBattleActive = true;
        currentRound = 1;
        playerScore = 0;
        enemyScore = 0;

        playerDeck.InitializeDeck();
        enemyDeck.InitializeDeck();

        StartCoroutine(StartRound());
    }

    private IEnumerator StartRound()
    {
        playerBoard.ClearBoard();
        enemyBoard.ClearBoard();

        for (int i = 0; i < cardsToDraw; i++)
        {
            playerDeck.DrawCard(playerHand);
            enemyDeck.DrawCardToHand();
        }

        OnRoundStarted?.Invoke(currentRound, maxRounds);

        turnManager.StartPlayerTurn();

        yield return new WaitUntil(() => turnManager.IsRoundComplete);

        EndRound();
    }

    private void EndRound()
    {
        CalculateScores();

        OnRoundEnded?.Invoke(playerScore, enemyScore);

        if (currentRound >= maxRounds || playerScore >= 2 || enemyScore >= 2)
        {
            EndBattle();
        }
        else
        {
            currentRound++;
            StartCoroutine(StartRound());
        }
    }

    private void CalculateScores()
    {
        playerScore = playerBoard.CalculateScore();
        int enemyRoundScore = enemyBoard.CalculateScore();

        if (playerScore > enemyRoundScore)
            playerScore++;
        else if (enemyRoundScore > playerScore)
            enemyScore++;
    }

    private void EndBattle()
    {
        isBattleActive = false;
        int winner = playerScore > enemyScore ? 1 : (playerScore < enemyScore ? 2 : 0);
        OnBattleEnded?.Invoke(winner);
    }

    public void OnCardClicked(CardUI cardUI)
    {
        if (!turnManager.IsPlayerTurn) return;

        CardInstance card = cardUI.CardInstance;

        if (CanPlayCard(card))
        {
            PlayCard(card);
        }
    }

    public bool CanPlayCard(CardInstance card)
    {
        return turnManager.IsPlayerTurn &&
               card.baseData.cost <= playerDeck.CurrentMana &&
               !card.isOnBoard;
    }

    public void PlayCard(CardInstance card)
    {
        playerBoard.AddCard(card);
        playerDeck.SpendMana(card.baseData.cost);
        turnManager.OnCardPlayed();
    }

    public bool IsCardOnBoard(CardInstance card)
    {
        return playerBoard.ContainsCard(card) || enemyBoard.ContainsCard(card);
    }
}
