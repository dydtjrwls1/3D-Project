using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingleTon<GameManager>
{
    PlayerController player;

    public PlayerController Player
    {
        get => player;
    }

    protected override void OnInitialize()
    {
        // light 업데이트 (scene 을 불러왔을 때 제대로 작동하지 않아서 추가함)
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
        DynamicGI.UpdateEnvironment();

        player = FindAnyObjectByType<PlayerController>();
        if (player == null) 
        {
            Debug.LogWarning("Player가 Scene에 없습니다.");
        }


    }
}
