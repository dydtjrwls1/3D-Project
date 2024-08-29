using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    bool isActivate = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivate)
        {
            isActivate = true;
            Player player = other.GetComponent<Player>();
            player.CurrentSavePoint = transform.position;
        }    
    }
}
