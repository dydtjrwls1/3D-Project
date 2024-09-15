using Cinemachine;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class ChargingState : IPlayerState
{
    Transform body;
    Transform root;
    Transform cameraPoint;
    CinemachineVirtualCamera vcam;
    Cinemachine3rdPersonFollow framingTransposer;

    public void EnterState(PlayerController player)
    {
        body = body ?? player.PlayerBody;
        root = root ?? player.root;
        cameraPoint = cameraPoint ?? player.cameraPoint;
        vcam = vcam ?? player.PlayerMainCam;
        framingTransposer = framingTransposer ?? vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        if(framingTransposer == null)
        {
            Debug.LogError("CinemachineFramingTransposer component not found.");
        }
    }

    public void ExitState(PlayerController player)
    {
        // 카메라 원위치
        framingTransposer.CameraDistance = player.defaultCameraDistance;

        // Player 몸통 원위치
        body.localRotation = Quaternion.identity;
    }

    public void UpdateState(PlayerController player)
    {
        player.onCharging?.Invoke(player.CurrentChargeDelta);
        // 플레이어 몸통 회전 + 카메라 점점 가까이 + Mesh를 카메라가 바라보는 방향으로
        root.localRotation = Quaternion.Euler(Vector3.up * cameraPoint.localEulerAngles.y);
        body.Rotate(Time.deltaTime * player.CurrentRotateSpeed * Vector3.up, Space.Self);
        framingTransposer.CameraDistance = player.CurrentZoomDistance;
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

    public void EnterState(PlayerController player)
    {
        root = root ?? player.root;
        rb = rb ?? player.PlayerRb;

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
