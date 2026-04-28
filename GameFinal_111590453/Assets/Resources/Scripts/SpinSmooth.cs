using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinSmooth : MonoBehaviour
{
    [SerializeField] float speedDegreesPerSecond = 20f;

    void Update()
    {
        transform.Rotate(0f, speedDegreesPerSecond * Time.deltaTime, 0f, Space.World);
    }
}
