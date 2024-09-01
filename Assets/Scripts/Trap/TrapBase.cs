using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBase : MonoBehaviour
{
    public bool isOneTime = false;                   // 1회성인지 아닌지 여부

    protected bool isActivatable = true;                       // 활성 가능 상태인지 여부

    protected virtual void OnTriggerEnter(Collider other)
    {
        if(isActivatable && other.CompareTag("Player")) // 활성화 가능 상태일 때 and 들어온 대상이 플레이어일 때
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
    /// 함정이 발동 되었을 때 실행할 함수
    /// </summary>
    protected virtual void Activate(Collider other)
    {
        isActivatable = false;
    }

    /// <summary>
    /// 대상이 함정에서 나갔을 때 실행할 함수
    /// </summary>
    protected virtual void Deactivate()
    {
        // 1회성이 아닐 경우 활성화 가능 상태 키기
        if (!isOneTime)
            isActivatable = true;
    }
}
