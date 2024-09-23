using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : SingleTon<FadeManager>
{
    public Image fadeImage;
    public Canvas fadeCanvas;

    public float fadeDuration = 1.0f;



    private void Start()
    {
        DontDestroyOnLoad(fadeCanvas);
    }

    protected override void OnInitialize()
    {
        fadeImage.color = new(1, 1, 1, 0);
    }

    public void FadeAndLoadScene(int sceneNum)
    {
        StartCoroutine(FadeAndLoadSceneCoroutine(sceneNum));
    }

    IEnumerator FadeAndLoadSceneCoroutine(int sceneNum)
    {
        // Fade Out
        fadeCanvas.sortingOrder = 1;
        yield return StartCoroutine(Fade(0, 1));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNum);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        //DynamicGI.UpdateEnvironment();
        // Fade In
        yield return StartCoroutine(Fade(1, 0));
        fadeCanvas.sortingOrder = -1;
    }

    

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0.0f;
        float inverseFadeDuration = 1 / fadeDuration;

        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime * inverseFadeDuration);
            fadeImage.color = color;

            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }
}
