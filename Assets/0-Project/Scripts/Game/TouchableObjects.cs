using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class TouchableObjects : MonoBehaviour
{
    public Camera fakeCamera;
    //CameraConfig
    [HideInInspector] public Vector3 newCameraPosition;
    [HideInInspector] public float ortographicSize = 5f;

    public Transform targetTransform;
    
    [Header("Dialogue Integration")]
    [Tooltip("Bu karakter için diyalog paneli")]
    public CharacterDialoguePanel dialoguePanel;
    
    [Tooltip("Bu objenin temsil ettiği karakter tipi")]
    [EnumToggleButtons]public CharacterType characterType;
    
    [Tooltip("Bu karakter şu an konuşulabilir mi?")]
    public bool isAvailable = true;
    
    /// <summary>
    /// Karakterin müsait olup olmadığını kontrol eder
    /// Flag sistemiyle entegre çalışır
    /// </summary>
    public bool CanInteract()
    {
        if (!isAvailable) return false;
        
        // Story flag kontrolü
        if (StoryStateManager.Instance != null)
        {
            // Unavailable flag'i varsa etkileşime girilmez
            string unavailableFlag = $"{characterType.ToString().ToLower()}_unavailable";
            if (StoryStateManager.Instance.HasFlag(unavailableFlag))
                return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Bu karakterle diyaloğu başlatır
    /// </summary>
    public void StartDialogue()
    {
        if (!CanInteract())
        {
            Debug.Log($"[TouchableObjects] {characterType} is not available for dialogue");
            return;
        }
        
        if (dialoguePanel != null)
        {
            dialoguePanel.StartDialogue();
        }
        else
        {
            Debug.LogWarning($"[TouchableObjects] DialoguePanel not assigned for {characterType}");
        }
    }

    void Start()
    {
        newCameraPosition = fakeCamera.transform.position;
        ortographicSize = fakeCamera.orthographicSize;
    }
}
