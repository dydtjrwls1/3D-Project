using Cinemachine;
using UnityEngine;

public class ChargingState : IPlayerState
{
    Transform body;
    CinemachineVirtualCamera vcam;
    Cinemachine3rdPersonFollow framingTransposer;

    public void EnterState(PlayerController player)
    {
        body = player.PlayerBody;
        vcam = player.PlayerMainCam;
        framingTransposer = vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        if(framingTransposer == null)
        {
            Debug.LogError("CinemachineFramingTransposer component not found.");
        }
    }

    public void ExitState(PlayerController player)
    {
        //throw new System.NotImplementedException();
    }

    public void UpdateState(PlayerController player)
    {
        body.Rotate(Time.deltaTime * player.CurrentRotateSpeed * Vector3.up);
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
