using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private Vector3 option1 = Vector3.right;
    private Vector3 option2 = Vector3.up;
    private Vector3 option3 = Vector3.forward;

    [SerializeField] private int selectedIndex = 0;

    public Vector3 SelectedVector
    {
        get
        {
            switch (selectedIndex)
            {
                case 0: return option1;
                case 1: return option2;
                case 2: return option3;
                default: return Vector3.zero;
            }
        }
    }

    [SerializeField]
    public float speed = 60.0f;

    private void Update()
    {
        transform.Rotate(Time.deltaTime * speed * SelectedVector);
    }

    [CustomEditor(typeof(RotateObject))]
    public class MyScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            RotateObject myScript = (RotateObject)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));

            string[] options = new string[] { "X Axis", "Y Axis", "Z Axis" };
            myScript.selectedIndex = EditorGUILayout.Popup("Select Axis", myScript.selectedIndex, options);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.LabelField("Selected Axis", myScript.SelectedVector.ToString());
        }
    }
}


