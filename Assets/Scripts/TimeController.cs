using System;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [SerializeField] private float timeScale = 1f;

    private void Update()
    {
        if (Mathf.Abs(Time.timeScale - timeScale) > Mathf.Epsilon)
        {
            Time.timeScale = timeScale;
        }
    }
}
