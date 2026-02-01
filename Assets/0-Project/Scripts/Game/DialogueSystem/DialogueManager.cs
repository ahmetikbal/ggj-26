using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Diyalog sisteminin ana kontrolcüsü
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    [Header("Database")]
    [SerializeField] private DialogueDatabase dialogueDatabase;
    
    [Header("Settings")]
    [SerializeField] private float defaultTypingSpeed = 0.03f;
    
    [Header("Events")]
    public UnityEvent OnDialogueStarted;
    public UnityEvent OnDialogueEnded;
    public UnityEvent<DialogueAction> OnActionTriggered;
    
    // Current State
    private CharacterDialogueData currentCharacter;
    private DialogueNode currentNode;
    private int currentLineIndex;
    private bool isDialogueActive;
    private bool isTyping;
    
    // Callback for final decision
    private Action<string> finalDecisionCallback;
    
    // Current panel reference
    private CharacterDialoguePanel currentPanel;
    
    // Properties
    public bool IsDialogueActive => isDialogueActive;
    public bool IsTyping => isTyping;
    
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
    
    #region Public API
    
    /// <summary>
    /// Belirli bir karakterle diyaloğu başlatır
    /// </summary>
    public void StartDialogue(CharacterType characterType, CharacterDialoguePanel panel)
    {
        if (isDialogueActive)
        {
            Debug.LogWarning("[DialogueManager] Dialogue already active!");
            return;
        }
        
        currentPanel = panel;
        currentCharacter = dialogueDatabase.GetCharacterData(characterType);
        
        if (currentCharacter == null)
        {
            Debug.LogError($"[DialogueManager] Character data not found: {characterType}");
            return;
        }
        
        // Find entry node for current chapter
        int chapter = StoryStateManager.Instance.CurrentChapter;
        var entryNode = currentCharacter.GetEntryNode(chapter);
        
        if (entryNode == null)
        {
            Debug.LogError($"[DialogueManager] No entry node found for {characterType} in chapter {chapter}");
            return;
        }
        
        // Mark as talked
        StoryStateManager.Instance.MarkCharacterTalked(characterType);
        
        // Start dialogue
        isDialogueActive = true;
        GameManager.Instance.gameState = GameManager.GameState.Talk;
        OnDialogueStarted?.Invoke();
        
        PlayNode(entryNode);
    }
    
    /// <summary>
    /// Belirli bir node'u zorla başlatır (zorunlu geçişler için)
    /// </summary>
    public void ForcePlayNode(CharacterType characterType, string nodeId, CharacterDialoguePanel panel)
    {
        currentPanel = panel;
        currentCharacter = dialogueDatabase.GetCharacterData(characterType);
        var node = currentCharacter?.GetNode(nodeId);
        
        if (node != null)
        {
            isDialogueActive = true;
            GameManager.Instance.gameState = GameManager.GameState.Talk;
            OnDialogueStarted?.Invoke();
            PlayNode(node);
        }
    }
    
    /// <summary>
    /// Sonraki satıra geç (Continue butonuna basıldığında çağrılır)
    /// </summary>
    public void NextLine()
    {
        if (!isDialogueActive) return;
        
        // Yazım devam ediyorsa tamamla
        if (isTyping)
        {
            CompleteTyping();
            return;
        }
        
        currentLineIndex++;
        
        // Daha fazla satır var mı?
        if (currentLineIndex < currentNode.lines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            // Satırlar bitti, seçenek var mı?
            if (currentNode.choices != null && currentNode.choices.Length > 0)
            {
                ShowChoices();
            }
            else
            {
                // Seçenek yok, node'u tamamla
                CompleteCurrentNode();
            }
        }
    }
    
    /// <summary>
    /// Oyuncu bir seçenek seçtiğinde çağrılır
    /// </summary>
    public void OnChoiceSelected(int choiceIndex)
    {
        if (!isDialogueActive || currentNode.choices == null) return;
        if (choiceIndex < 0 || choiceIndex >= currentNode.choices.Length) return;
        
        var choice = currentNode.choices[choiceIndex];
        
        // Flag set et
        if (!string.IsNullOrEmpty(choice.flagToSet))
        {
            StoryStateManager.Instance.SetFlag(choice.flagToSet);
        }
        
        // Seçenek aksiyonunu çalıştır
        if (choice.actionOnSelect != DialogueAction.None)
        {
            ExecuteAction(choice.actionOnSelect);
        }
        
        // Seçenekleri gizle
        currentPanel?.HideChoices();
        
        // Cevap satırları var mı?
        if (choice.responseLines != null && choice.responseLines.Length > 0)
        {
            // Cevap satırlarını göster, sonra jump veya next
            StartCoroutine(PlayResponseLines(choice));
        }
        else
        {
            // Direkt jump veya next'e git
            ProcessAfterChoice(choice);
        }
    }
    
    /// <summary>
    /// Final karar sistemi için callback register eder
    /// </summary>
    public void RegisterFinalDecisionCallback(Action<string> callback)
    {
        finalDecisionCallback = callback;
    }
    
    /// <summary>
    /// Final kararı gönderir (UI'dan çağrılır)
    /// </summary>
    public void SubmitFinalDecision(string characterName)
    {
        Debug.Log($"[DialogueManager] Final decision submitted: {characterName}");
        finalDecisionCallback?.Invoke(characterName);
    }
    
    /// <summary>
    /// Aktif diyaloğu sonlandırır
    /// </summary>
    public void EndDialogue()
    {
        if (!isDialogueActive) return;
        
        isDialogueActive = false;
        isTyping = false;
        currentNode = null;
        currentLineIndex = 0;
        
        currentPanel?.HideDialogue();
        
        GameManager.Instance.gameState = GameManager.GameState.Free;
        OnDialogueEnded?.Invoke();
        
        Debug.Log("[DialogueManager] Dialogue ended");

        GameManager.Instance.ResetCameraToInitialPosition();
    }
    
    #endregion
    
    #region Private Methods
    
    private void PlayNode(DialogueNode node)
    {
        // Koşul kontrolü
        if (!StoryStateManager.Instance.CanShowNode(node))
        {
            Debug.Log($"[DialogueManager] Node {node.nodeId} conditions not met, skipping");
            
            // Next node'a geç
            if (!string.IsNullOrEmpty(node.nextNodeId))
            {
                var nextNode = currentCharacter.GetNode(node.nextNodeId);
                if (nextNode != null)
                {
                    PlayNode(nextNode);
                    return;
                }
            }
            
            EndDialogue();
            return;
        }
        
        currentNode = node;
        currentLineIndex = 0;
        
        Debug.Log($"[DialogueManager] Playing node: {node.nodeId}");
        
        if (node.lines != null && node.lines.Length > 0)
        {
            DisplayCurrentLine();
        }
        else if (node.choices != null && node.choices.Length > 0)
        {
            ShowChoices();
        }
        else
        {
            CompleteCurrentNode();
        }
    }
    
    private void DisplayCurrentLine()
    {
        if (currentNode.lines == null || currentLineIndex >= currentNode.lines.Length)
            return;
        
        var line = currentNode.lines[currentLineIndex];
        
        // Panel'e gönder
        if (currentPanel != null)
        {
            currentPanel.DisplayLine(line.speaker, line.text, line.typingSpeed);
        }
        
        isTyping = true;
        StartCoroutine(TypeTextCoroutine(line.text, line.typingSpeed));
    }
    
    private IEnumerator TypeTextCoroutine(string text, float speed)
    {
        float actualSpeed = speed > 0 ? speed : defaultTypingSpeed;
        
        for (int i = 0; i <= text.Length; i++)
        {
            if (!isTyping) break;
            
            currentPanel?.UpdateTypedText(text.Substring(0, i));
            yield return new WaitForSeconds(actualSpeed);
        }
        
        isTyping = false;
        currentPanel?.OnTypingComplete();
    }
    
    private void CompleteTyping()
    {
        isTyping = false;
        
        if (currentNode?.lines != null && currentLineIndex < currentNode.lines.Length)
        {
            var line = currentNode.lines[currentLineIndex];
            currentPanel?.UpdateTypedText(line.text);
            currentPanel?.OnTypingComplete();
        }
    }
    
    private void ShowChoices()
    {
        if (currentNode.choices == null || currentNode.choices.Length == 0) return;
        
        string[] choiceTexts = new string[currentNode.choices.Length];
        for (int i = 0; i < currentNode.choices.Length; i++)
        {
            choiceTexts[i] = currentNode.choices[i].choiceText;
        }
        
        currentPanel?.ShowChoices(choiceTexts);
    }
    
    private IEnumerator PlayResponseLines(DialogueChoice choice)
    {
        foreach (var line in choice.responseLines)
        {
            currentPanel?.DisplayLine(line.speaker, line.text, line.typingSpeed);
            
            isTyping = true;
            yield return StartCoroutine(TypeTextCoroutine(line.text, line.typingSpeed));
            
            // Oyuncunun devam etmesini bekle
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space));
            yield return null; // Bir frame bekle
        }
        
        ProcessAfterChoice(choice);
    }
    
    private void ProcessAfterChoice(DialogueChoice choice)
    {
        // Jump to specified node or continue with default next
        string nextId = !string.IsNullOrEmpty(choice.jumpToNodeId) 
            ? choice.jumpToNodeId 
            : currentNode.nextNodeId;
        
        if (!string.IsNullOrEmpty(nextId))
        {
            var nextNode = currentCharacter.GetNode(nextId);
            if (nextNode != null)
            {
                PlayNode(nextNode);
                return;
            }
        }
        
        CompleteCurrentNode();
    }
    
    private void CompleteCurrentNode()
    {
        // Node tamamlanma aksiyonu
        if (currentNode.onCompleteAction != DialogueAction.None)
        {
            ExecuteAction(currentNode.onCompleteAction);
        }
        
        // Sonraki node var mı?
        if (!string.IsNullOrEmpty(currentNode.nextNodeId))
        {
            var nextNode = currentCharacter.GetNode(currentNode.nextNodeId);
            if (nextNode != null)
            {
                PlayNode(nextNode);
                return;
            }
        }
        
        // Node zinciri bitti
        EndDialogue();
    }
    
    private void ExecuteAction(DialogueAction action)
    {
        Debug.Log($"[DialogueManager] Executing action: {action}");
        OnActionTriggered?.Invoke(action);
        
        switch (action)
        {
            case DialogueAction.StartMinigame_FruitNinja:
                EndDialogue();
                // FruitNinjaManager.Instance?.StartGame();
                GameManager.Instance.gameState = GameManager.GameState.MiniGame;
                break;
                
            case DialogueAction.StartMinigame_TableClean:
                EndDialogue();
                // TableCleanManager.Instance?.StartGame();
                GameManager.Instance.gameState = GameManager.GameState.MiniGame;
                break;
                
            case DialogueAction.EndChapter1:
                StoryStateManager.Instance.SetChapter(2);
                StoryStateManager.Instance.SetChapterState(ChapterState.Chapter2_Start);
                break;
                
            case DialogueAction.EndChapter2:
                StoryStateManager.Instance.SetChapterState(ChapterState.Finale);
                break;
                
            case DialogueAction.ShowFinalDecision:
                // Final karar UI'ını göster
                Debug.Log("[DialogueManager] Show final decision UI");
                break;
                
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
                
            // Zorunlu karakter geçişleri
            case DialogueAction.ForceCharacter_Garson:
            case DialogueAction.ForceCharacter_Asci:
            case DialogueAction.ForceCharacter_Tuccar:
            case DialogueAction.ForceCharacter_Ayca:
            case DialogueAction.ForceCharacter_Simyaci:
            case DialogueAction.ForceCharacter_Besteci:
            case DialogueAction.ForceCharacter_Beatrice:
                // Bu aksiyonlar dışarıdan handle edilecek (event üzerinden)
                break;
        }
    }
    
    #endregion
}
