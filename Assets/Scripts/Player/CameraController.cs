using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 5.0f;

    Player player;
    Transform cameraPoint;

    Quaternion nextRotation;                            // cameraPoint�� ��ǥ ȸ�� ��

    Vector2 delta;                                      // ���콺 ���� ��
    float maxDeltaValue = 30.0f;                        // ���콺 ���� �� �ִ�ġ

    private void Awake()
    {
        player = GetComponent<Player>();
        cameraPoint = transform.GetChild(1);
    }

    private void Start()
    {
        // ���콺 �������� ���� ������ ���콺 ��ġ ���氪�� delta �� ����
        player.OnMouseInput += (delta) => {
            this.delta.x = Mathf.Clamp(delta.x, -maxDeltaValue, maxDeltaValue);
            this.delta.y = Mathf.Clamp(delta.y, -maxDeltaValue, maxDeltaValue);
            UpdateRotation(this.delta); // ���콺 ��ġ ������ ���������� nextRotation �� ����
        }; 
    }

    private void LateUpdate()
    {
        CameraRotation();
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
}
