using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    Button button;
    Image image;
    TextMeshProUGUI text;

    Color orgTextColor;

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        text = transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        orgTextColor = text.color;
    }

    void Start()
    {
        text.color = Color.clear;
        image.color = Color.clear;
        button.interactable = false;

        GameManager.Instance.Player.onDie += () =>
        {
            image.color = Color.white;
            button.interactable = true;
            text.color = orgTextColor;
        };

        // 현재 신을 Load 한다.
        button.onClick.AddListener(() => { FadeManager.Instance.FadeAndLoadScene(SceneManager.GetActiveScene().buildIndex); });

    }

}
