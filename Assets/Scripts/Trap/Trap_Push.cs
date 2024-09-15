using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Trap_Push : TrapBase
{
    public float force = 5.0f;

    Material trapMaterial;

    Transform hinge;
    Transform plate;

    float flipTime = 0.3f;
    float flipRotate = 90.0f;
    float FlipRotate => flipRotate / flipTime;

    private void Awake()
    {
        hinge = transform.GetChild(1);
        plate = hinge.GetChild(0);

        Transform child = transform.GetChild(0);
        trapMaterial = child.GetComponent<MeshRenderer>().material;
    }

    protected override void Activate(Collider other)
    {
        base.Activate(other);
        MoveHinge(other);
        StartCoroutine(RotateHinge());
        trapMaterial.color = Color.clear;
    }

    void MoveHinge(Collider other)
    {
        Vector3 toTarget = other.transform.position - transform.position;
        Quaternion lookTarget = Quaternion.LookRotation(toTarget);

        hinge.localPosition = lookTarget * (hinge.forward * 0.5f);
        hinge.localRotation = lookTarget;

        plate.position = transform.position;

        Player player = other.GetComponent<Player>();
        Rigidbody rb = other.GetComponent<Rigidbody>();

        rb?.AddForce(lookTarget * new Vector3(0, 1, 1) * force, ForceMode.Impulse);

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
