using UnityEngine;

public enum GameState
{
    Playing,
    GameOver,
    LevelComplete
}

public class GameManager : MonoBehaviour
{
    [Header("Start-Werte")]
    [SerializeField] private int startLives = 3;
    [SerializeField] private BodensensorTrigger bodensensor;
    [SerializeField] private BallController ball;

    public int Lives { get; private set; }
    public int Score { get; private set; }
    public GameState State { get; private set; }
    private int blocksRemaining;
    public int BlocksRemaining => blocksRemaining;
    public event System.Action<int> BlocksRemainingChanged;

    public event System.Action<int> LivesChanged;
    public event System.Action<int> ScoreChanged;
    public event System.Action<GameState> StateChanged;

    private void Awake()
    {
        Lives = startLives;
        Score = 0;
        State = GameState.Playing;

        Debug.Log("Leben: " + Lives + ", Punkte: " + Score);
    }

    private void OnEnable()
    {
        if (bodensensor != null) bodensensor.BallLost += OnBallLost;
    }

    private void OnDisable()
    {
        if (bodensensor != null) bodensensor.BallLost -= OnBallLost;
    }

    private void OnBallLost()
    {
        if (State != GameState.Playing) return;

        Lives--;
        LivesChanged?.Invoke(Lives);
        Debug.Log("Leben: " + Lives);

        if (Lives > 0 && ball != null)
        {
            ball.ResetBall();
        }

        if (Lives <= 0)
        {
            State = GameState.GameOver;
            StateChanged?.Invoke(State);
            Debug.Log("Game Over");
        }
    }

    public void AddScore(int points)
    {
        if (State != GameState.Playing) return;

        Score += points;
        ScoreChanged?.Invoke(Score);
        Debug.Log("Punkte: " + Score);
    }

    public void RegisterBlock()
    {
        blocksRemaining++;
        BlocksRemainingChanged?.Invoke(blocksRemaining);
    }

    public void BlockDestroyed()
    {
        if (State != GameState.Playing) return;

        blocksRemaining--;
        BlocksRemainingChanged?.Invoke(blocksRemaining);
        Debug.Log("Noch " + blocksRemaining + " Blöcke");

        if (blocksRemaining <= 0)
        {
            State = GameState.LevelComplete;
            StateChanged?.Invoke(State);
            Debug.Log("Level geschafft");
        }
    }

    public void ResumePlaying()
    {
        State = GameState.Playing;
        StateChanged?.Invoke(State);

        if (ball != null)
        {
            ball.ResetBall();
        }
    }

    [ContextMenu("Test: 100 Punkte")]
    private void TestScore()
    {
        AddScore(100);
    }

    [ContextMenu("Test: Block registrieren")]
    private void TestRegister()
    {
        RegisterBlock();
    }

    [ContextMenu("Test: Block zerstören")]
    private void TestDestroy()
    {
        BlockDestroyed();
    }
}
