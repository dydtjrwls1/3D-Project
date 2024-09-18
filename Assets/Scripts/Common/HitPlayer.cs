using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPlayer : MonoBehaviour
{
    public float hitForce = 20.0f;
    public float hitTime = 1.0f;

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Vector3 hitDIr = contact.normal;
                collision.gameObject.GetComponent<PlayerController>().HitPlayer(-hitDIr * hitForce, hitTime);
                return;
            }
        }
    }
}
