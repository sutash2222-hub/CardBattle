using System.Collections;
using UnityEngine;

public class LeaderAbilityManager : MonoBehaviour
{
    public static LeaderAbilityManager Instance { get; private set; }

    [Header("Leader Settings")]
    public LeaderAbilityData playerLeader;
    public LeaderAbilityData enemyLeader;

    private bool playerAbilityUsed;
    private bool enemyAbilityUsed;

    public bool PlayerAbilityUsed => playerAbilityUsed;
    public bool EnemyAbilityUsed => enemyAbilityUsed;

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

    public bool CanUsePlayerLeaderAbility()
    {
        return !playerAbilityUsed && playerLeader != null && !playerLeader.isPassive;
    }

    public void ActivatePlayerLeaderAbility(BoardUI playerBoard, BoardUI enemyBoard)
    {
        if (!CanUsePlayerLeaderAbility()) return;

        StartCoroutine(ExecuteLeaderAbility(playerLeader, playerBoard, enemyBoard, true));
    }

    public void ActivateEnemyLeaderAbility(BoardUI playerBoard, BoardUI enemyBoard)
    {
        if (enemyAbilityUsed || enemyLeader == null || enemyLeader.isPassive) return;

        StartCoroutine(ExecuteLeaderAbility(enemyLeader, playerBoard, enemyBoard, false));
    }

    private IEnumerator ExecuteLeaderAbility(LeaderAbilityData leader, BoardUI playerBoard, BoardUI enemyBoard, bool isPlayer)
    {
        // Play VFX
        if (leader.abilityVFX != null)
        {
            GameObject vfx = Instantiate(leader.abilityVFX, transform.position, Quaternion.identity);
            Destroy(vfx, 3f);
        }

        // Play sound
        if (leader.abilitySound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(leader.abilitySound);
        }

        yield return new WaitForSeconds(0.5f);

        // Execute ability
        switch (leader.abilityType)
        {
            case LeaderAbilityType.Reinforce:
                ReinforceAbility(playerBoard);
                break;
            case LeaderAbilityType.Rally:
                RallyAbility(isPlayer);
                break;
            case LeaderAbilityType.WarHorn:
                WarHornAbility(playerBoard);
                break;
            case LeaderAbilityType.ImperialGrip:
                ImperialGripAbility(isPlayer);
                break;
            case LeaderAbilityType.EmperorCall:
                EmperorCallAbility(isPlayer);
                break;
            case LeaderAbilityType.Guerrilla:
                GuerrillaAbility(playerBoard);
                break;
            case LeaderAbilityType.Ambush:
                AmbushAbility(enemyBoard);
                break;
            case LeaderAbilityType.Waylay:
                WaylayAbility(enemyBoard);
                break;
            case LeaderAbilityType.Crones:
                CronesAbility(playerBoard);
                break;
            case LeaderAbilityType.Consume:
                ConsumeAbility(playerBoard);
                break;
            case LeaderAbilityType.Storm:
                StormAbility(enemyBoard);
                break;
            case LeaderAbilityType.Berserk:
                BerserkAbility(playerBoard);
                break;
            case LeaderAbilityType.Resilience:
                ResilienceAbility(playerBoard);
                break;
        }

        // Mark as used
        if (isPlayer)
        {
            playerAbilityUsed = true;
        }
        else
        {
            enemyAbilityUsed = true;
        }

        // Log ability
        if (BattleLogSystem.Instance != null)
        {
            string player = isPlayer ? "Player" : "Enemy";
            BattleLogSystem.Instance.LogAbility($"{player} Leader", leader.abilityName);
        }
    }

    private void ReinforceAbility(BoardUI board)
    {
        foreach (CardInstance card in board.GetCardsOnBoard())
        {
            card.currentPower += 1;
        }
    }

    private void RallyAbility(bool isPlayer)
    {
        // Draw 2 cards
        DeckManager deck = isPlayer ? 
            BattleManager.Instance.playerDeck : 
            BattleManager.Instance.enemyDeck;

        for (int i = 0; i < 2; i++)
        {
            if (isPlayer)
            {
                deck.DrawCard(BattleManager.Instance.playerHand);
            }
            else
            {
                deck.DrawCardToHand();
            }
        }
    }

    private void WarHornAbility(BoardUI board)
    {
        // Double all units in a random row
        int row = Random.Range(0, 3);
        foreach (CardInstance card in board.GetCardsOnBoard())
        {
            card.currentPower *= 2;
        }
    }

    private void ImperialGripAbility(bool isPlayer)
    {
        // Look at top 3 cards (simplified - just draw 1)
        DeckManager deck = isPlayer ?
            BattleManager.Instance.playerDeck :
            BattleManager.Instance.enemyDeck;

        if (isPlayer)
        {
            deck.DrawCard(BattleManager.Instance.playerHand);
        }
        else
        {
            deck.DrawCardToHand();
        }
    }

    private void EmperorCallAbility(bool isPlayer)
    {
        // Play a random card from deck
        DeckManager deck = isPlayer ?
            BattleManager.Instance.playerDeck :
            BattleManager.Instance.enemyDeck;

        // Simplified - just draw a card
        if (isPlayer)
        {
            deck.DrawCard(BattleManager.Instance.playerHand);
        }
        else
        {
            deck.DrawCardToHand();
        }
    }

    private void GuerrillaAbility(BoardUI board)
    {
        // Return weakest unit to hand
        CardInstance weakest = board.GetWeakestUnit();
        if (weakest != null)
        {
            board.RemoveCard(weakest);
            // Add back to hand would go here
        }
    }

    private void AmbushAbility(BoardUI enemyBoard)
    {
        // Set random enemy to 0 power
        CardInstance target = enemyBoard.GetRandomUnit();
        if (target != null)
        {
            target.currentPower = 0;
        }
    }

    private void WaylayAbility(BoardUI enemyBoard)
    {
        // Deal 3 damage to random enemy
        CardInstance target = enemyBoard.GetRandomUnit();
        if (target != null)
        {
            target.currentPower -= 3;
        }
    }

    private void CronesAbility(BoardUI board)
    {
        // Summon all copies of a random unit
        // Simplified - boost all units by 2
        foreach (CardInstance card in board.GetCardsOnBoard())
        {
            card.currentPower += 2;
        }
    }

    private void ConsumeAbility(BoardUI board)
    {
        // Consume weakest ally
        CardInstance weakest = board.GetWeakestUnit();
        if (weakest != null)
        {
            int boost = weakest.currentPower;
            board.RemoveCard(weakest);
            // Boost random unit
            CardInstance target = board.GetRandomUnit();
            if (target != null)
            {
                target.currentPower += boost;
            }
        }
    }

    private void StormAbility(BoardUI enemyBoard)
    {
        // Deal 1 damage to all enemies
        foreach (CardInstance card in enemyBoard.GetCardsOnBoard())
        {
            card.currentPower -= 1;
        }
    }

    private void BerserkAbility(BoardUI board)
    {
        // Boost all damaged units
        foreach (CardInstance card in board.GetCardsOnBoard())
        {
            if (card.currentPower < card.baseData.power)
            {
                card.currentPower += 3;
            }
        }
    }

    private void ResilienceAbility(BoardUI board)
    {
        // Keep strongest unit
        CardInstance strongest = board.GetStrongestUnit();
        if (strongest != null)
        {
            strongest.hasResilience = true;
        }
    }

    public void ResetAbilities()
    {
        playerAbilityUsed = false;
        enemyAbilityUsed = false;
    }
}
