using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Transform target;

    public float hingeHeight = 20.0f;

    [Range(10.0f, 90.0f)]
    public float angle = 30.0f;

    [Range(0.5f, 5.0f)]
    public float speed = 1.0f;

    float elapsedTime = 0.0f;


    private void Awake()
    {
        if (target == null)
            target = transform.GetChild(0);

        target.localPosition = Vector3.up * hingeHeight;

        Transform mesh = target.GetChild(0);
        mesh.localPosition = -Vector3.up * hingeHeight;

        //rb = target.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        elapsedTime += Time.fixedDeltaTime;

        //target.rotation = Quaternion.Euler(Vector3.forward * angle * Mathf.Sin(elapsedTime * speed));
        target.eulerAngles = Vector3.forward * angle * Mathf.Sin(elapsedTime * speed);
    }

    private void Update()
    {
       // elapsedTime += Time.deltaTime;

        //target.eulerAngles = Vector3.forward * angle * Mathf.Sin(elapsedTime * speed);
        
    }

    //public void OnHit(Rigidbody target, Collision _)
    //{
    //    Transform trans = target.transform;
    //    target.AddForce((trans.up + -trans.forward) * hitForce, ForceMode.Impulse);
    //}
}
