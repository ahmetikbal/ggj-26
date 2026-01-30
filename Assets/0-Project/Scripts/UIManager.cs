using DG.Tweening;
using GameAnalyticsSDK;
using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using ElephantSDK;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject mainMenu;
    public GameObject gameMenu;
    public GameObject losePanel;
    public GameObject winPanel;

    [Header("Level Area")]
    public TextMeshProUGUI currentLevelText, nextLevelText;
    public GameObject levelProgressArea;
    public Image levelProgressBar;

    [Header("Diamond Area")]
    public TextMeshProUGUI collectedDiamondText;
    public RectTransform diamondBaseImage;
    public GameObject diamondPrefabInCanvas;
    public TextMeshProUGUI totalDiamondTextOnWinPanel;

    [Header("Settings")]
    public GameObject settingsArea;
    public Animator settingsAreaAnimator;
    private int settingsStatus = 0;
    public Image hapticIcon, soundIcon;
    public Sprite hapticOn, hapticOff;
    public Sprite soundOn, soundOff;
    private int hapticStatus, soundStatus;

    [Header("Others")]
    public Image winEmoji, loseEmoji;
    public Sprite[] winSprites, loseSprites;
    public GameObject creativePresentationPanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        hapticStatus = PlayerPrefs.GetInt("hapticStatus", 1);
        soundStatus = PlayerPrefs.GetInt("soundStatus", 1);

        hapticIcon.sprite = (hapticStatus == 1) ? hapticOn : hapticOff;
        soundIcon.sprite = (soundStatus == 1) ? soundOn : soundOff;
    }

    public void SettingsButton()
    {
        if (settingsStatus == 0) //closed
        {
            settingsArea.SetActive(true);
            settingsAreaAnimator.SetBool("open", true);
            settingsStatus = 1;
        }
        else
        {
            settingsAreaAnimator.SetBool("open", false);
            settingsStatus = 0;
        }

        MMVibrationManager.Haptic(HapticTypes.Selection);
    }

    public void HapticButton()
    {
        if (hapticStatus == 1)
        {
            MMVibrationManager.SetHapticsActive(false);
            hapticIcon.sprite = hapticOff;

            hapticStatus = 0;
            PlayerPrefs.SetInt("hapticStatus", hapticStatus);
        }
        else
        {
            MMVibrationManager.SetHapticsActive(true);
            hapticIcon.sprite = hapticOn;

            hapticStatus = 1;
            PlayerPrefs.SetInt("hapticStatus", hapticStatus);

            MMVibrationManager.Haptic(HapticTypes.Selection);
        }
    }

    public void SoundButton()
    {
        if (soundStatus == 1)
        {
            AudioListener.pause = true;

            soundIcon.sprite = soundOff;
            soundStatus = 0;
            PlayerPrefs.SetInt("soundStatus", soundStatus);
        }
        else
        {
            AudioListener.pause = false;

            soundIcon.sprite = soundOn;
            soundStatus = 1;
            PlayerPrefs.SetInt("soundStatus", soundStatus);
        }

        MMVibrationManager.Haptic(HapticTypes.Selection);
    }

    public void PrivacyButton() //ROLLIC PRIVACY GUIDE
    {
        Elephant.ShowSettingsView();
    }

    public void RestartTheScene()
    {
        MMVibrationManager.Haptic(HapticTypes.Selection);

        DOTween.KillAll();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenWinPanel()
    {
        levelProgressArea.SetActive(false);

        winEmoji.sprite = winSprites[Random.Range(0, winSprites.Length)];

        winPanel.SetActive(true);
        winPanel.GetComponent<CanvasGroup>().alpha = 0;
        winPanel.GetComponent<CanvasGroup>().DOFade(1f, 1f).SetUpdate(true);
    }

    public void OpenLosePanel()
    {
        levelProgressArea.SetActive(false);

        loseEmoji.sprite = loseSprites[Random.Range(0, loseSprites.Length)];

        losePanel.SetActive(true);
        losePanel.GetComponent<CanvasGroup>().alpha = 0;
        losePanel.GetComponent<CanvasGroup>().DOFade(1f, 1f).SetUpdate(true);
    }

    public void PlayArea()
    {
        mainMenu.SetActive(false);
        GameManager.Instance.ChangeGameState(GameManager.GameState.Started);
        MMVibrationManager.Haptic(HapticTypes.Selection);

        //SDK
        if (ElephantCore.Instance != null)
        {
            int currentLevelTextValue = PlayerPrefs.GetInt("currentLevelText") + 1;
            Elephant.LevelStarted(currentLevelTextValue);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, currentLevelTextValue.ToString());
        }
    }

    //PROGRESS BAR
    public void UpdateProgressBar(float ratio)
    {
        if ((GameManager.Instance._gameState != GameManager.GameState.GameOver) && (GameManager.Instance._gameState != GameManager.GameState.Win))
            levelProgressBar.fillAmount = Mathf.Lerp(levelProgressBar.fillAmount, ratio, Time.deltaTime);
    }

    // CHECKPOINT SYSTEM WHICH WORKING WITH CHECKPOINT, LEVELMANAGER, LEVELSETTINGS AND UIMANAGER SCRIPTS
    #region CHECKPOINT SYSTEM (INSTANTIATE AND UPDATE STAR)

    /*
    public GameObject checkpointStarPrefab;
    public Sprite checkpointStarPassedSprite;

    public void CreateCheckpointStar(float ratio)
    {
        GameObject star = Instantiate(checkpointStarPrefab);
        star.transform.SetParent(levelProgressBar.transform, false);

        var change = levelProgressBar.rectTransform.rect.xMax - levelProgressBar.rectTransform.rect.xMin;
        var lastXPos = levelProgressBar.rectTransform.rect.xMin + (change * ratio);

        star.GetComponent<RectTransform>().localPosition = new Vector2(lastXPos, 35f);
    }

    public void UpdateCheckpointStar(int whichCheckpoint)
    {
        levelProgressBar.transform.GetChild(whichCheckpoint).GetComponent<Image>().sprite = checkpointStarPassedSprite;
    }
    */

    #endregion

    public void UpdateCollectedDiamondText(int amount) //DIAMOND OR COIN
    {
        collectedDiamondText.text = amount.ToString();
    }

    //WORLD TO SCREEN POINT DIAMOND
    public void CreateAndMoveDiamondImage(Vector3 diamondWorldPoint)
    {
        GameObject diamond = Instantiate(diamondPrefabInCanvas, gameMenu.transform);
        diamond.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(diamondWorldPoint);

        diamond.transform.DOMove(diamondBaseImage.position, 1f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            GameManager.Instance.collectedDiamond++;
            UpdateCollectedDiamondText(GameManager.Instance.collectedDiamond);

            GameObject.Destroy(diamond);
        });
    }
}