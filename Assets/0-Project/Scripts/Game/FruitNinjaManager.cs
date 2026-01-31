using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FruitNinjaManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 60f;
    [SerializeField] private int targetScore = 30;
    [SerializeField] private int bombPenalty = 10;
    
    [Header("References")]
    [SerializeField] private FruitSpawner fruitSpawner;
    [SerializeField] private SliceController sliceController;
    
    [Header("Events")]
    public UnityEvent<int> OnScoreChanged;
    public UnityEvent<float> OnTimeChanged;
    public UnityEvent OnGameWon;
    public UnityEvent OnGameLost;
    
    private int currentScore = 0;
    private float timeRemaining;
    private bool isGameActive = false;
    
    public enum GameState
    {
        Idle,
        Playing,
        Success,
        Failed
    }
    
    private GameState currentState = GameState.Idle;
    
    void Start()
    {
        timeRemaining = gameDuration;
    }

    void OnEnable()
    {
        StartGame();
    }
    
    public void StartGame()
    {
        // Cancel existing game loop if running
        StopGame();
        
        currentState = GameState.Playing;
        isGameActive = true;
        currentScore = 0;
        timeRemaining = gameDuration;
        
        OnScoreChanged?.Invoke(currentScore);
        OnTimeChanged?.Invoke(timeRemaining);
        
        if (fruitSpawner != null)
        {
            fruitSpawner.Initialize(this);
            fruitSpawner.StartSpawning();
        }
        
        if (sliceController != null)
        {
            sliceController.enabled = true;
        }
        
        StartCoroutine(GameTimer());
    }
    
    public void StopGame()
    {
        isGameActive = false;
        currentState = GameState.Idle;
        
        if (fruitSpawner != null)
        {
            fruitSpawner.StopSpawning();
        }
        
        if (sliceController != null)
        {
            sliceController.enabled = false;
        }
    }
    
    private IEnumerator GameTimer()
    {
        while (isGameActive && timeRemaining > 0)
        {
            yield return null;
            timeRemaining -= Time.deltaTime;
            OnTimeChanged?.Invoke(Mathf.Max(0, timeRemaining));
        }
        
        if (isGameActive)
        {
            EndGame();
        }
    }
    
    private void EndGame()
    {
        isGameActive = false;
        
        if (fruitSpawner != null)
        {
            fruitSpawner.StopSpawning();
        }
        
        if (sliceController != null)
        {
            sliceController.enabled = false;
        }
        
        if (currentScore >= targetScore)
        {
            currentState = GameState.Success;
            OnGameWon?.Invoke();
        }
        else
        {
            currentState = GameState.Failed;
            OnGameLost?.Invoke();
        }
    }
    
    public void OnFruitSliced()
    {
        if (!isGameActive) return;
        
        currentScore++;
        OnScoreChanged?.Invoke(currentScore);
        
        // Check if target reached
        if (currentScore >= targetScore)
        {
            EndGame();
        }
    }
    
    public void OnBombSliced()
    {
        if (!isGameActive) return;
        
        // Apply penalty
        currentScore = Mathf.Max(0, currentScore - bombPenalty);
        OnScoreChanged?.Invoke(currentScore);
        
        // Optional: End game immediately on bomb
        // EndGame();
    }
    
    public void OnObjectMissed()
    {
        // Optional: Penalty for missing fruits
    }
    
    public int GetCurrentScore() => currentScore;
    public float GetTimeRemaining() => timeRemaining;
    public bool IsGameActive() => isGameActive;
    public GameState GetCurrentState() => currentState;
}
