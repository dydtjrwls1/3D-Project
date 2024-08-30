using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    bool isActivate = false;

    MeshRenderer flag_MeshRenderer;

    public float duration = 1.0f;

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        flag_MeshRenderer = child.GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivate)
        {
            isActivate = true;
            StartCoroutine(Activate());

            Player player = other.GetComponent<Player>();
            player.CurrentSavePoint = transform.position;

        }    
    }

    IEnumerator Activate()
    {
        float time = 0.0f;
        Material flag_Meterial = flag_MeshRenderer.material;

        while (time < duration)
        {
            time += Time.deltaTime;
            flag_Meterial.color = Color.Lerp(flag_Meterial.color, Color.blue, time / duration);
            yield return null;
        }
    }
}
