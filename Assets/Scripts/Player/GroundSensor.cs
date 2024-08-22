using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    // 바닥에 닿았음을 알리는 델리게이트
    public Action<bool> onGround;

    // 트리거에 여러 오브젝트가 닿았을 때를 대비하기 위한 변수
    int groundCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        groundCount++;

        if(groundCount > 0) // 트리거에 하나 이상의 물체가 닿았으면 true로 알림
            onGround?.Invoke(true);
    }

    private void OnTriggerExit(Collider other)
    {
        groundCount--;

        if(groundCount < 1) // 트리거에 들어있는 물체가 없을 때 false 로 알림
        {
            onGround?.Invoke(false);
            groundCount = 0;
        }
            
    }
}
