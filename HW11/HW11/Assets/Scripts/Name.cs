using System.Collections.Generic;
using UnityEngine;

public class Name : MonoBehaviour
{
    [Header("Text Settings")]
    public string textInput = "竣崴";
    public Font textFont;
    [Range(5, 50)]
    public int resolution = 15;       

    [Header("Shape Adjustment")]
    [Range(0.5f, 3.0f)]
    public float heightScale = 1.5f;  
    [Range(0.5f, 3.0f)]
    public float widthScale = 1.0f;   

    [Header("Wall Settings")]
    public float blockSize = 0.5f;

    private List<GameObject> wallObjects = new List<GameObject>();
    private bool hasCollapsed = false;

    void Start()
    {
        if (textFont == null)
        {
            Debug.LogError("請在 Inspector 中指定 Font (字型)！");
            return;
        }


        foreach (var obj in wallObjects) Destroy(obj);
        wallObjects.Clear();

        int[,] grid = ConvertTextToGrid(textInput, textFont);
        GenerateWall(grid);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !hasCollapsed)
        {
            CollapseWall();
        }
    }

    int[,] ConvertTextToGrid(string text, Font font)
    {

        GameObject tempObj = new GameObject("TempTextObject");
        TextMesh textMesh = tempObj.AddComponent<TextMesh>();
        textMesh.text = text;
        textMesh.font = font;
        textMesh.fontSize = 100; 
        textMesh.color = Color.white;
        textMesh.anchor = TextAnchor.MiddleCenter;
        tempObj.GetComponent<MeshRenderer>().material = font.material;

    
        Bounds bounds = tempObj.GetComponent<Renderer>().bounds;

        GameObject camObj = new GameObject("TempCamera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.backgroundColor = Color.black;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.orthographic = true;

        cam.orthographicSize = bounds.extents.y * 1.1f;

    
        int rtWidth = 1024;
        int rtHeight = 512;
        RenderTexture rt = new RenderTexture(rtWidth, rtHeight, 24);
        rt.filterMode = FilterMode.Point;
        cam.targetTexture = rt;


        tempObj.transform.position = new Vector3(1000, 1000, 0);
        camObj.transform.position = new Vector3(1000, 1000, -10);

        cam.Render();


        RenderTexture.active = rt;
        Texture2D resultTex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        resultTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        resultTex.Apply();


        RenderTexture.active = null;
        Destroy(rt);
        Destroy(tempObj);
        Destroy(camObj);


        float aspectRatio = bounds.size.x / bounds.size.y;

 
        int gridH = Mathf.CeilToInt(resolution * heightScale);

 
        int gridW = Mathf.CeilToInt(gridH * aspectRatio * widthScale);

        int[,] grid = new int[gridH, gridW];


        for (int y = 0; y < gridH; y++)
        {
            for (int x = 0; x < gridW; x++)
            {
   
                float u = (float)x / gridW;
                float v = (float)y / gridH;

  
                if (u < 0 || u > 1 || v < 0 || v > 1) continue;

                Color pixelColor = resultTex.GetPixelBilinear(u, v);

                if (pixelColor.r > 0.4f) 
                    grid[y, x] = 1;
                else
                    grid[y, x] = 0;
            }
        }

        return grid;
    }

    void GenerateWall(int[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);
        float startX = -(cols * blockSize) / 2.0f + (blockSize / 2.0f);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int gridValue = grid[y, x];

                Vector3 position = new Vector3(
                    startX + (x * blockSize),
                    y * blockSize + (blockSize / 2.0f),
                    0
                );

                GameObject block;
                if (gridValue == 1)
                {
                    block = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    block.GetComponent<Renderer>().material.color = Color.red;
                    block.name = $"Letter";
                }
                else
                {
                    block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
                    block.name = $"Bg";
                }

                block.transform.position = position;
                block.transform.localScale = Vector3.one * blockSize;
                block.transform.parent = this.transform;
                wallObjects.Add(block);
            }
        }
    }

    void CollapseWall()
    {
        hasCollapsed = true;
        foreach (GameObject obj in wallObjects)
        {
            if (obj.GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = obj.AddComponent<Rigidbody>();
                rb.AddForce(Random.insideUnitSphere * 2f, ForceMode.Impulse);
            }
        }
    }
}