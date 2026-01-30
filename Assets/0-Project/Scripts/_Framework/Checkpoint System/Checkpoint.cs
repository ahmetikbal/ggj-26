using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Checkpoint : MonoBehaviour
{
    public int checkpointOrder;

    // CHECKPOINT SYSTEM WHICH WORKING WITH CHECKPOINT, LEVELMANAGER, LEVELSETTINGS AND UIMANAGER SCRIPTS
    /*
    
    public void CreateStar()
    {
        UIManager.Instance.CreateCheckpointStar(LevelManager.Instance.CalculateProgressBarRatio(transform.position));

        if (checkpointOrder <= LevelManager.checkpoint) //HAS BEEN PASSED
        {
            UIManager.Instance.UpdateCheckpointStar(checkpointOrder);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            CheckpointPassed();
    }

    public void CheckpointPassed()
    {
        if (checkpointOrder > LevelManager.checkpoint && (GameManager.Instance._gameState != GameManager.GameState.GameOver))
        {
            LevelManager.checkpoint = checkpointOrder;
            UIManager.Instance.UpdateCheckpointStar(checkpointOrder);
        }
    }

    */
}