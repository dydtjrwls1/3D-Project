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

    [SerializeField] private bool randomStart = true;

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

    [SerializeField] float speed = 60.0f;

    float randomAngle;

    private void Start()
    {
        randomAngle = Random.Range(0, 360f);

        transform.eulerAngles = SelectedVector * randomAngle;
    }

    private void Update()
    {
        transform.Rotate(Time.deltaTime * speed * SelectedVector);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RotateObject))]
    public class MyScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            RotateObject myScript = (RotateObject)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("randomStart"));

            string[] options = new string[] { "X Axis", "Y Axis", "Z Axis" };
            myScript.selectedIndex = EditorGUILayout.Popup("Select Axis", myScript.selectedIndex, options);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.LabelField("Selected Axis", myScript.SelectedVector.ToString());
        }

        
    }
#endif
    //[InitializeOnLoadMethod]
    //static void CheckPropertyPaths()
    //{
    //    var so = new SerializedObject(Texture2D.whiteTexture);
    //    var pop = so.GetIterator();

    //    while (pop.NextVisible(true))
    //        Debug.Log(pop.propertyPath);
    //}


}


