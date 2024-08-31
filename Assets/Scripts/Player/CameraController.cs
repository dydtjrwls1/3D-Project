using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Range(0f, 1f)]
    public float damp = 1.0f;
    [Range(0f, 2f)]
    public float rotateSpeed = 1.0f;

    public float zoomSpeed = 1.0f;
    public float maxZoom = 8.0f;
    public float minZoom = 3.0f;

    Player player;
    Transform cameraPoint;
    Transform mainCamera;

    Quaternion nextRotation;                            // cameraPoint의 목표 회전 값
    Vector3 nextZoom;                                   // mainCamera 의 다음 z 값 (줌 정도)

    float maxDeltaValue = 30.0f;                        // 마우스 변동 값 최대치 (마우스의 변동값이 너무 클 경우 카메라가 튀는것 방지용)
   

    private void Awake()
    {
        player = GetComponent<Player>();
        cameraPoint = transform.GetChild(1);
        mainCamera = Camera.main.transform;
    }

    private void Start()
    {
        // 마우스 움직임이 있을 때마다 마우스 위치 변경값을 delta 에 저장
        player.OnMouseInput += (delta) => {
            delta.x = Mathf.Clamp(delta.x, -maxDeltaValue, maxDeltaValue);
            delta.y = Mathf.Clamp(delta.y, -maxDeltaValue, maxDeltaValue);
            UpdateRotation(delta); // 마우스 위치 변동이 있을때마다 nextRotation 값 갱신
        };
    }

    private void FixedUpdate()
    {
        CameraRotation();
    }

    private void LateUpdate()
    {
        // Zoom();
    }

    /// <summary>
    /// 다음 회전 방향을 갱신하는 함수
    /// </summary>
    /// <param name="delta">마우스 변동 값 (X, Y)</param>
    void UpdateRotation(Vector2 delta)
    {
        // 카메라의 다음 X 회전 
        float rotationX = Mathf.Clamp(cameraPoint.rotation.eulerAngles.x + (-delta.y) * rotateSpeed, 0, 90.0f);
        float rotationY = cameraPoint.rotation.eulerAngles.y + delta.x * rotateSpeed; // 다음 Y 회전

        nextRotation = Quaternion.Euler(rotationX, rotationY, cameraPoint.rotation.eulerAngles.z);
    }

    void CameraRotation()
    {
        cameraPoint.rotation = Quaternion.Slerp(cameraPoint.rotation, nextRotation, Time.fixedDeltaTime + damp);
        cameraPoint.rotation = Quaternion.Euler(
            cameraPoint.rotation.eulerAngles.x,
            cameraPoint.rotation.eulerAngles.y,
            0);
    }

    void Zoom()
    {
        mainCamera.localPosition = nextZoom;
    }
}
