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

    Quaternion nextRotation;                            // cameraPoint�� ��ǥ ȸ�� ��
    Vector3 nextZoom;                                   // mainCamera �� ���� z �� (�� ����)

    float maxDeltaValue = 30.0f;                        // ���콺 ���� �� �ִ�ġ (���콺�� �������� �ʹ� Ŭ ��� ī�޶� Ƣ�°� ������)
   

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
            delta.x = Mathf.Clamp(delta.x, -maxDeltaValue, maxDeltaValue);
            delta.y = Mathf.Clamp(delta.y, -maxDeltaValue, maxDeltaValue);
            UpdateRotation(delta); // ���콺 ��ġ ������ ���������� nextRotation �� ����
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
    /// ���� ȸ�� ������ �����ϴ� �Լ�
    /// </summary>
    /// <param name="delta">���콺 ���� �� (X, Y)</param>
    void UpdateRotation(Vector2 delta)
    {
        // ī�޶��� ���� X ȸ�� 
        float rotationX = Mathf.Clamp(cameraPoint.rotation.eulerAngles.x + (-delta.y) * rotateSpeed, 0, 90.0f);
        float rotationY = cameraPoint.rotation.eulerAngles.y + delta.x * rotateSpeed; // ���� Y ȸ��

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
