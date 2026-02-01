using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Oyun durumunu ve hikaye ilerlemesini yöneten manager
/// </summary>
public class StoryStateManager : MonoBehaviour
{
    public static StoryStateManager Instance { get; private set; }
    
    [Header("Current State")]
    [SerializeField] private int currentChapter = 1;
    [SerializeField] [EnumToggleButtons] private ChapterState chapterState = ChapterState.Intro;
    
    [Header("Debug - Active Flags")]
    [SerializeField] private List<string> debugActiveFlags = new List<string>();
    
    // Aktif story flag'leri
    private HashSet<string> activeFlags = new HashSet<string>();
    
    // Konuşulan karakterler
    private HashSet<CharacterType> talkedCharacters = new HashSet<CharacterType>();
    
    // Events
    public event Action<string> OnFlagSet;
    public event Action<int> OnChapterChanged;
    public event Action<ChapterState> OnChapterStateChanged;
    
    // Properties
    public int CurrentChapter => currentChapter;
    [EnumToggleButtons] public ChapterState CurrentChapterState => chapterState;
    
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
    
    #region Flag Operations
    
    /// <summary>
    /// Bir flag'i aktif eder
    /// </summary>
    public void SetFlag(string flag)
    {
        if (string.IsNullOrEmpty(flag)) return;
        
        if (activeFlags.Add(flag))
        {
            debugActiveFlags.Add(flag);
            Debug.Log($"[StoryState] Flag set: {flag}");
            OnFlagSet?.Invoke(flag);
        }
    }
    
    /// <summary>
    /// Flag aktif mi kontrol eder
    /// </summary>
    public bool HasFlag(string flag)
    {
        return activeFlags.Contains(flag);
    }
    
    /// <summary>
    /// Bir flag'i kaldırır
    /// </summary>
    public void RemoveFlag(string flag)
    {
        if (activeFlags.Remove(flag))
        {
            debugActiveFlags.Remove(flag);
            Debug.Log($"[StoryState] Flag removed: {flag}");
        }
    }
    
    /// <summary>
    /// Tüm flag'leri temizler
    /// </summary>
    public void ClearAllFlags()
    {
        activeFlags.Clear();
        debugActiveFlags.Clear();
    }
    
    #endregion
    
    #region Character Tracking
    
    /// <summary>
    /// Karakterle konuşulduğunu işaretle
    /// </summary>
    public void MarkCharacterTalked(CharacterType character)
    {
        if (talkedCharacters.Add(character))
        {
            // Otomatik flag oluştur
            SetFlag($"talked_{character}");
            Debug.Log($"[StoryState] Talked to: {character}");
        }
    }
    
    /// <summary>
    /// Bu karakterle konuşulmuş mu?
    /// </summary>
    public bool HasTalkedTo(CharacterType character)
    {
        return talkedCharacters.Contains(character);
    }
    
    #endregion
    
    #region Chapter Management
    
    /// <summary>
    /// Chapter numarasını değiştirir
    /// </summary>
    public void SetChapter(int chapter)
    {
        if (currentChapter != chapter)
        {
            currentChapter = chapter;
            SetFlag($"chapter_{chapter}_started");
            Debug.Log($"[StoryState] Chapter changed to: {chapter}");
            OnChapterChanged?.Invoke(chapter);
        }
    }
    
    /// <summary>
    /// Chapter state'ini değiştirir
    /// </summary>
    public void SetChapterState(ChapterState state)
    {
        if (chapterState != state)
        {
            chapterState = state;
            Debug.Log($"[StoryState] Chapter state changed to: {state}");
            OnChapterStateChanged?.Invoke(state);
        }
    }
    
    /// <summary>
    /// Bir sonraki chapter'a geç
    /// </summary>
    public void AdvanceChapter()
    {
        SetChapter(currentChapter + 1);
    }
    
    #endregion
    
    #region Condition Checking
    
    /// <summary>
    /// Bir DialogueNode'un gösterilebilir olup olmadığını kontrol eder
    /// </summary>
    public bool CanShowNode(DialogueNode node)
    {
        if (node == null) return false;
        
        // Chapter kontrolü
        if (currentChapter < node.minimumChapter)
            return false;
        
        // Gerekli flag'ler kontrolü (hepsi olmalı)
        if (node.requiredFlags != null)
        {
            foreach (var required in node.requiredFlags)
            {
                if (!string.IsNullOrEmpty(required) && !HasFlag(required))
                    return false;
            }
        }
        
        // Engelleyen flag'ler kontrolü (hiçbiri olmamalı)
        if (node.blockedByFlags != null)
        {
            foreach (var blocked in node.blockedByFlags)
            {
                if (!string.IsNullOrEmpty(blocked) && HasFlag(blocked))
                    return false;
            }
        }
        
        return true;
    }
    
    #endregion
    
    #region Save/Load (Optional - for future)
    
    /// <summary>
    /// Durumu JSON olarak export eder
    /// </summary>
    public string ExportState()
    {
        var state = new SavedState
        {
            chapter = currentChapter,
            chapterState = chapterState,
            flags = new List<string>(activeFlags),
            talkedCharacters = new List<CharacterType>(talkedCharacters)
        };
        return JsonUtility.ToJson(state);
    }
    
    /// <summary>
    /// JSON'dan durumu yükler
    /// </summary>
    public void ImportState(string json)
    {
        var state = JsonUtility.FromJson<SavedState>(json);
        currentChapter = state.chapter;
        chapterState = state.chapterState;
        activeFlags = new HashSet<string>(state.flags);
        talkedCharacters = new HashSet<CharacterType>(state.talkedCharacters);
        debugActiveFlags = state.flags;
    }
    
    [Serializable]
    private class SavedState
    {
        public int chapter;
        public ChapterState chapterState;
        public List<string> flags;
        public List<CharacterType> talkedCharacters;
    }
    
    #endregion
}
