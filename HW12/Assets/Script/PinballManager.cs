using UnityEngine;

public class PinballManager : MonoBehaviour
{
    [Header("物件設定")]
    [Tooltip("請拖入地板 (Plane) 物件")]
    public Transform boardPlane;

    [Tooltip("請拖入釘柱 Prefab")]
    public GameObject pinPrefab;

    [Tooltip("請拖入小圓球 Prefab")]
    public GameObject ballPrefab;

    [Header("生成參數")]
    [Tooltip("要生成多少個釘柱")]
    public int pinCount = 30;

    [Tooltip("地板傾斜角度 (X軸)")]
    public float tiltAngle = 30f;

    void Start()
    {
        if (boardPlane == null || pinPrefab == null || ballPrefab == null)
        {
            Debug.LogError("請在 Inspector 中設定所有欄位！");
            return;
        }

        GeneratePins();
        TiltBoard();
    }

    void Update()
    {
        // 按下空白鍵發射球
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnBall();
        }
    }

    /// <summary>
    /// 在 Plane 上隨機生成釘柱
    /// </summary>
    void GeneratePins()
    {
        // 假設 Plane 是標準 10x10 大小 (Local Scale = 1)
        // Plane 的局部座標範圍大約是 x: -5~5, z: -5~5
        // 我們稍微內縮一點，避免釘子長在邊緣 (-4 ~ 4)

        for (int i = 0; i < pinCount; i++)
        {
            // 1. 生成隨機局部座標 (Local Position)
            // 注意：Plane 是躺平的，所以垂直面是 Y 軸，我們要在 X, Z 平面上分佈
            float randX = Random.Range(-4.0f, 4.0f);
            float randZ = Random.Range(-4.0f, 4.0f);

            // 稍微把 Y 提高一點點，避免與地面穿插
            Vector3 localPos = new Vector3(randX, 0.2f, randZ);

            // 2. 生成物件，並直接設定父物件為 boardPlane
            // 這樣釘柱就會變成 Plane 的子物件
            GameObject newPin = Instantiate(pinPrefab, boardPlane);

            // 3. 設定局部座標
            newPin.transform.localPosition = localPos;

            // 4. 確保角度歸零 (跟隨父物件)
            newPin.transform.localRotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// 傾斜地板
    /// </summary>
    void TiltBoard()
    {
        // 繞著 X 軸旋轉，讓球受重力影響滾下來
        boardPlane.rotation = Quaternion.Euler(tiltAngle, 0, 0);
    }

    /// <summary>
    /// 在上方隨機位置生成球
    /// </summary>
    void SpawnBall()
    {
        // 我們希望球從地板的「上方」(局部座標的 +Z 方向) 出現
        // 並在 X 軸上隨機分佈
        float randX = Random.Range(-3.0f, 3.0f);

        // 設定球的生成點 (相對於 Board 的局部座標)
        // Z = 4.5 代表在地板頂端
        // Y = 1.0 代表稍微懸空掉落
        Vector3 spawnLocalPos = new Vector3(randX, 1.0f, 4.5f);

        // 將局部座標轉換為世界座標 (World Position)，因為 Instantiate 預設使用世界座標較直觀
        Vector3 spawnWorldPos = boardPlane.TransformPoint(spawnLocalPos);

        Instantiate(ballPrefab, spawnWorldPos, Quaternion.identity);
    }
}