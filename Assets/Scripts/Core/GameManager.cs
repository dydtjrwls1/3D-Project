using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    Player player;

    public Player Player
    {
        get => player;
    }

    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<Player>();
        if (player == null) 
        {
            Debug.LogWarning("Player가 Scnen에 없습니다.");
        }
    }
}
