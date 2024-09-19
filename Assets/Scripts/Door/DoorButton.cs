using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public Door targetDoor;
    public Transform targetPoint;

    Transform button;

    bool canUse = true;

    private void Awake()
    {
        if(targetDoor.GetComponent<IUsable>() == null)
        {
            Debug.LogWarning($"{targetDoor.name} 은 사용 가능하지 않습니다.");
        }

        button = transform.GetChild(0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (canUse)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                canUse = false;
                targetDoor.Use();
                StartCoroutine(Press());
            }
        }
    }

    IEnumerator Press()
    {
        while (button.localPosition.y > targetPoint.localPosition.y)
        {
            button.Translate(Time.deltaTime * Vector3.down);
            yield return null;
        }
    }
}
