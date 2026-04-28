using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            o.GetComponent<Renderer>().material.color = GenerateRandomColor();
            o.transform.position = new Vector3(-5 + 0.5f + i % 10 * 1.0f, 0.5f, -5 + 0.5f + i / 10 * 1.0f);

        }
    }
    Color GenerateRandomColor()
    {
        float r = Random.value;
        float g = Random.value;
        float b = Random.value;
        return new Color(r, g, b);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
