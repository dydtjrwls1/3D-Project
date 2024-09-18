using Cinemachine;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.UI;

public class ChargingState : IPlayerState
{
    Transform body;
    Transform root;
    Transform cameraPoint;
    Cinemachine3rdPersonFollow framingTransposer;
    Image chargingImage;

    public ChargingState(PlayerController player)
    {
        body = player.body;
        root = player.root;
        cameraPoint = player.cameraPoint;
        chargingImage = player.ChargingImage;

        CinemachineVirtualCamera vcam = player.PlayerMainCam;
        framingTransposer = framingTransposer ?? vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        if (framingTransposer == null)
        {
            Debug.LogError("CinemachineFramingTransposer component not found.");
        }
    }

    public void EnterState(PlayerController player)
    {
        chargingImage.fillAmount = 0.0f;
    }

    public void ExitState(PlayerController player)
    {
        Debug.Log("EXIT");
        // 카메라 원위치
        framingTransposer.CameraDistance = player.DefaultCameraDistance;

        // Player 몸통 원위치
        body.localRotation = Quaternion.identity;

        chargingImage.fillAmount = 0.0f;
    }

    public void UpdateState(PlayerController player)
    {
        if (player.IsHit) 
        {
            player.SetState(player.IdleState); // Charging 도중 장애물에 Hit 시 Idle 로 돌아간다.
            return;
        }
        
        player.onCharging?.Invoke(player.CurrentChargeDelta);
        // 플레이어 몸통 회전 + 카메라 점점 가까이 + Mesh를 카메라가 바라보는 방향으로
        Quaternion lookPoint = Quaternion.Euler(Vector3.up * cameraPoint.localEulerAngles.y);
        root.localRotation = lookPoint;
        body.Rotate(Time.deltaTime * player.CurrentRotateSpeed * Vector3.up, Space.Self);
        framingTransposer.CameraDistance = player.CurrentZoomDistance;

        // charging image 채우기 변화
        chargingImage.fillAmount = player.CurrentChargeDelta;
        chargingImage.transform.localRotation = lookPoint; // 항상 카메라가 바라보는 방향을 향하게 회전
    }
}

public class IdleState : IPlayerState
{
    public void EnterState(PlayerController player)
    {
        
    }

    public void ExitState(PlayerController player)
    {
    }

    public void UpdateState(PlayerController player)
    {
    }
}

public class FireState : IPlayerState
{
    Transform root;
    Rigidbody rb;

    public FireState(PlayerController player)
    {
        rb = player.PlayerRb;
        root = player.root;
    }

    public void EnterState(PlayerController player)
    {
        // 플레이어를 카메라가 바라보는 방향으로 발사
        rb.AddForce(player.CurrentFireForce * (root.up + root.forward), ForceMode.Impulse);
    }

    public void ExitState(PlayerController player)
    {
        // rigidbody의 velocity 초기화
        rb.angularVelocity = Vector3.zero;
    }

    public void UpdateState(PlayerController player)
    {
        
    }
}
