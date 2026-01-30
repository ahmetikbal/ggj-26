using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SplashScene : MonoBehaviour
{
    private void Start()
    {
        //One second delay
        DOVirtual.DelayedCall(1f, () => SceneManager.LoadScene(1));
    }
}