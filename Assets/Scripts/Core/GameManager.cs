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
        // light ������Ʈ (scene �� �ҷ����� �� ����� �۵����� �ʾƼ� �߰���)
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
        DynamicGI.UpdateEnvironment();

        player = FindAnyObjectByType<PlayerController>();
        if (player == null) 
        {
            Debug.LogWarning("Player�� Scene�� �����ϴ�.");
        }


    }
}
