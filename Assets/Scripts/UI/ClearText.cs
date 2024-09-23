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
        goal.onClear += () => { text.color = orgColor; };
    }
}
