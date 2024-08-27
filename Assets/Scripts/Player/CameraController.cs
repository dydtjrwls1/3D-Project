using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 5.0f;
    public float zoomSpeed = 1.0f;
    public float maxZoom = 8.0f;
    public float minZoom = 3.0f;

    Player player;
    Transform cameraPoint;
    Transform mainCamera;

    Quaternion nextRotation;                            // cameraPoint의 목표 회전 값
    Vector3 nextZoom;                                   // mainCamera 의 다음 z 값 (줌 정도)

    Vector2 delta;                                      // 마우스 변동 값
    float maxDeltaValue = 30.0f;                        // 마우스 변동 값 최대치
   

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
            this.delta.x = Mathf.Clamp(delta.x, -maxDeltaValue, maxDeltaValue);
            this.delta.y = Mathf.Clamp(delta.y, -maxDeltaValue, maxDeltaValue);
            UpdateRotation(this.delta); // 마우스 위치 변동이 있을때마다 nextRotation 값 갱신
        };

        player.OnScrollInput += (delta, canceled) =>
        {
            if (canceled) // 스크롤 변동 종료 시 nextzoom 은 현재 위치
            {
                nextZoom = mainCamera.localPosition;
                return;
            }
                
            float input = delta > 0.0f ? 1.0f : -1.0f;  // 스크롤 방향이 위면 1 아래면 -1
            float nextZ = Mathf.Clamp(mainCamera.localPosition.z + input, -maxZoom, -minZoom);   // 줌값의 최대 최소값으로 clamp

            nextZoom = new Vector3(mainCamera.localPosition.x, mainCamera.localPosition.y, nextZ);
        };

        nextZoom = mainCamera.localPosition;
    }

    private void LateUpdate()
    {
        CameraRotation();
        Zoom();
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

    void Zoom()
    {
        mainCamera.localPosition = Vector3.Slerp(mainCamera.localPosition, nextZoom, Time.deltaTime * zoomSpeed);
    }
}
