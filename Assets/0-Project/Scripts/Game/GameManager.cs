using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

    public enum GameState
    {
        Free,
        MoveToTarget,
        Talk,
        MiniGame
    }

    public GameState gameState;

    public Animator inspectorAnimator;
    public Transform lastTargetTransformForInspector;
    [HideInInspector] public Vector3 nextCameraPosition;
    [HideInInspector] public float nextOrtographicSize;

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
                break;
            case GameState.MoveToTarget:
                inspectorAnimator.speed = 1;
                if(InspectorNavMesh.Instance.navMeshAgent.remainingDistance <= 0.1f)
                {
                    Camera.main.transform.DOMove(nextCameraPosition, 1f).SetEase(Ease.Linear);
                    DOVirtual.Float(Camera.main.orthographicSize, nextOrtographicSize, 1f, (value) => Camera.main.orthographicSize = value).SetEase(Ease.InOutQuad);
                    gameState = GameState.Talk;
                }
                break;
            case GameState.Talk:
            inspectorAnimator.speed = 0;
            
                break;
            case GameState.MiniGame:
                break;
        }
    }
}
