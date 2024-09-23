using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonScreen : MonoBehaviour
{
    Button[] buttons;

    private void Awake()
    {
        buttons = new Button[transform.childCount];
        for (int i = 0; i < buttons.Length; i++)
        {
            Transform child = transform.GetChild(i);
            Button button = child.GetComponent<Button>();
            buttons[i] = button;

            int sceneNum = i + 1;
            button.onClick.AddListener(() => 
            {
                FadeManager.Instance.FadeAndLoadScene(sceneNum);
            });
        }
    }
}
