using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FruitNinjaUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverTitle;
    [SerializeField] private TextMeshProUGUI gameOverMessage;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    
    [Header("Manager Reference")]
    [SerializeField] private FruitNinjaManager gameManager;
    
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<FruitNinjaManager>();
        }
        
        if (gameManager != null)
        {
            gameManager.OnScoreChanged.AddListener(UpdateScore);
            gameManager.OnTimeChanged.AddListener(UpdateTimer);
            gameManager.OnGameWon.AddListener(ShowWinScreen);
            gameManager.OnGameLost.AddListener(ShowLoseScreen);
        }
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        UpdateScore(0);
        UpdateTimer(gameManager != null ? gameManager.GetTimeRemaining() : 0);
    }
    
    void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnScoreChanged.RemoveListener(UpdateScore);
            gameManager.OnTimeChanged.RemoveListener(UpdateTimer);
            gameManager.OnGameWon.RemoveListener(ShowWinScreen);
            gameManager.OnGameLost.RemoveListener(ShowLoseScreen);
        }
    }
    
    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    
    public void UpdateTimer(float timeRemaining)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
    
    public void ShowWinScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (gameOverTitle != null)
            {
                gameOverTitle.text = "BAŞARILI!";
                gameOverTitle.color = Color.green;
            }
            
            if (gameOverMessage != null && gameManager != null)
            {
                gameOverMessage.text = $"Harika! {gameManager.GetCurrentScore()} meyve kestin!\nAşçı seninle konuşmaya hazır.";
            }
        }
    }
    
    public void ShowLoseScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (gameOverTitle != null)
            {
                gameOverTitle.text = "BAŞARISIZ";
                gameOverTitle.color = Color.red;
            }
            
            if (gameOverMessage != null && gameManager != null)
            {
                gameOverMessage.text = $"Sadece {gameManager.GetCurrentScore()} meyve kesebildin.\nTekrar dener misin?";
            }
        }
    }
    
    public void RestartGame()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
    }
    
    public void ExitGame()
    {
        // FruitNinja tamamlandı, DialogueActionHandler sahne geçişlerini halleder
        Debug.Log("[FruitNinjaUI] Exit mini-game - calling OnFruitNinjaComplete");
        
        // Tek çağrı yeterli - tüm sahne geçişleri ve diyalog başlatma burada hallediliyor
        DialogueActionHandler.Instance.OnFruitNinjaComplete();
    }
}
