using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClearText : MonoBehaviour
{
    Goal goal;

    TextMeshProUGUI text;

    Color orgColor;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        orgColor = text.color;
        text.color = Color.clear;

        goal = FindAnyObjectByType<Goal>();
        goal.onClear += () => 
        {
            text.DOColor(orgColor, 1.0f);
            transform.DOScale(1.5f, 1.0f);
        };
    }
}
