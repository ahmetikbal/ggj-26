using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections;

/// <summary>
/// DialogueManager'dan gelen aksiyonları handle eder
/// Sahne geçişleri, minigame entegrasyonu, zorunlu karakter geçişleri
/// </summary>
public class DialogueActionHandler : MonoBehaviour
{
    public static DialogueActionHandler Instance { get; private set; }
    
    [Header("Character Panel References")]
    [SerializeField] private CharacterDialoguePanel garsonPanel;
    [SerializeField] public CharacterDialoguePanel asciPanel;
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
    
    // Hangi sahne aktif olduğunu takip et
    public enum ActiveScene
    {
        Main,
        Asci,
        FruitNinja,
        TableGame
    }
    private ActiveScene currentScene = ActiveScene.Asci;
    
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
    
    #region Scene Management
    
    /// <summary>
    /// Belirtilen sahneye geçiş yapar
    /// </summary>
    public void SwitchToScene(ActiveScene targetScene)
    {
        Debug.Log($"[ActionHandler] Switching scene: {currentScene} -> {targetScene}");
        
        // Tüm sahneleri kapat
        GameManager.Instance.mainSceneGameObject.SetActive(false);
        GameManager.Instance.asciSceneGameObject.SetActive(false);
        GameManager.Instance.fruitNinjaGameObject.SetActive(false);
        GameManager.Instance.tableGameGameObject.SetActive(false);
        
        // Hedef sahneyi aç
        switch (targetScene)
        {
            case ActiveScene.Main:
                GameManager.Instance.mainSceneGameObject.SetActive(true);
                break;
            case ActiveScene.Asci:
                GameManager.Instance.asciSceneGameObject.SetActive(true);
                break;
            case ActiveScene.FruitNinja:
                GameManager.Instance.fruitNinjaGameObject.SetActive(true);
                break;
            case ActiveScene.TableGame:
                GameManager.Instance.tableGameGameObject.SetActive(true);
                break;
        }
        
        currentScene = targetScene;
    }
    
    /// <summary>
    /// Şu anki aktif sahne
    /// </summary>
    public ActiveScene CurrentScene => currentScene;
    
    #endregion
    
    #region Action Handler
    
    /// <summary>
    /// DialogueAction'ları handle eder
    /// </summary>
    private void HandleAction(DialogueAction action)
    {
        Debug.Log($"[ActionHandler] Handling action: {action}");
        
        switch (action)
        {
            // Minigame Actions
            case DialogueAction.StartMinigame_FruitNinja:
                StartFruitNinjaMinigame();
                break;
                
            case DialogueAction.StartMinigame_TableClean:
                StartTableCleanMinigame();
                break;
            
            // Force Character Actions
            case DialogueAction.ForceCharacter_Garson:
                ForceCharacterWithSceneSwitch(CharacterType.Garson, garsonPanel, ActiveScene.Main);
                break;
                
            case DialogueAction.ForceCharacter_Asci:
                ForceCharacterWithSceneSwitch(CharacterType.AsciFadime, asciPanel, ActiveScene.Asci);
                break;
                
            case DialogueAction.ForceCharacter_Besteci:
                ForceCharacterWithSceneSwitch(CharacterType.BesteciRedif, besteciPanel, ActiveScene.Main);
                break;
                
            case DialogueAction.ForceCharacter_Simyaci:
                ForceCharacterWithSceneSwitch(CharacterType.SimyaciSimurg, simyaciPanel, ActiveScene.Main);
                break;
                
            case DialogueAction.ForceCharacter_Ayca:
                ForceCharacterWithSceneSwitch(CharacterType.AycaHanim, aycaPanel, ActiveScene.Main);
                break;
                
            case DialogueAction.ForceCharacter_Tuccar:
                ForceCharacterWithSceneSwitch(CharacterType.TuccarAtlas, tuccarPanel, ActiveScene.Main);
                break;
                
            case DialogueAction.ForceCharacter_Beatrice:
                ForceCharacterWithSceneSwitch(CharacterType.BeatriceHanim, beatricePanel, ActiveScene.Main);
                break;
            
            // Chapter Actions
            case DialogueAction.EndChapter1:
                HandleEndChapter1();
                break;
                
            case DialogueAction.StartChapter2:
                HandleStartChapter2();
                break;
                
            case DialogueAction.EndChapter2:
                HandleEndChapter2();
                break;
            
            // Character Enable/Disable
            case DialogueAction.DisableCharacter_Besteci:
                StoryStateManager.Instance.SetFlag("besteci_unavailable");
                break;
                
            case DialogueAction.EnableCharacter_Besteci:
                StoryStateManager.Instance.RemoveFlag("besteci_unavailable");
                break;
                
            case DialogueAction.DisableCharacter_Simyaci:
                StoryStateManager.Instance.SetFlag("simyaci_unavailable");
                break;
                
            case DialogueAction.EnableCharacter_Simyaci:
                StoryStateManager.Instance.RemoveFlag("simyaci_unavailable");
                break;
            
            // Final Decision
            case DialogueAction.ShowFinalDecision:
                ShowFinalDecisionUI();
                break;
        }
    }
    
    #endregion
    
    #region Chapter Transitions
    
    /// <summary>
    /// Chapter 1 sonu - mainScene'e geç ve Chapter 2'yi başlat
    /// </summary>
    private void HandleEndChapter1()
    {
        Debug.Log("[ActionHandler] Chapter 1 ended, transitioning to Chapter 2...");
        
        // Story state güncelle
        StoryStateManager.Instance.SetChapter(2);
        StoryStateManager.Instance.SetChapterState(ChapterState.Chapter2_Start);
        
        // mainScene'e geç
        StartCoroutine(TransitionToChapter2());
    }
    
    private IEnumerator TransitionToChapter2()
    {
        // Kamerayı resetle
        GameManager.Instance.ResetCameraToInitialPosition();
        
        yield return new WaitForSeconds(1.2f);
        
        // mainScene'e geç
        SwitchToScene(ActiveScene.Main);
        
        // Oyuncu serbest dolaşsın
        GameManager.Instance.gameState = GameManager.GameState.Free;
        
        Debug.Log("[ActionHandler] Chapter 2 started - player can now freely explore");
    }
    
    /// <summary>
    /// Chapter 2'yi manuel olarak başlat
    /// </summary>
    private void HandleStartChapter2()
    {
        StoryStateManager.Instance.SetChapter(2);
        StoryStateManager.Instance.SetChapterState(ChapterState.Chapter2_Start);
        
        SwitchToScene(ActiveScene.Main);
        GameManager.Instance.gameState = GameManager.GameState.Free;
    }
    
    /// <summary>
    /// Chapter 2 sonu - Final karar UI'ını göster
    /// </summary>
    private void HandleEndChapter2()
    {
        Debug.Log("[ActionHandler] Chapter 2 ended, showing final decision...");
        
        StoryStateManager.Instance.SetChapterState(ChapterState.Finale);
        ShowFinalDecisionUI();
    }
    
    #endregion
    
    #region Minigame Handlers
    
    private void StartFruitNinjaMinigame()
    {
        pendingAction = DialogueAction.StartMinigame_FruitNinja;
        
        // Sahne geçişi
        SwitchToScene(ActiveScene.FruitNinja);
        
        if (fruitNinjaCanvas != null)
        {
            fruitNinjaCanvas.SetActive(true);
        }
        
        GameManager.Instance.gameState = GameManager.GameState.MiniGame;
        Debug.Log("[ActionHandler] Fruit Ninja minigame started");
    }
    
    private void StartTableCleanMinigame()
    {
        pendingAction = DialogueAction.StartMinigame_TableClean;
        
        // Sahne geçişi
        SwitchToScene(ActiveScene.TableGame);
        
        if (tableCleanCanvas != null)
        {
            tableCleanCanvas.SetActive(true);
        }
        
        GameManager.Instance.gameState = GameManager.GameState.MiniGame;
        Debug.Log("[ActionHandler] Table Clean minigame started");
    }
    
    /// <summary>
    /// FruitNinja minigame tamamlandığında çağrılır
    /// FruitNinjaUI.ExitGame() içinden çağırılmalı
    /// </summary>
    public void OnFruitNinjaComplete()
    {
        Debug.Log("[ActionHandler] Fruit Ninja completed");
        
        // Flag set et
        StoryStateManager.Instance.SetFlag("fruitninja_completed");
        
        // Minigame UI'ını kapat
        if (fruitNinjaCanvas != null)
            fruitNinjaCanvas.SetActive(false);
        
        // Aşçı sahnesine dön ve post-minigame diyaloğu başlat
        SwitchToScene(ActiveScene.Asci);
        
        // Kamera reset
        GameManager.Instance.gameState = GameManager.GameState.Talk;
        
        // Aşçının post-minigame diyaloğunu başlat
        if (asciPanel != null)
        {
            asciPanel.dialoguePanel.SetActive(true);
            DialogueManager.Instance.ForcePlayNode(
                CharacterType.AsciFadime, 
                "ch1_post_minigame", 
                asciPanel
            );
        }
        
        pendingAction = DialogueAction.None;
    }
    
    /// <summary>
    /// TableClean minigame tamamlandığında çağrılır
    /// Garsonun masa temizleme oyununun sonunda bu fonksiyonu çağır!
    /// </summary>
    public void OnTableCleanComplete()
    {
        Debug.Log("[ActionHandler] Table Clean completed");
        
        // Flag set et
        StoryStateManager.Instance.SetFlag("tableclean_completed");
        
        // Minigame UI'ını kapat
        if (tableCleanCanvas != null)
            tableCleanCanvas.SetActive(false);
        
        // Main sahneye dön
        SwitchToScene(ActiveScene.Main);
        
        // Garson'un kamera pozisyonunu al ve ayarla
        var garsonTouchable = TouchableController.Instance?.GetTouchableByCharacter(CharacterType.Garson);
        if (garsonTouchable != null)
        {
            // Kamerayı garson'a zoom yap
            Camera.main.transform.DOMove(garsonTouchable.newCameraPosition, 0.5f).SetEase(Ease.InOutQuad);
            DOVirtual.Float(Camera.main.orthographicSize, garsonTouchable.ortographicSize, 0.5f, 
                (value) => Camera.main.orthographicSize = value).SetEase(Ease.InOutQuad);
        }
        
        GameManager.Instance.gameState = GameManager.GameState.Talk;
        
        // Garsonun post-tableclean diyaloğunu başlat
        if (garsonPanel != null)
        {
            garsonPanel.dialoguePanel.SetActive(true);
            DialogueManager.Instance.ForcePlayNode(
                CharacterType.Garson, 
                "ch1_post_tableclean", 
                garsonPanel
            );
        }
        
        pendingAction = DialogueAction.None;
    }
    
    /// <summary>
    /// Eski genel minigame complete - geriye uyumluluk için bırakıldı
    /// Yeni projede OnFruitNinjaComplete veya OnTableCleanComplete kullanın
    /// </summary>
    public void OnMinigameComplete()
    {
        switch (pendingAction)
        {
            case DialogueAction.StartMinigame_FruitNinja:
                OnFruitNinjaComplete();
                break;
            case DialogueAction.StartMinigame_TableClean:
                OnTableCleanComplete();
                break;
        }
    }
    
    #endregion
    
    #region Character Force Dialogue
    
    /// <summary>
    /// Karaktere sahne geçişiyle birlikte zorla git
    /// Kamera hareketi ve inspector yürümesi dahil
    /// </summary>
    private void ForceCharacterWithSceneSwitch(CharacterType character, CharacterDialoguePanel panel, ActiveScene targetScene)
    {
        pendingCharacter = character;
        
        // Mevcut sahne ile hedef sahne aynı mı?
        if (currentScene == targetScene)
        {
            // Aynı sahnedeyiz, sadece kamera geçişi yap
            StartCoroutine(ForceCharacterWithCamera(character, panel));
        }
        else
        {
            // Sahne geçişi gerekiyor
            StartCoroutine(ForceCharacterWithSceneTransition(character, panel, targetScene));
        }
    }
    
    /// <summary>
    /// Sahne geçişi ile karakter diyaloğu
    /// </summary>
    private IEnumerator ForceCharacterWithSceneTransition(CharacterType character, CharacterDialoguePanel panel, ActiveScene targetScene)
    {
        Debug.Log($"[ActionHandler] Transitioning to {targetScene} for {character}");
        
        // Önce kamerayı sıfırla
        GameManager.Instance.ResetCameraToInitialPosition();
        
        // Animasyon bekle
        yield return new WaitForSeconds(1.1f);
        
        // Sahne geçişi
        SwitchToScene(targetScene);
        
        // Biraz bekle
        yield return new WaitForSeconds(0.2f);
        
        // TouchableController üzerinden karakter etkileşimini simüle et
        var touchable = TouchableController.Instance?.GetTouchableByCharacter(character);
        
        if (touchable != null)
        {
            // Kamera ve inspector hareketini başlat
            TouchableController.Instance.ForceInteract(character);
        }
        else
        {
            // TouchableObjects bulunamadıysa doğrudan diyalog başlat
            Debug.LogWarning($"[ActionHandler] TouchableObjects not found for {character}, starting dialogue directly");
            if (panel != null)
            {
                panel.dialoguePanel.SetActive(true);
                panel.StartDialogue();
            }
        }
    }
    
    /// <summary>
    /// Aynı sahnede kamera ve inspector hareketi ile karakter diyaloğu
    /// </summary>
    private IEnumerator ForceCharacterWithCamera(CharacterType character, CharacterDialoguePanel panel)
    {
        Debug.Log($"[ActionHandler] Moving to {character} in current scene");
        
        // Önce kamerayı sıfırla
        GameManager.Instance.ResetCameraToInitialPosition();
        
        // Animasyon bekle
        yield return new WaitForSeconds(1.1f);
        
        // TouchableController üzerinden karakter etkileşimini simüle et
        TouchableController.Instance?.ForceInteract(character);
    }
    
    /// <summary>
    /// Panel referansını karakter tipine göre al
    /// </summary>
    public CharacterDialoguePanel GetPanelForCharacter(CharacterType type)
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

