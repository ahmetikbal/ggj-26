using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using Sirenix.OdinInspector;

public class IconGenerator : MonoBehaviour
{
    public string pathFolder = "0-Project/UI";

    private Camera camera;

    public bool useMultipleObjects;

    public List<GameObject> sceneObjects;



#if UNITY_EDITOR
    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    [Button, ContextMenu("Screenshot")]
    private void ProcessScreenshots()
    {
        StartCoroutine(Screenshot());
    }

    private IEnumerator Screenshot()
    {
        if(camera == null)
            camera = GetComponent<Camera>();

        if (useMultipleObjects)
        {
            foreach (var obje in sceneObjects)
            {
                obje.SetActive(false);
                yield return null;
            }

            foreach (var obje in sceneObjects)
            {
                obje.SetActive(true);

                TakeScreenshot(GiveId());

                obje.SetActive(false);

                yield return null;
            }
        }
        else
        {
            
            TakeScreenshot(GiveId());

            yield return null;


        }

    }

    int GiveId()
    {
        int i = UnityEngine.Random.Range(0, 1000000);

        Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/{pathFolder}/Icon_{i}.png");

        if (s != null)
            i = UnityEngine.Random.Range(0, 1000000);

        return i;
    }

    void TakeScreenshot(int i)
    {
        string fullPath = $"{Application.dataPath}/{pathFolder}/Icon_{i}.png";

        RenderTexture rt = new RenderTexture(1024, 1024, 32);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 1024, 1024), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;
        if (Application.isEditor)
        {
            DestroyImmediate(rt);
        }
        else
        {
            Destroy(rt);
        }
        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);


        AssetDatabase.Refresh();

    }
#endif
}
