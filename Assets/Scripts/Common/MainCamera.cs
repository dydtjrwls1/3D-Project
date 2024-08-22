using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [Header("Camera Data")]
    public float yPosition = 4.5f;
    public float zPosition = 5.0f;
    public float xRotation = 30.0f;

    [Header("POV Camera Data")]
    public float yPOVPosition = 2.3f;
    public float zPOVPosition = 0.0f;
    public float xPOVRotation = 5.0f;

    bool isPOV = false;

    struct CameraData
    {
        public float y;
        public float z;
        public float xRot;

        public CameraData(float y, float z, float xRot)
        {
            this.y = y;
            this.z = z;
            this.xRot = xRot;
        }
    }

    CameraData data;

    Transform player;

    void Start()
    {
        player = FindAnyObjectByType<Player>().transform;
        data = new CameraData(yPosition, zPosition, xRotation);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Move camera position behind the player per frame.
        transform.position = player.position + (player.up * data.y) + (-player.forward * data.z);
        transform.rotation = Quaternion.Euler(data.xRot, player.rotation.eulerAngles.y, 0);
    }

    public void Switching()
    {
        isPOV = !isPOV;
        if (isPOV)
        {
            data.y = yPOVPosition;
            data.z = zPOVPosition;
            data.xRot = xPOVRotation;
        } else
        {
            data.y = yPosition;
            data.z = zPosition;
            data.xRot = xRotation;
        }
        
    }
}
