using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public Image blackEffect;
    public GameObject video, intro;

    void Start()
    {
        // Initial setup: Black effect transparent, video hidden
        if (blackEffect != null)
        {
            Color c = blackEffect.color;
            c.a = 0f;
            blackEffect.color = c;
        }
        
        if (video != null)
            video.SetActive(false);

        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        // 3 seconds delay
        yield return new WaitForSeconds(3f);

        // Fade alpha to 1 smoothly
        if (blackEffect != null)
        {
            // Using DOFade is the cleanest way with DOTween for UI Images
            yield return blackEffect.DOFade(1f, 1f).SetEase(Ease.InOutQuad).WaitForCompletion();
        }

        // Activate video
        if (video != null)
        {
            video.SetActive(true);
            blackEffect.gameObject.SetActive(false);
            intro.SetActive(false);
        }

        // Wait for video duration (17 seconds)
        yield return new WaitForSeconds(17f);

        // Load Game scene
        SceneManager.LoadScene("Game");
    }
}
