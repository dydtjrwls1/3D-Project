using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeScreen : MonoBehaviour
{
    Image[] lifes = new Image[3];

    void Start()
    {
        PlayerController player = GameManager.Instance.Player;
        player.onHealthChange += (life) =>
        {

        };
    }

}
