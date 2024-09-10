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
        body = player.PlayerBody;
        root = player.root;
        cameraPoint = player.cameraPoint;
        vcam = player.PlayerMainCam;
        framingTransposer = vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        if(framingTransposer == null)
        {
            Debug.LogError("CinemachineFramingTransposer component not found.");
        }
    }

    public void ExitState(PlayerController player)
    {
        framingTransposer.CameraDistance = player.defaultCameraDistance;
        body.localRotation = Quaternion.identity;
    }

    public void UpdateState(PlayerController player)
    {
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
        root = player.root;
        rb = player.PlayerRb;

        rb.AddForce(player.CurrentFireForce * (root.up + root.forward), ForceMode.Impulse);
    }

    public void ExitState(PlayerController player)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void UpdateState(PlayerController player)
    {
        
    }
}
