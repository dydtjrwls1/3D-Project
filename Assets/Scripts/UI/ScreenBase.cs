using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CanvasGroup))]
public class ScreenBase : MonoBehaviour
{
    [Range(0f, 1f)]
    public float widthRate = 1.0f;

    [Range(0f, 1f)]
    public float heightRate = 1.0f;

    public float activateTime = 1.0f;

    public Color screenColor;

    public bool awakeActivate = true;

    public bool awakeInstantly = true;

    Image screenImage;
    CanvasGroup canvasGroup;
    RectTransform rectTransform;

    float width;
    float height;

    public Action onActivate = null;
    public Action onDeactivate = null;

    protected virtual void Awake()
    {
        screenImage = GetComponent<Image>();
        screenImage.color = screenColor;

        canvasGroup = GetComponent<CanvasGroup>();
        if (!awakeActivate)
        {
            Deactivate();
        }

        rectTransform = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        width = Screen.currentResolution.width * widthRate;
        height = Screen.currentResolution.height * heightRate;

        rectTransform.sizeDelta = new Vector2(width, height);
    }

    public virtual void Activate()
    {
        if (awakeInstantly)
        {
            canvasGroup.alpha = 1.0f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
        else
        {
            StartCoroutine(ActivateCoroutine());
        }

        onActivate?.Invoke();
    }

    IEnumerator ActivateCoroutine()
    {
        float elapsedTime = 0.0f;
        float inverseActivateTime = 1 / activateTime;

        while(elapsedTime < activateTime)
        {
            elapsedTime += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(0.0f, 1.0f, elapsedTime * inverseActivateTime);

            yield return null;
        }

        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public virtual void Deactivate() 
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        onDeactivate?.Invoke();
    }

}
