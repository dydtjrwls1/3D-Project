using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [Header("Camera Position")]
    public float yPosition = 4.5f;
    public float zPosition = 5.0f;

    [Header("Camera XRotation")]
    public float xRotation = 30.0f;

    Transform player;

    void Start()
    {
        player = FindAnyObjectByType<Player>().transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Move camera position behind the player per frame.
        transform.position = player.position + (player.up * yPosition) + (-player.forward * zPosition);
        transform.rotation = Quaternion.Euler(xRotation, player.rotation.eulerAngles.y, 0);
    }
}
