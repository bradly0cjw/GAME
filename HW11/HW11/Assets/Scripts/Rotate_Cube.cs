using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Cube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);
            o.transform.position = new Vector3(0, 0.125f + i * 0.25f, 0);
            o.transform.localScale = new Vector3(2, 0.25f, 2);
            o.transform.Rotate(new Vector3(0, i * 3.0f, 0));
            o.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
