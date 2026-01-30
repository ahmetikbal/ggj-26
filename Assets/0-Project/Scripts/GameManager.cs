using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        NotStarted,
        Started,
        GameOver,
        Win
    }

    public GameState _gameState;

    public GameObject player;

    public int collectedDiamond = 0;
    public int totalDiamond;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        Time.timeScale = GameData.Instance.timeScale;
        //Application.targetFrameRate = _gameData.targetFPS;

        totalDiamond = PlayerPrefs.GetInt("totalDiamond", 0);
    }

    void Update()
    {
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.B))
            Debug.Break();

        if (Input.GetKeyDown(KeyCode.M))
            UnityEditor.EditorWindow.focusedWindow.maximized = !UnityEditor.EditorWindow.focusedWindow.maximized;

        if (Input.GetKeyDown(KeyCode.R))
            UIManager.Instance.RestartTheScene();

        if (Input.GetKeyDown(KeyCode.C))
            UIManager.Instance.creativePresentationPanel.SetActive(true);

#endif
    }

    public void ChangeGameState(GameState state)
    {
        _gameState = state;

        switch (_gameState)
        {
            case GameState.NotStarted:
                //FUNCTIONS
                break;
            case GameState.Started:

                break;
            case GameState.GameOver:

                break;
            case GameState.Win:

                break;
            default:
                break;
        }
    }

    public bool DoIHaveEnoughMoney(int neededMoney)
    {
        return collectedDiamond >= neededMoney;
    }

    public void MoneyAdd(int money)
    {
        collectedDiamond += money;
    }
}