using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tüm karakterlerin diyalog verilerini tutan veritabanı
/// Inspector'da düzenlenebilir
/// </summary>
[CreateAssetMenu(fileName = "DialogueDatabase", menuName = "Dialogue/Database")]
public class DialogueDatabase : ScriptableObject
{
    [Header("All Character Dialogues")]
    public List<CharacterDialogueData> characters = new List<CharacterDialogueData>();
    
    /// <summary>
    /// Belirtilen karakterin diyalog verisini döndürür
    /// </summary>
    public CharacterDialogueData GetCharacterData(CharacterType characterType)
    {
        return characters.Find(c => c.characterType == characterType);
    }
    
    /// <summary>
    /// Belirtilen karakterin belirtilen node'unu döndürür
    /// </summary>
    public DialogueNode GetNode(CharacterType characterType, string nodeId)
    {
        var charData = GetCharacterData(characterType);
        return charData?.GetNode(nodeId);
    }
    
    /// <summary>
    /// Karakter için chapter'a uygun entry node döndürür
    /// </summary>
    public DialogueNode GetEntryNode(CharacterType characterType, int chapter)
    {
        var charData = GetCharacterData(characterType);
        return charData?.GetEntryNode(chapter);
    }
    
    #region Editor Helper Methods
    
#if UNITY_EDITOR
    /// <summary>
    /// Editor'da yeni karakter ekler
    /// </summary>
    public void AddCharacter(CharacterType type, string displayName)
    {
        if (GetCharacterData(type) != null)
        {
            Debug.LogWarning($"Character {type} already exists!");
            return;
        }
        
        characters.Add(new CharacterDialogueData
        {
            characterType = type,
            displayName = displayName,
            dialogueNodes = new List<DialogueNode>()
        });
        
        UnityEditor.EditorUtility.SetDirty(this);
    }
    
    /// <summary>
    /// Karaktere yeni node ekler
    /// </summary>
    public void AddNodeToCharacter(CharacterType type, DialogueNode node)
    {
        var charData = GetCharacterData(type);
        if (charData == null)
        {
            Debug.LogWarning($"Character {type} not found!");
            return;
        }
        
        charData.dialogueNodes.Add(node);
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
    
    #endregion
}
