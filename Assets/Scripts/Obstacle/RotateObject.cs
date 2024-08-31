using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotateForce = 180.0f;
   
    public Transform target;

    Rigidbody rb;

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
        //rb.angularVelocity = Vector3.up * rotateForce;
        rb.AddTorque(Vector3.up * rotateForce, ForceMode.Impulse);
    }
}
