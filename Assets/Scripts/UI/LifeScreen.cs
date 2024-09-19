using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeScreen : MonoBehaviour
{
    Image[] lifes = new Image[3];

    private void Awake()
    {
        for (int i = 0; i < lifes.Length; i++)
        {
            Transform child = transform.GetChild(i);
            lifes[i] = child.GetChild(0).GetComponent<Image>();
        }
    }

    void Start()
    {
        PlayerController player = GameManager.Instance.Player;
        player.onHealthChange += (life) =>
        {
            for (int i = 0; i < lifes.Length; i++) 
            {
                lifes[i].color = i < life ? Color.white : Color.clear;
            }
        };
    }

}
