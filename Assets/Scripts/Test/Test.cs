using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    float elapsedTime = 0.0f;

    public Vector3 startVec = new Vector3(10, 5, 10);

    public List<Vector3> points = new List<Vector3>(); // 선을 그릴 포인트들
    public Color gizmoColor = Color.green; // Gizmo 색상

    float magnitude;
    private void Start()
    {
        magnitude = startVec.magnitude;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        Debug.Log($"Start {Vector3.SqrMagnitude(startVec)}");
        Vector3 endVec = (Quaternion.AngleAxis(-5, Vector3.up) * startVec.normalized) * magnitude;
        Debug.Log($"End {Vector3.SqrMagnitude(endVec)}");
        startVec = Vector3.Slerp(startVec, endVec, 0.1f);

        

        if (points.Count == 0 || (points.Count > 0 && points[points.Count - 1] != transform.position))
        {
            points.Add(endVec);
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        if (points.Count > 1)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                Gizmos.DrawLine(points[i], points[i + 1]);
            }
        }
    }
}
