using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IUsable
{
    public Transform target;

    public float speed = 3.0f;

    Transform doorMesh;

    public bool CanUse => true;

    private void Awake()
    {
        doorMesh = transform.GetChild(0);    
    }

    public void Use()
    {
        StartCoroutine(Activate());
    }

    IEnumerator Activate()
    {
        while(doorMesh.position.y > target.position.y)
        {
            doorMesh.Translate(Time.deltaTime * speed * Vector3.down);
            yield return null;
        }
    }

}
