using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static T FindComponent<T>(GameObject parentObject) where T : Component
    {
        // 부모 오브젝트의 자식들을 순회
        foreach (Transform child in parentObject.transform)
        {
            T comp = child.GetComponent<T>();
            // 자식 오브젝트에서 특정 컴포넌트를 찾기
            if (comp != null)
            {
                return comp;  // 컴포넌트를 찾은 경우
            }

            T foundComponent = FindComponent<T>(child.gameObject);
            // 자식 오브젝트의 자식들도 순회 (재귀 호출)
            if (foundComponent != null)
            {
                return foundComponent;
            }
        }

        return null;
    }

    public static T FindInterface<T>(GameObject parentObject) where T : class
    {
        // 부모 오브젝트의 자식들을 순회
        foreach (Transform child in parentObject.transform)
        {
            // 자식 오브젝트에서 특정 인터페이스를 구현한 컴포넌트를 찾기
            T component = child.GetComponent<T>() as T;
            if (component != null)
            {
                return component;  // 인터페이스를 구현한 컴포넌트를 찾은 경우 반환
            }

            // 자식 오브젝트의 자식들도 순회 (재귀 호출)
            T foundComponent = FindInterface<T>(child.gameObject);
            if (foundComponent != null)
            {
                return foundComponent;  // 재귀 호출에서 찾은 경우 반환
            }
        }

        return null;  // 인터페이스를 구현한 컴포넌트를 찾지 못한 경우
    }
}
