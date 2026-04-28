using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenSphere : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject o = null;
            int r = Random.Range(0, 3);
            switch (r)
            {
                case 0:
                    o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    o.transform.position = new Vector3(-5 + 0.5f + i % 10 * 1.0f, 0.5f, -5 + 0.5f + i / 10 * 1.0f);
                    break;
                case 1:
                    o = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    o.transform.position = new Vector3(-5 + 0.5f + i % 10 * 1.0f, 0.5f, -5 + 0.5f + i / 10 * 1.0f);
                    break;
                case 2:
                    o = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    o.transform.position = new Vector3(-5 + 0.5f + i % 10 * 1.0f, 1f, -5 + 0.5f + i / 10 * 1.0f);
                    break;
                case 3:
                    o = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    o.transform.position = new Vector3(-5 + 0.5f + i % 10 * 1.0f, 1f, -5 + 0.5f + i / 10 * 1.0f);
                    break;
                default:
                    o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    break;
            }

            o.GetComponent<Renderer>().material.color = GenerateRandomColor();
            
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
