using ElephantSDK;
using GameAnalyticsSDK;
using MoreMountains.NiceVibrations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public GameObject levelsParent;

    [HideInInspector] public int currentLevel;
    [HideInInspector] public static int checkpoint = -1; //CHECKPOINT SYSTEM

    [HideInInspector] public LevelSettings levelSettings; //Current Level Settings

    // LEVEL PROGRESS BAR
    //[HideInInspector] public Vector3 currentLevelFinishArea;
    //[HideInInspector] public float currentLevelLength;

    [BoxGroup("Level Test Settings")] public bool testMode;
    [BoxGroup("Level Test Settings")] public int testLevel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        currentLevel = PlayerPrefs.GetInt("currentLevel", 0);

#if UNITY_EDITOR
        if (testMode)
            currentLevel = testLevel - 1;
#endif

        for (int i = 0; i < levelsParent.transform.childCount; i++)
            levelsParent.transform.GetChild(i).gameObject.SetActive(false);

        //to avoid bugs if some levels are deleted
        if ((currentLevel + 1) > levelsParent.transform.childCount)
        {
            currentLevel = 0;
            PlayerPrefs.DeleteAll();
            UIManager.Instance.RestartTheScene();
        }

        levelsParent.transform.GetChild(currentLevel).gameObject.SetActive(true);
    }

    private void Start()
    {
        int currentLevelText = PlayerPrefs.GetInt("currentLevelText", 0);
        UIManager.Instance.currentLevelText.text = "Level " + (currentLevelText + 1).ToString();
        //UIManager.Instance.nextLevelText.text = (currentLevelText + 2).ToString();

        levelSettings = levelsParent.transform.GetChild(currentLevel).GetComponent<LevelSettings>();

        // LEVEL PROGRESS BAR
        //currentLevelFinishArea = levelSettings.finishTransform.position;
        //currentLevelLength = Vector3.Distance(GameManager.Instance.player.transform.position, currentLevelFinishArea);

        // CHECKPOINT SYSTEM WHICH WORKING WITH CHECKPOINT, LEVELMANAGER, LEVELSETTINGS AND UIMANAGER SCRIPTS
        //if (checkpoint != -1) //IF ONE CHECKPOINT HAS BEEN PASSED
        //    GameManager.Instance.player.transform.position = levelSettings.checkpoints[checkpoint].transform.parent.position - new Vector3(0, 0, 0.5f); //GO TO THE CHECKPOINT
    }

    private void LateUpdate()
    {
        // LEVEL PROGRESS BAR
        //var ratio = CalculateProgressBarRatio(GameManager.Instance.player.transform.position); //CALCULATES CURRENT PLAYER POSITION RATIO
        //UIManager.Instance.UpdateProgressBar(ratio); //UPDATE PROGRESS BAR
    }

    // FOR LEVEL PROGRESS BAR AND CHECKPOINT STAR SYSTEM
    //public float CalculateProgressBarRatio(Vector3 worldPosition)
    //{
    //    return (currentLevelLength - Vector3.Distance(worldPosition, currentLevelFinishArea)) / (currentLevelLength); //RATIO BETWEEN 0 AND 1
    //}


    [Button, BoxGroup("Level Test Settings")]
    public void GameOver()
    {
        if (GameManager.Instance._gameState == GameManager.GameState.Started)
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.GameOver);

            MMVibrationManager.Haptic(HapticTypes.Failure);

            //SDK
            if (ElephantCore.Instance != null)
            {
                int currentLevelTextValue = PlayerPrefs.GetInt("currentLevelText") + 1;
                Elephant.LevelFailed(currentLevelTextValue);
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, currentLevelTextValue.ToString());
            }

            UIManager.Instance.OpenLosePanel();
        }
    }

    [Button, BoxGroup("Level Test Settings")]
    public void LevelCompleted()
    {
        if (GameManager.Instance._gameState == GameManager.GameState.Started)
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.Win);

            MMVibrationManager.Haptic(HapticTypes.Success);

            //SDK
            if (ElephantCore.Instance != null)
            {
                int currentLevelTextValue = PlayerPrefs.GetInt("currentLevelText") + 1;
                Elephant.LevelCompleted(currentLevelTextValue);
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, currentLevelTextValue.ToString());
            }

            if (currentLevel != levelsParent.transform.childCount - 1)
                currentLevel += 1;
            else
                currentLevel = GameData.Instance.firstLevelAfterFinish;

            if (!testMode)
            {
                PlayerPrefs.SetInt("currentLevel", currentLevel);
                PlayerPrefs.SetInt("currentLevelText", PlayerPrefs.GetInt("currentLevelText") + 1);
            }

            UIManager.Instance.OpenWinPanel();
        }
    }

}