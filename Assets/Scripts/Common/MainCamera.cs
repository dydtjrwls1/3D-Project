using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public float zPosition = 5.0f;

    public float yPosition = 4.5f;

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
        transform.position = player.position + (-player.forward * zPosition) + (player.up * yPosition);
        transform.rotation = Quaternion.Euler(xRotation, player.rotation.eulerAngles.y, 0);
    }
}
