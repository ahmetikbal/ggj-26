using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tek bir diyalog satırı
/// </summary>
[Serializable]
public class DialogueLine
{
    [Tooltip("Konuşan karakter")]
    public CharacterType speaker;
    
    [Tooltip("Konuşma metni")]
    [TextArea(2, 5)]
    public string text;
    
    [Tooltip("Yazım hızı (saniye/karakter)")]
    public float typingSpeed = 0.03f;
}

/// <summary>
/// Oyuncuya sunulan seçenek (a/b şıkları)
/// </summary>
[Serializable]
public class DialogueChoice
{
    [Tooltip("Seçenek metni - örn: 'a) olay anında ne yapıyordunuz?'")]
    public string choiceText;
    
    [Tooltip("Bu seçenek seçildiğinde gösterilecek cevap diyalogları")]
    public DialogueLine[] responseLines;
    
    [Tooltip("Cevaptan sonra devam edilecek node ID (boşsa mevcut node'un nextNodeId'sine gider)")]
    public string jumpToNodeId;
    
    [Tooltip("Bu seçim yapıldığında set edilecek flag")]
    public string flagToSet;
    
    [Tooltip("Bu seçim yapıldığında çalışacak aksiyon")]
    public DialogueAction actionOnSelect;
}

/// <summary>
/// Tek bir diyalog node'u - bir konuşma bloğu
/// </summary>
[Serializable]
public class DialogueNode
{
    [Tooltip("Bu node'un benzersiz ID'si")]
    public string nodeId;
    
    [Tooltip("Editor'da görünecek açıklama")]
    public string editorNote;
    
    [Header("Content")]
    [Tooltip("Bu node'daki diyalog satırları")]
    public DialogueLine[] lines;
    
    [Header("Choices")]
    [Tooltip("Oyuncuya sunulacak seçenekler (boşsa otomatik devam)")]
    public DialogueChoice[] choices;
    
    [Header("Flow Control")]
    [Tooltip("Seçenek yoksa veya seçenekten sonra gidilecek node ID")]
    public string nextNodeId;
    
    [Tooltip("Node tamamlandığında çalışacak aksiyon")]
    public DialogueAction onCompleteAction;
    
    [Header("Conditions")]
    [Tooltip("Bu node'un görünmesi için gerekli flag'ler (AND mantığı)")]
    public string[] requiredFlags;
    
    [Tooltip("Bu flag'lerden biri varsa node gizlenir")]
    public string[] blockedByFlags;
    
    [Tooltip("Minimum chapter numarası")]
    public int minimumChapter = 1;
}

/// <summary>
/// Bir karakterin tüm diyaloglarını tutan container
/// </summary>
[Serializable]
public class CharacterDialogueData
{
    [Tooltip("Karakter tipi")]
    public CharacterType characterType;
    
    [Tooltip("Karakter adı (UI'da gösterilecek)")]
    public string displayName;
    
    [Tooltip("Bu karaktere ait tüm diyalog node'ları")]
    public List<DialogueNode> dialogueNodes = new List<DialogueNode>();
    
    /// <summary>
    /// Belirtilen ID'ye sahip node'u bulur
    /// </summary>
    public DialogueNode GetNode(string nodeId)
    {
        return dialogueNodes.Find(n => n.nodeId == nodeId);
    }
    
    /// <summary>
    /// Bu karakterin chapter için en uygun başlangıç node'unu bulur
    /// Öncelik: tableclean_completed > fruitninja_completed > talked_all_suspects > entry
    /// </summary>
    public DialogueNode GetEntryNode(int chapter)
    {
        var storyState = StoryStateManager.Instance;
        if (storyState == null)
        {
            Debug.LogWarning("[DialogueData] StoryStateManager not found, returning first available node");
            return dialogueNodes.Find(n => n.minimumChapter <= chapter);
        }
        
        // Öncelik sırasına göre entry point'leri kontrol et
        // 1. En yüksek öncelik: ch1_second (tableclean_completed gerektirir) 
        // 2. Orta öncelik: ch1_post_minigame (fruitninja_completed gerektirir)
        // 3. Düşük öncelik: ch1_entry veya ch2_entry (chapter bazlı)
        
        DialogueNode bestMatch = null;
        int bestPriority = -1;
        
        foreach (var node in dialogueNodes)
        {
            // Chapter kontrolü
            if (node.minimumChapter > chapter) continue;
            
            // Koşulları karşılıyor mu?
            if (!storyState.CanShowNode(node)) continue;
            
            // Entry point olabilecek node'ları filtrele
            // Sadece belirli ID pattern'leri entry point olabilir
            if (!IsEntryPointNode(node)) continue;
            
            // Priority hesapla
            int priority = CalculateEntryPriority(node, chapter);
            
            if (priority > bestPriority)
            {
                bestPriority = priority;
                bestMatch = node;
            }
        }
        
        if (bestMatch == null)
        {
            Debug.LogWarning($"[DialogueData] No valid entry node found for {characterType} in chapter {chapter}");
        }
        else
        {
            Debug.Log($"[DialogueData] Selected entry node '{bestMatch.nodeId}' for {characterType} (priority: {bestPriority})");
        }
        
        return bestMatch;
    }
    
    /// <summary>
    /// Node'un entry point olup olmadığını kontrol eder
    /// Entry point: Karakter ile serbest etkileşimde başlayabilecek node
    /// </summary>
    private bool IsEntryPointNode(DialogueNode node)
    {
        if (string.IsNullOrEmpty(node.nodeId)) return false;
        
        // Entry point olabilecek node ID pattern'leri
        return node.nodeId.Contains("entry") ||      // ch1_entry, ch2_entry
               node.nodeId.Contains("second") ||     // ch1_second (talked_all_suspects)
               node.nodeId.Contains("post_minigame") || // ch1_post_minigame (force ile çağrılır ama...)
               node.nodeId.Contains("post_tableclean"); // ch1_post_tableclean
    }
    
    /// <summary>
    /// Entry node için priority hesaplar
    /// Yüksek = daha öncelikli
    /// </summary>
    private int CalculateEntryPriority(DialogueNode node, int chapter)
    {
        int priority = 0;
        
        // Chapter bazlı temel priority
        priority += node.minimumChapter * 1000;
        
        // Required flag sayısı - daha fazla flag = daha spesifik
        if (node.requiredFlags != null)
        {
            priority += node.requiredFlags.Length * 100;
            
            // Özel flag'ler için ekstra priority
            foreach (var flag in node.requiredFlags)
            {
                if (flag == "tableclean_completed")
                    priority += 500; // En yüksek öncelik
                else if (flag == "talked_all_suspects")
                    priority += 200; // İkinci öncelik
                else if (flag == "fruitninja_completed")
                    priority += 100; // Üçüncü öncelik
            }
        }
        
        // Entry node'lar temel priority'ye sahip (en düşük)
        if (node.nodeId.Contains("entry"))
            priority -= 50;
        
        return priority;
    }
}
