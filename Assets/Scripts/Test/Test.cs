using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        Debug.Log(Vector3.SignedAngle(new Vector3(-1, 0, -1), Vector3.right, Vector3.up));       
    }

}
