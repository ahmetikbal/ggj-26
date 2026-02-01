using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// DialogueManager'dan gelen aksiyonları handle eder
/// Minigame entegrasyonu, zorunlu karakter geçişleri vs.
/// </summary>
public class DialogueActionHandler : MonoBehaviour
{
    public static DialogueActionHandler Instance { get; private set; }
    
    [Header("Character Panel References")]
    [SerializeField] private CharacterDialoguePanel garsonPanel;
    [SerializeField] private CharacterDialoguePanel asciPanel;
    [SerializeField] private CharacterDialoguePanel besteciPanel;
    [SerializeField] private CharacterDialoguePanel simyaciPanel;
    [SerializeField] private CharacterDialoguePanel aycaPanel;
    [SerializeField] private CharacterDialoguePanel tuccarPanel;
    [SerializeField] private CharacterDialoguePanel beatricePanel;
    
    [Header("Minigame References")]
    [SerializeField] private GameObject fruitNinjaCanvas;
    [SerializeField] private GameObject tableCleanCanvas;
    
    [Header("Final Decision")]
    [SerializeField] private GameObject finalDecisionPanel;
    
    [Header("Events")]
    public UnityEvent<string> OnGameEnded; // Suçlu ismiyle çağrılır
    
    private DialogueAction pendingAction;
    private CharacterType pendingCharacter;
    
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
        // DialogueManager'ın action eventini dinle
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnActionTriggered.AddListener(HandleAction);
        }
    }
    
    private void OnDestroy()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnActionTriggered.RemoveListener(HandleAction);
        }
    }
    
    /// <summary>
    /// DialogueAction'ları handle eder
    /// </summary>
    private void HandleAction(DialogueAction action)
    {
        Debug.Log($"[ActionHandler] Handling action: {action}");
        
        switch (action)
        {
            case DialogueAction.StartMinigame_FruitNinja:
                StartFruitNinjaMinigame();
                break;
                
            case DialogueAction.StartMinigame_TableClean:
                StartTableCleanMinigame();
                break;
                
            case DialogueAction.ForceCharacter_Garson:
                ForceCharacterDialogue(garsonPanel);
                break;
                
            case DialogueAction.ForceCharacter_Asci:
                ForceCharacterDialogue(asciPanel);
                break;
                
            case DialogueAction.ForceCharacter_Besteci:
                ForceCharacterDialogue(besteciPanel);
                break;
                
            case DialogueAction.ForceCharacter_Simyaci:
                ForceCharacterDialogue(simyaciPanel);
                break;
                
            case DialogueAction.ForceCharacter_Ayca:
                ForceCharacterDialogue(aycaPanel);
                break;
                
            case DialogueAction.ForceCharacter_Tuccar:
                ForceCharacterDialogue(tuccarPanel);
                break;
                
            case DialogueAction.ForceCharacter_Beatrice:
                ForceCharacterDialogue(beatricePanel);
                break;
                
            case DialogueAction.ShowFinalDecision:
                ShowFinalDecisionUI();
                break;
        }
    }
    
    #region Minigame Handlers
    
    private void StartFruitNinjaMinigame()
    {
        pendingAction = DialogueAction.StartMinigame_FruitNinja;
        
        if (fruitNinjaCanvas != null)
        {
            fruitNinjaCanvas.SetActive(true);
        }

        GameManager.Instance.fruitNinjaGameObject.SetActive(true);
        GameManager.Instance.asciSceneGameObject.SetActive(false);
        GameManager.Instance.mainSceneGameObject.SetActive(false);
        
        // FruitNinjaManager'a callback register et
        // FruitNinjaManager.Instance.OnGameComplete += OnMinigameComplete;
        // FruitNinjaManager.Instance.StartGame();
        
        Debug.Log("[ActionHandler] Fruit Ninja minigame started");
    }
    
    private void StartTableCleanMinigame()
    {
        pendingAction = DialogueAction.StartMinigame_TableClean;
        
        if (tableCleanCanvas != null)
        {
            tableCleanCanvas.SetActive(true);
        }
        
        Debug.Log("[ActionHandler] Table Clean minigame started");
    }
    
    /// <summary>
    /// Minigame tamamlandığında çağrılır
    /// </summary>
    public void OnMinigameComplete()
    {
        Debug.Log($"[ActionHandler] Minigame completed, pending action was: {pendingAction}");
        
        // Minigame UI'ını kapat
        if (fruitNinjaCanvas != null)
            fruitNinjaCanvas.SetActive(false);
        if (tableCleanCanvas != null)
            tableCleanCanvas.SetActive(false);
        
        GameManager.Instance.gameState = GameManager.GameState.Free;
        
        // Minigame sonrası devam edilecek aksiyonlar
        switch (pendingAction)
        {
            case DialogueAction.StartMinigame_FruitNinja:
                // Aşçı ile devam et (minigame sonrası)
                StoryStateManager.Instance.SetFlag("fruitninja_completed");
                if (asciPanel != null)
                {
                    // Aşçının minigame sonrası node'unu başlat
                    DialogueManager.Instance.ForcePlayNode(
                        CharacterType.AsciFadime, 
                        "ch1_post_minigame", 
                        asciPanel
                    );
                }
                break;
                
            case DialogueAction.StartMinigame_TableClean:
                // Garson ile devam et
                StoryStateManager.Instance.SetFlag("tableclean_completed");
                if (garsonPanel != null)
                {
                    DialogueManager.Instance.ForcePlayNode(
                        CharacterType.Garson, 
                        "ch1_post_tableclean", 
                        garsonPanel
                    );
                }
                break;
        }
        
        pendingAction = DialogueAction.None;
    }
    
    #endregion
    
    #region Character Force Dialogue
    
    private void ForceCharacterDialogue(CharacterDialoguePanel panel)
    {
        if (panel != null)
        {
            // Biraz gecikme ile başlat (diyalog geçiş animasyonu için)
            Invoke(nameof(StartPendingDialogue), 0.5f);
            pendingCharacter = panel.CharacterType;
        }
    }
    
    private void StartPendingDialogue()
    {
        var panel = GetPanelForCharacter(pendingCharacter);
        if (panel != null)
        {
            panel.StartDialogue();
        }
    }
    
    private CharacterDialoguePanel GetPanelForCharacter(CharacterType type)
    {
        return type switch
        {
            CharacterType.Garson => garsonPanel,
            CharacterType.AsciFadime => asciPanel,
            CharacterType.BesteciRedif => besteciPanel,
            CharacterType.SimyaciSimurg => simyaciPanel,
            CharacterType.AycaHanim => aycaPanel,
            CharacterType.TuccarAtlas => tuccarPanel,
            CharacterType.BeatriceHanim => beatricePanel,
            _ => null
        };
    }
    
    #endregion
    
    #region Final Decision
    
    private void ShowFinalDecisionUI()
    {
        if (finalDecisionPanel != null)
        {
            finalDecisionPanel.SetActive(true);
        }
        
        // Callback register et
        DialogueManager.Instance.RegisterFinalDecisionCallback(OnFinalDecisionMade);
    }
    
    /// <summary>
    /// Final karar butonlarından çağrılır
    /// </summary>
    public void SubmitFinalDecision(string characterName)
    {
        DialogueManager.Instance.SubmitFinalDecision(characterName);
    }
    
    private void OnFinalDecisionMade(string characterName)
    {
        Debug.Log($"[ActionHandler] Final decision made: {characterName}");
        
        if (finalDecisionPanel != null)
        {
            finalDecisionPanel.SetActive(false);
        }
        
        // Oyun sonu eventi
        OnGameEnded?.Invoke(characterName);
        
        // Doğru cevap kontrolü
        bool isCorrect = characterName.ToLower().Contains("beatrice") || 
                         characterName.ToLower().Contains("simurg") ||
                         characterName.ToLower().Contains("simyacı");
        
        if (isCorrect)
        {
            Debug.Log("[ActionHandler] Correct answer!");
            StoryStateManager.Instance.SetFlag("game_won");
        }
        else
        {
            Debug.Log("[ActionHandler] Wrong answer!");
            StoryStateManager.Instance.SetFlag("game_lost");
        }
    }
    
    #endregion
}
