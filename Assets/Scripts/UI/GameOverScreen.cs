using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : ScreenBase
{
    Button restartButton;

    override protected void Awake()
    {
        base.Awake();
        Transform child = transform.GetChild(0);
        restartButton = child.GetComponent<Button>();
    }

    override protected void Start()
    {
        base.Start();
        restartButton.onClick.AddListener(() => { FadeManager.Instance.FadeAndLoadScene(SceneManager.GetActiveScene().buildIndex); });

        PlayerController player = GameManager.Instance.Player;
        player.onDie += Activate;
    }
}
