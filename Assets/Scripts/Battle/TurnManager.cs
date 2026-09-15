using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("Turn Settings")]
    public int maxManaPerTurn = 10;
    public float turnTimeLimit = 60f;

    private int currentMana;
    private int maxMana;
    private bool isPlayerTurn;
    private bool isRoundComplete;
    private float turnTimer;

    public int CurrentMana => currentMana;
    public int MaxMana => maxMana;
    public bool IsPlayerTurn => isPlayerTurn;
    public bool IsRoundComplete => isRoundComplete;

    public event Action<bool> OnTurnStarted;
    public event Action OnTurnEnded;
    public event Action<int> OnManaChanged;

    private void Start()
    {
        maxMana = 0;
        currentMana = 0;
    }

    public void StartPlayerTurn()
    {
        isPlayerTurn = true;
        maxMana = Mathf.Min(maxMana + 1, maxManaPerTurn);
        currentMana = maxMana;
        turnTimer = turnTimeLimit;

        OnTurnStarted?.Invoke(true);
        OnManaChanged?.Invoke(currentMana);
    }

    private void StartEnemyTurn()
    {
        isPlayerTurn = false;
        currentMana = maxMana;
        turnTimer = turnTimeLimit;

        OnTurnStarted?.Invoke(false);
        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        AIPlayer ai = GetComponent<AIPlayer>();
        if (ai != null)
        {
            yield return StartCoroutine(ai.MakeMove());
        }

        yield return new WaitForSeconds(1f);

        EndTurn();
    }

    public void OnCardPlayed()
    {
        currentMana--;
        OnManaChanged?.Invoke(currentMana);

        if (currentMana <= 0)
        {
            EndTurn();
        }
    }

    public void EndTurn()
    {
        OnTurnEnded?.Invoke();

        if (isPlayerTurn)
        {
            isPlayerTurn = false;
            StartCoroutine(EnemyTurnRoutine());
        }
        else
        {
            isRoundComplete = true;
        }
    }

    private void Update()
    {
        if (!isRoundComplete)
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                EndTurn();
            }
        }
    }

    public void ResetForNewRound()
    {
        maxMana = 0;
        currentMana = 0;
        isRoundComplete = false;
    }
}
