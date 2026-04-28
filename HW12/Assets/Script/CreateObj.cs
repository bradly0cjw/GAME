using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObj : MonoBehaviour
{
    void Start()
    {
        GameObject root = GameObject.Find("Problem1");
        if (root == null)
        {
            root = new GameObject("Problem1");
        }

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.SetParent(root.transform, false);
        floor.transform.localScale = new Vector3(1.2f, 1f, 1.2f);
        floor.transform.localPosition = Vector3.zero;

        Material _m = Resources.Load("Materials/rock-wall", typeof(Material)) as Material;
        Material _r = Resources.Load("Materials/R", typeof(Material)) as Material;
        Material _b = Resources.Load("Materials/b", typeof(Material)) as Material;
        Material _g = Resources.Load("Materials/g", typeof(Material)) as Material;
        Material _y = Resources.Load("Materials/y", typeof(Material)) as Material;

        var floorRenderer = floor.GetComponent<Renderer>();
        if (_m != null)
            floorRenderer.material = _m;
        else
        {
            floorRenderer.material = new Material(Shader.Find("Standard"));
            floorRenderer.material.color = new Color(0.85f, 0.80f, 0.70f);
        }

        float outerSize = 12f;        
        float borderThickness = 1.2f; 
        float borderHeight = 1f;      
        float y = borderHeight / 2f;  

        float edgeOffset = outerSize / 2f - borderThickness / 2f;
        float borderLengthSingleCorner = outerSize - borderThickness;
        float halfT = borderThickness / 2f;

        CreateBorder(
            root.transform,
            "TopBorder",
            new Vector3(halfT, y, edgeOffset),
            new Vector3(borderLengthSingleCorner, borderHeight, borderThickness),
            _b != null ? _b : null,
            Color.blue);

        CreateBorder(
            root.transform,
            "BottomBorder",
            new Vector3(-halfT, y, -edgeOffset),
            new Vector3(borderLengthSingleCorner, borderHeight, borderThickness),
            _r != null ? _r : null,
            Color.red);

        CreateBorder(
            root.transform,
            "LeftBorder",
            new Vector3(-edgeOffset, y, halfT),
            new Vector3(borderThickness, borderHeight, borderLengthSingleCorner),
            _g != null ? _g : null,
            Color.green);

        CreateBorder(
            root.transform,
            "RightBorder",
            new Vector3(edgeOffset, y, -halfT),
            new Vector3(borderThickness, borderHeight, borderLengthSingleCorner),
            _y != null ? _y : null,
            Color.yellow);
    }

    private void CreateBorder(Transform parent, string name, Vector3 position, Vector3 scale, Material materialOrNull, Color fallbackColor)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent, false);
        cube.transform.localPosition = position;
        cube.transform.localScale = scale;

        var rend = cube.GetComponent<Renderer>();
        if (materialOrNull != null)
        {
            rend.material = materialOrNull;
        }
        else
        {
            rend.material = new Material(Shader.Find("Standard"));
            rend.material.color = fallbackColor;
        }
    }

    void Update()
    {
    }
}
