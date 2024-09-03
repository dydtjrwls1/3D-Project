using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class RotateObject : MonoBehaviour, IHit
{
    public float rotateForce = 180.0f;
   
    public Transform target;

    Rigidbody rb;

    public void OnHit()
    {

    }

    private void Awake()
    {
        rb = target.GetComponent<Rigidbody>();
        if(rb == null)
        {
            Debug.LogWarning($"{this.name} doesn't have a Rigidbody.");
        }
    }
    

    private void FixedUpdate()
    {
        // target.Rotate(Time.deltaTime * rotateForce * Vector3.up);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, Time.fixedDeltaTime * rotateForce, 0));
    }
}
