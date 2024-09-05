using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSpace : MonoBehaviour, IHit
{
    public float hitForce = 7.5f;

    public void OnHit(Rigidbody target, Collision coll)
    {
        Transform trans = target.transform;
        target.AddForce((trans.up + -trans.forward) * hitForce, ForceMode.Impulse);
    }
}
