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

    Quaternion nextRotation;                            // cameraPoint�� ��ǥ ȸ�� ��
    Vector3 nextZoom;                                   // mainCamera �� ���� z �� (�� ����)

    Vector2 delta;                                      // ���콺 ���� ��
    float maxDeltaValue = 30.0f;                        // ���콺 ���� �� �ִ�ġ
   

    private void Awake()
    {
        player = GetComponent<Player>();
        cameraPoint = transform.GetChild(1);
        mainCamera = Camera.main.transform;
    }

    private void Start()
    {
        // ���콺 �������� ���� ������ ���콺 ��ġ ���氪�� delta �� ����
        player.OnMouseInput += (delta) => {
            this.delta.x = Mathf.Clamp(delta.x, -maxDeltaValue, maxDeltaValue);
            this.delta.y = Mathf.Clamp(delta.y, -maxDeltaValue, maxDeltaValue);
            UpdateRotation(this.delta); // ���콺 ��ġ ������ ���������� nextRotation �� ����
        };

        player.OnScrollInput += (delta, canceled) =>
        {
            if (canceled) // ��ũ�� ���� ���� �� nextzoom �� ���� ��ġ
            {
                nextZoom = mainCamera.localPosition;
                return;
            }
                
            float input = delta > 0.0f ? 1.0f : -1.0f;  // ��ũ�� ������ ���� 1 �Ʒ��� -1
            float nextZ = Mathf.Clamp(mainCamera.localPosition.z + input, -maxZoom, -minZoom);   // �ܰ��� �ִ� �ּҰ����� clamp

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
        // ī�޶��� ���� X ȸ�� (0 ~ 90 ���� ���� ������)
        float rotationX = Mathf.Clamp(cameraPoint.rotation.eulerAngles.x + (-delta.y), 0, 90.0f);
        float rotationY = cameraPoint.rotation.eulerAngles.y + delta.x; // ���� Y ȸ��

        nextRotation = Quaternion.Euler(rotationX, rotationY, cameraPoint.rotation.eulerAngles.z);
    }

    void CameraRotation()
    {
        cameraPoint.rotation = Quaternion.Slerp(cameraPoint.rotation, nextRotation, Time.deltaTime * cameraSpeed);
        cameraPoint.rotation = Quaternion.Euler(cameraPoint.rotation.eulerAngles.x, cameraPoint.rotation.eulerAngles.y, 0.0f); // Slerp �� �����ϴ� z �� ����
    }

    void Zoom()
    {
        mainCamera.localPosition = Vector3.Slerp(mainCamera.localPosition, nextZoom, Time.deltaTime * zoomSpeed);
    }
}
