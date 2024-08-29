using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    Transform secondsHinge, minutesHinge, hoursHinge;

    float secondsToDigrees = 6.0f, minutesToDigrees = 6.0f, hoursToDigrees = 30.0f;

    private void Awake()
    {
        secondsHinge = transform.GetChild(0);
        minutesHinge = transform.GetChild(1);
        hoursHinge = transform.GetChild(2);
    }

    private void Update()
    {
        TimeSpan time = DateTime.Now.TimeOfDay;
        secondsHinge.localRotation = Quaternion.Euler(0f, 0f, secondsToDigrees * (float)time.TotalSeconds);
        minutesHinge.localRotation = Quaternion.Euler(0f, 0f, minutesToDigrees * (float)time.TotalMinutes);
        hoursHinge.localRotation = Quaternion.Euler(0f, 0f, hoursToDigrees * (float)time.TotalHours);
    }
}
