using UnityEngine;

public class UVPoker : MonoBehaviour
{
    // [0.0, 1.0] 的範圍，代表要顯示的貼圖區域
    // 舉例：要顯示左上角四分之一， MinUV = (0, 0.5), MaxUV = (0.5, 1)
    [Header("要顯示的紋理範圍 (0.0 到 1.0)")]
     Vector2 minUV = new Vector2(0.0f, 0.0f);     // 紋理座標的左下角
    //public Vector2 maxUV = new Vector2(1f, 1f); // 紋理座標的右上角
     Vector2 maxUV = new Vector2(1f, 1f);       // 紋理座標的右上角
    
    static int rows = 5;
    static int cols = 13;

    float pokerW = 1.0f / cols;
    float pokerH = 1.0f / rows;

    int index = 0;

    void Start()
    {
        ApplyCustomUVs();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            index = ++index % 52;
            pokerTexture(index);
            ApplyCustomUVs();
        }
    }

    void pokerTexture(int index)
    {
        int[] rc = indexTorc(index);
        if (rc != null)
        {
            rc[0] = (rows - 1) - rc[0];
        }

        minUV = new Vector2(rc[1]*pokerW, rc[0]*pokerH);
        maxUV = new Vector2((rc[1]+1)*pokerW, (rc[0]+1)*pokerH);
    }

    int[] indexTorc(int index)
    {
        int[] rc = new int[2];
        rc[0] = index / cols;
        rc[1] = index % cols;
        return rc;
    }

    // 可以在 Editor 模式下修改 minUV/maxUV 後，右鍵點擊腳本元件執行
    [ContextMenu("應用自訂 UV 座標")]
    public void ApplyCustomUVs()
    {
        // 取得 Quad 的 MeshFilter 元件
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("找不到 MeshFilter 元件！");
            return;
        }

        Mesh mesh = meshFilter.mesh;
        Vector2[] uv = mesh.uv;

        // Quad 頂點順序通常為：左下(0), 右下(1), 左上(2), 右上(3)
        // 這是 Unity Quad 的標準 UV 頂點順序1p

        // 0. 左下角 (Bottom-Left)
        uv[0] = minUV;

        // 1. 右下角 (Bottom-Right)
        uv[1] = new Vector2(maxUV.x, minUV.y);

        // 2. 左上角 (Top-Left)
        uv[2] = new Vector2(minUV.x, maxUV.y);

        // 3. 右上角 (Top-Right)
        uv[3] = maxUV;

        // 重新賦予修改後的 UV 陣列給 Mesh
        mesh.uv = uv;
    }
}