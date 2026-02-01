using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Sirenix.OdinInspector;

/// <summary>
/// Her karakterin diyalog panelinde bulunacak script
/// Sahnede zaten mevcut olan UI elementlerine referans tutar
/// </summary>
public class CharacterDialoguePanel : MonoBehaviour
{
    [Header("Character Info")]
    [SerializeField] [EnumToggleButtons] private CharacterType characterType;
    [SerializeField] private string characterDisplayName;
    
    [Header("UI References - Main")]
    [SerializeField] public GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button continueButton;
    
    [Header("UI References - Choices")]
    [SerializeField] private GameObject choicesPanel;
    [SerializeField] private Button choiceButtonA;
    [SerializeField] private Button choiceButtonB;
    [SerializeField] private TMP_Text choiceTextA;
    [SerializeField] private TMP_Text choiceTextB;
    
    [Header("Optional")]
    [SerializeField] private Image characterPortrait;
    [SerializeField] private Sprite[] emotionSprites; // 0: normal, 1: sad, 2: angry, etc.
    
    [Header("Events")]
    public UnityEvent OnDialogueStarted;
    public UnityEvent OnDialogueEnded;
    
    // Properties
    public CharacterType CharacterType => characterType;
    
    private void Start()
    {
        // Setup button listeners
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }
        
        if (choiceButtonA != null)
        {
            choiceButtonA.onClick.AddListener(() => OnChoiceClicked(0));
        }
        
        if (choiceButtonB != null)
        {
            choiceButtonB.onClick.AddListener(() => OnChoiceClicked(1));
        }
        
        // Initially hide
        HideDialogue();
    }
    
    #region Public Methods - Called by DialogueManager
    
    /// <summary>
    /// Bu karakterle diyaloğu başlatır
    /// </summary>
    public void StartDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);
        
        if (choicesPanel != null)
            choicesPanel.SetActive(false);
        
        // Text'leri sıfırla - eski diyalog kalmasın
        if (dialogueText != null)
            dialogueText.text = "";
        
        if (speakerNameText != null)
            speakerNameText.text = "";
        
        OnDialogueStarted?.Invoke();
        
        // DialogueManager'a bu panel ile başlamasını söyle
        DialogueManager.Instance.StartDialogue(characterType, this);
    }
    
    /// <summary>
    /// Diyalog satırını gösterir
    /// </summary>
    public void DisplayLine(CharacterType speaker, string text, float typingSpeed)
    {
        // Konuşan kim?
        string speakerName = GetSpeakerName(speaker);
        
        if (speakerNameText != null)
            speakerNameText.text = speakerName;
        
        if (dialogueText != null)
            dialogueText.text = ""; // Typing effect için boş başla
        
        // Continue butonu göster, seçenekleri gizle
        if (continueButton != null)
            continueButton.gameObject.SetActive(true);
        
        if (choicesPanel != null)
            choicesPanel.SetActive(false);
    }
    
    /// <summary>
    /// Typewriter efekti için çağrılır - kademeli text güncellemesi
    /// </summary>
    public void UpdateTypedText(string partialText)
    {
        if (dialogueText != null)
            dialogueText.text = partialText;
    }
    
    /// <summary>
    /// Typing tamamlandığında çağrılır
    /// </summary>
    public void OnTypingComplete()
    {
        // İsterseniz burada bir ses çalabilir veya animasyon tetikleyebilirsiniz
    }
    
    /// <summary>
    /// Seçenekleri gösterir
    /// </summary>
    public void ShowChoices(string[] choiceTexts)
    {
        // Continue butonunu gizle
        if (continueButton != null)
            continueButton.gameObject.SetActive(false);
        
        // Seçenekleri göster
        if (choicesPanel != null)
            choicesPanel.SetActive(true);
        
        // A seçeneği
        if (choiceButtonA != null && choiceTexts.Length > 0)
        {
            choiceButtonA.gameObject.SetActive(true);
            if (choiceTextA != null)
                choiceTextA.text = choiceTexts[0];
        }
        else if (choiceButtonA != null)
        {
            choiceButtonA.gameObject.SetActive(false);
        }
        
        // B seçeneği
        if (choiceButtonB != null && choiceTexts.Length > 1)
        {
            choiceButtonB.gameObject.SetActive(true);
            if (choiceTextB != null)
                choiceTextB.text = choiceTexts[1];
        }
        else if (choiceButtonB != null)
        {
            choiceButtonB.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Seçenekleri gizler
    /// </summary>
    public void HideChoices()
    {
        if (choicesPanel != null)
            choicesPanel.SetActive(false);
        
        if (continueButton != null)
            continueButton.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Diyaloğu sonlandırır ve paneli gizler
    /// </summary>
    public void HideDialogue()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true); //canvas

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        
        if (choicesPanel != null)
            choicesPanel.SetActive(false);
        
        OnDialogueEnded?.Invoke();
    }
    
    /// <summary>
    /// Karakter portresini değiştirir (opsiyonel)
    /// </summary>
    public void SetEmotion(int emotionIndex)
    {
        if (characterPortrait != null && emotionSprites != null && emotionIndex < emotionSprites.Length)
        {
            characterPortrait.sprite = emotionSprites[emotionIndex];
        }
    }
    
    #endregion
    
    #region Private Methods
    
    private void OnContinueClicked()
    {
        DialogueManager.Instance.NextLine();
    }
    
    private void OnChoiceClicked(int index)
    {
        DialogueManager.Instance.OnChoiceSelected(index);
    }
    
    private string GetSpeakerName(CharacterType speaker)
    {
        // Eğer konuşan bu karakterin kendisiyse display name kullan
        if (speaker == characterType)
            return characterDisplayName;
        
        // Dedektif veya diğer karakterler için
        return speaker switch
        {
            CharacterType.Dedektif => "Dedektif",
            CharacterType.AsciFadime => "Aşçı Fadime",
            CharacterType.Garson => "Garson",
            CharacterType.BesteciRedif => "Besteci Redif",
            CharacterType.SimyaciSimurg => "Simyacı Simurg",
            CharacterType.AycaHanim => "Ayça Hanım",
            CharacterType.TuccarAtlas => "Tüccar Atlas",
            CharacterType.BeatriceHanim => "Beatrice Hanım",
            _ => speaker.ToString()
        };
    }
    
    #endregion
    
    #region Editor Helpers
    
#if UNITY_EDITOR
    /// <summary>
    /// Editor'da test için diyalog başlatır
    /// </summary>
    [ContextMenu("Test Start Dialogue")]
    private void TestStartDialogue()
    {
        if (Application.isPlaying)
        {
            StartDialogue();
        }
        else
        {
            Debug.LogWarning("Play mode'da çalıştırın!");
        }
    }
#endif
    
    #endregion
}
