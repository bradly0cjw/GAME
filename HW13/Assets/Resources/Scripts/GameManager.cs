using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("請拖入製作好的 1x1x3 積木 Prefab")]
    public GameObject blockPrefabOdd;
    public GameObject blockPrefabEven;

    [Tooltip("堆疊層數")]
    public int layerCount = 5;

    void Start()
    {
        GenerateTower();
    }

    void Update()
    {
        HandleInput();
    }

    /// <summary>
    /// 生成疊疊樂塔
    /// </summary>
    void GenerateTower()
    {
        if (blockPrefabOdd == null || blockPrefabEven == null)
        {
            Debug.LogError("請指定 Block Prefab！");
            return;
        }

        // 每個積木的寬度是 1，長度是 3。
        // 我們希望每層有三個積木並排。
        // 當積木中心點在 0 時，旁邊兩個積木的中心點應為 -1 和 1。

        for (int y = 0; y < layerCount; y++)
        {
            // 判斷是否為偶數層 (0, 2, 4...)
            // 偶數層與奇數層方向垂直 (0度 vs 90度)
            bool isEvenLayer = (y % 2 == 0);

            // 設定旋轉角度
            Quaternion rotation = isEvenLayer ? Quaternion.identity : Quaternion.Euler(0, 90, 0);

            // 計算高度位置：
            // 假設積木 Pivot 在中心，高度為 1，則第一層 y=0.5，第二層 y=1.5
            float yPos = y * 1.0f + 0.5f;

            for (int i = -1; i <= 1; i++)
            {
                // 計算水平位置
                // 如果是偶數層，沿著 X 軸排列 (-1, 0, 1)
                // 如果是奇數層，沿著 Z 軸排列 (-1, 0, 1)
                Vector3 position;

                if (isEvenLayer)
                {
                    // 偶數層：積木長邊平行於 Z，故沿 X 軸並排
                    position = new Vector3(i, yPos, 0);
                }
                else
                {
                    // 奇數層：積木長邊平行於 X (因為轉了90度)，故沿 Z 軸並排
                    position = new Vector3(0, yPos, i);
                }

                Instantiate(isEvenLayer ? blockPrefabEven : blockPrefabOdd, position, rotation);
            }
        }
    }

    /// <summary>
    /// 處理滑鼠點擊與移除積木
    /// </summary>
    void HandleInput()
    {
        // 偵測滑鼠左鍵點擊 (0)
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 發射射線偵測碰撞
            if (Physics.Raycast(ray, out hit))
            {
                // 檢查是否點擊到有 Rigidbody 的物體 (即我們的積木)
                if (hit.collider.attachedRigidbody != null)
                {
                    // 銷毀該物件，觸發物理崩塌
                    Destroy(hit.collider.gameObject);

                    // (選用) 可以在這裡加一點特效或聲音
                    Debug.Log("積木已移除：" + hit.collider.name);
                }
            }
        }
    }
}