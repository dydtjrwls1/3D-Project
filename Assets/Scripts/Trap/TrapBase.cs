using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBase : MonoBehaviour
{
    public bool isOneTime = false;                   // 1ȸ������ �ƴ��� ����

    protected bool isActivatable = true;                       // Ȱ�� ���� �������� ����

    protected virtual void OnTriggerEnter(Collider other)
    {
        if(isActivatable && other.CompareTag("Player")) // Ȱ��ȭ ���� ������ �� and ���� ����� �÷��̾��� ��
        {
            Activate(other);
        }                       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
            Deactivate();
    }

    /// <summary>
    /// ������ �ߵ� �Ǿ��� �� ������ �Լ�
    /// </summary>
    protected virtual void Activate(Collider other)
    {
        isActivatable = false;
    }

    /// <summary>
    /// ����� �������� ������ �� ������ �Լ�
    /// </summary>
    protected virtual void Deactivate()
    {
        // 1ȸ���� �ƴ� ��� Ȱ��ȭ ���� ���� Ű��
        if (!isOneTime)
            isActivatable = true;
    }
}
