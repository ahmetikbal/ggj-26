using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [HideInInspector] public Vector3 initialCameraPosition;
    [HideInInspector] public float initialOrtographicSize;

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

        initialCameraPosition = Camera.main.transform.position;
        initialOrtographicSize = Camera.main.orthographicSize;
    }

    public enum GameState
    {
        Free,
        MoveToTarget,
        Talk,
        MiniGame,
        Cutscene
    }

    [EnumToggleButtons] public GameState gameState;

    public Animator inspectorAnimator;
    public Transform lastTargetTransformForInspector;
    [HideInInspector] public Vector3 nextCameraPosition;
    [HideInInspector] public float nextOrtographicSize;
    
    private bool dialogueTriggered = false;

    void Start()
    {
        gameState = GameState.Free;
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.Free:
                inspectorAnimator.speed = 1;
                dialogueTriggered = false; 
                break;
                
            case GameState.MoveToTarget:
                inspectorAnimator.speed = 1;
                if(InspectorNavMesh.Instance.navMeshAgent.remainingDistance <= 0.01f)
                {
                    Camera.main.transform.DOMove(nextCameraPosition, 1f).SetEase(Ease.Linear);
                    DOVirtual.Float(Camera.main.orthographicSize, nextOrtographicSize, 1f, (value) => Camera.main.orthographicSize = value).SetEase(Ease.InOutQuad);
                    gameState = GameState.Talk;
                    dialogueTriggered = false;
                }
                break;
                
            case GameState.Talk:
                inspectorAnimator.speed = 0;
                
                // Diyaloğu bir kez tetikle
                if (!dialogueTriggered)
                {
                    dialogueTriggered = true;
                    TouchableController.Instance?.OnInspectorReachedTarget();
                }
                break;
                
            case GameState.MiniGame:
                inspectorAnimator.speed = 0;
                break;
                
            case GameState.Cutscene:
                inspectorAnimator.speed = 0;
                break;
        }
    }
    
    /// <summary>
    /// Diyalog bittiğinde çağrılır
    /// </summary>
    public void OnDialogueEnded()
    {
        if (gameState == GameState.Talk)
        {
            gameState = GameState.Free;
        }
    }

    public void ResetCameraToInitialPosition()
    {
        Camera.main.transform.DOMove(initialCameraPosition, 1f).SetEase(Ease.Linear);
        DOVirtual.Float(Camera.main.orthographicSize, initialOrtographicSize, 1f, (value) => Camera.main.orthographicSize = value).SetEase(Ease.InOutQuad);
    }
}
