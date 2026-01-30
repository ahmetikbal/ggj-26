using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    public Transform finishTransform;


    #region CHECKPOINT SYSTEM WHICH WORKING WITH CHECKPOINT, LEVELMANAGER, LEVELSETTINGS AND UIMANAGER SCRIPTS
    // CHECKPOINT SYSTEM WHICH WORKING WITH CHECKPOINT, LEVELMANAGER, LEVELSETTINGS AND UIMANAGER SCRIPTS
    /*
    public List<Checkpoint> checkpoints = new List<Checkpoint>();

    private void Awake()
    {
        foreach (Checkpoint checkpoint in GetComponentsInChildren<Checkpoint>())
        {
            checkpoints.Add(checkpoint);
            checkpoint.checkpointOrder = checkpoints.Count - 1;
        }
    }

    private void Start()
    {
        foreach (Checkpoint checkpoint in checkpoints)
            checkpoint.CreateStar();
    }
    */
    #endregion

}