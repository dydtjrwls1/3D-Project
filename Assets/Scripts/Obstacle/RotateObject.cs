using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotateForce = 180.0f;
    // public float hitForce = 7.5f;
   
    public Transform target;

    private void Awake()
    {
        if (target == null)
            target = transform.GetChild(0);
    }

    private void Update()
    {
        transform.Rotate(Time.deltaTime * rotateForce * Vector3.up);
    }

    //public void OnHit(Rigidbody target, Collision _)
    //{
    //    Transform trans = target.transform;
    //    target.AddForce((trans.up + -trans.forward) * hitForce, ForceMode.Impulse);
    //}
}
