using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Trap_Push : TrapBase
{
    public float force = 5.0f;

    Transform hinge;
    Transform plate;

    float flipTime = 0.3f;
    float flipRotate = 90.0f;
    float FlipRotate => flipRotate / flipTime;

    private void Awake()
    {
        hinge = transform.GetChild(1);
        plate = hinge.GetChild(0);
    }

    protected override void Activate(Collider other)
    {
        base.Activate(other);
        MoveHinge(other);
        StartCoroutine(RotateHinge());
    }

    void MoveHinge(Collider other)
    {
        Vector3 toTarget = other.transform.position - transform.position;
        Quaternion lookTarget = Quaternion.LookRotation(toTarget);

        hinge.localPosition = lookTarget * (hinge.forward * 0.5f);
        hinge.localRotation = lookTarget;

        plate.position = transform.position;
    }

    IEnumerator RotateHinge()
    {
        float elapsedTime = 0;
        while(elapsedTime < flipTime)
        {
            elapsedTime += Time.deltaTime;
            hinge.Rotate(Vector3.right * Time.deltaTime * FlipRotate, Space.Self);
            yield return null;
        }
    }
}
