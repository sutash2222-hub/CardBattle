using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public GameState currentState;

    private int playerWins;
    private int enemyWins;

    public GameState CurrentState => currentState;

    public enum GameState
    {
        MainMenu,
        Battle,
        DeckBuilder,
        Victory,
        Defeat
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f;
                break;
            case GameState.Battle:
                Time.timeScale = 1f;
                break;
            case GameState.DeckBuilder:
                Time.timeScale = 1f;
                break;
            case GameState.Victory:
                Time.timeScale = 0f;
                playerWins++;
                break;
            case GameState.Defeat:
                Time.timeScale = 0f;
                enemyWins++;
                break;
        }
    }

    public void StartBattle()
    {
        ChangeState(GameState.Battle);
        SceneManager.LoadScene("BattleScene");
    }

    public void EndBattle(bool playerWon)
    {
        if (playerWon)
        {
            ChangeState(GameState.Victory);
        }
        else
        {
            ChangeState(GameState.Defeat);
        }
    }

    public void ReturnToMainMenu()
    {
        ChangeState(GameState.MainMenu);
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenDeckBuilder()
    {
        ChangeState(GameState.DeckBuilder);
        SceneManager.LoadScene("DeckBuilder");
    }
}
