using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f; 
    private int _direction = 1;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _direction = -1; // counterclockwise
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _direction = 1; // clockwise
        }

        transform.Rotate(0f, rotationSpeed * _direction * Time.deltaTime, 0f);
    }
}
