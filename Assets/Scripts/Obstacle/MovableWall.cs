using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class MovableWall : MonoBehaviour
{
    public float speed = 3.0f;
    public float limit = 3.0f;

    float maxLimit;
    float minLimit;

    float direction = 1.0f;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        maxLimit = (transform.localRotation * (transform.position + Vector3.right * limit)).x;
        minLimit = (transform.localRotation * (transform.position + Vector3.right * -limit)).x;

        
    }

    private void FixedUpdate()
    {
        if(rb.position.x > maxLimit || rb.position.x < minLimit)
        {
            direction *= -1.0f;
        }

        rb.MovePosition(rb.position + Time.fixedDeltaTime * direction * speed * (transform.localRotation * Vector3.right));
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position;
        Gizmos.color = Color.green;

        Vector3 minPos = transform.localRotation * (transform.position + Vector3.right * -limit);
        Vector3 maxPos = transform.localRotation * (transform.position + Vector3.right * limit);
        Gizmos.DrawLine(minPos, maxPos);
    }
#endif
}
