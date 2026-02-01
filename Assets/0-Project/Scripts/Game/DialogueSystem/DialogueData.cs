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
    /// Bu karakterin chapter için başlangıç node'unu bulur
    /// </summary>
    public DialogueNode GetEntryNode(int chapter)
    {
        // Önce chapter'a uygun entry node ara
        var entryId = $"ch{chapter}_entry";
        var node = GetNode(entryId);
        if (node != null) return node;
        
        // Yoksa ilk uygun node'u döndür
        return dialogueNodes.Find(n => n.minimumChapter <= chapter);
    }
}
