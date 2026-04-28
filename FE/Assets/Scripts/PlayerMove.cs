using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    float speed = 5f;
    float heading = 0f;
    float scale = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        if (v != 0.0f) {
            transform.position += transform.forward * v * Time.deltaTime * speed;
            //transform.Translate(transform.forward * v * speed *Time.deltaTime, Space.Self);
            Debug.Log("v:" + v + ", " + transform.forward.ToString() );
        }
        if (h != 0.0f)
        {
            heading += h;
            transform.rotation = Quaternion.Euler(0, heading * scale, 0);
            Debug.Log("heading: " + heading);
        }
    }
}