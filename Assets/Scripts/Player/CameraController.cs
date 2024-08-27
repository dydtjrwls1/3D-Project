using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 5.0f;

    Player player;
    Transform cameraPoint;

    Quaternion nextRotation;                            // cameraPoint의 목표 회전 값

    Vector2 delta;                                      // 마우스 변동 값
    float maxDeltaValue = 30.0f;                        // 마우스 변동 값 최대치

    private void Awake()
    {
        player = GetComponent<Player>();
        cameraPoint = transform.GetChild(1);
    }

    private void Start()
    {
        // 마우스 움직임이 있을 때마다 마우스 위치 변경값을 delta 에 저장
        player.OnMouseInput += (delta) => {
            this.delta.x = Mathf.Clamp(delta.x, -maxDeltaValue, maxDeltaValue);
            this.delta.y = Mathf.Clamp(delta.y, -maxDeltaValue, maxDeltaValue);
            UpdateRotation(this.delta); // 마우스 위치 변동이 있을때마다 nextRotation 값 갱신
        }; 
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    void UpdateRotation(Vector2 delta)
    {
        // 카메라의 다음 X 회전 (0 ~ 90 사이 값을 가진다)
        float rotationX = Mathf.Clamp(cameraPoint.rotation.eulerAngles.x + (-delta.y), 0, 90.0f);
        float rotationY = cameraPoint.rotation.eulerAngles.y + delta.x; // 다음 Y 회전

        nextRotation = Quaternion.Euler(rotationX, rotationY, cameraPoint.rotation.eulerAngles.z);
    }

    void CameraRotation()
    {
        cameraPoint.rotation = Quaternion.Slerp(cameraPoint.rotation, nextRotation, Time.deltaTime * cameraSpeed);
        cameraPoint.rotation = Quaternion.Euler(cameraPoint.rotation.eulerAngles.x, cameraPoint.rotation.eulerAngles.y, 0.0f); // Slerp 로 변동하는 z 값 고정
    }
}
