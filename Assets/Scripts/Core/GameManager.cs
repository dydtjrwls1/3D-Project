using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    PlayerController player;

    public PlayerController Player
    {
        get => player;
    }

    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<PlayerController>();
        if (player == null) 
        {
            Debug.LogWarning("Player가 Scene에 없습니다.");
        }
    }
}
