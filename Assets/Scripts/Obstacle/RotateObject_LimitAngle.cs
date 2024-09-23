using UnityEngine;

public class RotateObject_LimitAngle : MonoBehaviour
{
    [Range(10.0f, 90.0f)]
    public float angle = 30.0f;

    [Range(0.5f, 5.0f)]
    public float speed = 1.0f;

    public bool randomStart = true;

    float elapsedTime = 0.0f;


    private void Awake()
    {
        if (randomStart) elapsedTime = Random.Range(0, Mathf.PI);
    }

    private void FixedUpdate()
    {
        elapsedTime += Time.fixedDeltaTime;

        //target.rotation = Quaternion.Euler(Vector3.forward * angle * Mathf.Sin(elapsedTime * speed));
        transform.eulerAngles = Vector3.forward * angle * Mathf.Sin(elapsedTime * speed);
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
