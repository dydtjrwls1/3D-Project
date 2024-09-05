using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static T FindComponent<T>(GameObject parentObject) where T : Component
    {
        // �θ� ������Ʈ�� �ڽĵ��� ��ȸ
        foreach (Transform child in parentObject.transform)
        {
            T comp = child.GetComponent<T>();
            // �ڽ� ������Ʈ���� Ư�� ������Ʈ�� ã��
            if (comp != null)
            {
                return comp;  // ������Ʈ�� ã�� ���
            }

            T foundComponent = FindComponent<T>(child.gameObject);
            // �ڽ� ������Ʈ�� �ڽĵ鵵 ��ȸ (��� ȣ��)
            if (foundComponent != null)
            {
                return foundComponent;
            }
        }

        return null;
    }

    public static T FindInterface<T>(GameObject parentObject) where T : class
    {
        // �θ� ������Ʈ�� �ڽĵ��� ��ȸ
        foreach (Transform child in parentObject.transform)
        {
            // �ڽ� ������Ʈ���� Ư�� �������̽��� ������ ������Ʈ�� ã��
            T component = child.GetComponent<T>() as T;
            if (component != null)
            {
                return component;  // �������̽��� ������ ������Ʈ�� ã�� ��� ��ȯ
            }

            // �ڽ� ������Ʈ�� �ڽĵ鵵 ��ȸ (��� ȣ��)
            T foundComponent = FindInterface<T>(child.gameObject);
            if (foundComponent != null)
            {
                return foundComponent;  // ��� ȣ�⿡�� ã�� ��� ��ȯ
            }
        }

        return null;  // �������̽��� ������ ������Ʈ�� ã�� ���� ���
    }
}
