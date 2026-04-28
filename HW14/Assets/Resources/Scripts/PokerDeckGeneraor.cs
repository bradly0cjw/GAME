using UnityEngine;
using System.Collections.Generic;

public class PokerDeckRandom : MonoBehaviour
{
    [Header("素材設定")]
    public Texture2D[] frontTextures;
    public Texture2D backTexture;

    [Header("隨機生成設定")]
    [Tooltip("生成的 3D 範圍大小 (長宽高)")]
    public Vector3 spawnArea = new Vector3(10f, 10f, 10f);
    public Vector3 centerPosition = new Vector3(0f, 0f, 0f);

    [Tooltip("是否要隨機旋轉每張牌的角度？")]
    public bool randomRotation = true;

    [Header("生成張數")]
    [Tooltip("要生成的撲克牌張數 (使用前 n 張 frontTextures)")]
    public int cardCount = 5;

    // 暫存材質以便清理
    private List<Material> createdMaterials = new List<Material>();

    void Start()
    {
        if (backTexture == null)
        {
            Debug.LogError("請設定背面貼圖");
            return;
        }

        if (frontTextures == null || frontTextures.Length == 0)
        {
            Debug.LogError("請至少提供 1 張正面貼圖");
            return;
        }

        if (cardCount < 1)
        {
            cardCount = 1;
        }

        if (cardCount > frontTextures.Length)
        {
            Debug.LogWarning("cardCount 超過 frontTextures 數量，將僅使用可用的前幾張。");
            cardCount = frontTextures.Length;
        }

        GenerateDeck();
    }

    void GenerateDeck()
    {
        Shader cardShader = Shader.Find("Unlit/Texture"); // 或 Mobile/Unlit
        if (cardShader == null) cardShader = Shader.Find("Standard");

        for (int i = 0; i < cardCount; i++)
        {
            // --- 1. 建立結構 (保持不變) ---
            GameObject cardParent = new GameObject($"Card_{i}");
            cardParent.transform.SetParent(this.transform);
            // 在父物件上放置 BoxCollider 以便點擊偵測
            BoxCollider parentCollider = cardParent.AddComponent<BoxCollider>();
            parentCollider.size = new Vector3(1f, 1f, 0.02f); // 覆蓋整張牌，稍微有厚度
            parentCollider.center = Vector3.zero;

            // 正面 Quad
            GameObject frontQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            frontQuad.transform.SetParent(cardParent.transform);
            frontQuad.transform.localPosition = Vector3.zero;
            frontQuad.transform.localRotation = Quaternion.identity;
            // 移除子物件上的碰撞器，避免事件不傳到父物件
            Collider frontCol = frontQuad.GetComponent<Collider>();
            if (frontCol != null) Object.Destroy(frontCol);

            // 背面 Quad (轉 180 度)
            GameObject backQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            backQuad.transform.SetParent(cardParent.transform);
            backQuad.transform.localPosition = Vector3.zero;
            backQuad.transform.localRotation = Quaternion.Euler(0, 180, 0);
            // 背面不需要碰撞器，避免雙面同時點擊
            Collider backCol = backQuad.GetComponent<Collider>();
            if (backCol != null) Object.Destroy(backCol);

            // --- 2. 賦予材質 (保持不變) ---
            Material frontMat = new Material(cardShader);
            frontMat.mainTexture = frontTextures[i];
            frontQuad.GetComponent<Renderer>().material = frontMat;
            createdMaterials.Add(frontMat);

            Material backMat = new Material(cardShader);
            backMat.mainTexture = backTexture;
            backQuad.GetComponent<Renderer>().material = backMat;
            createdMaterials.Add(backMat);

            // --- 3. 隨機位置 (修改處) ---

            // 在 spawnArea 的範圍內隨機取值
            // 例如 x 範圍是 -5 到 5 (如果 spawnArea.x 是 10)
            float randomX = Random.Range((-spawnArea.x + centerPosition.x) / 2, (spawnArea.x + centerPosition.x) / 2);
            float randomY = Random.Range((-spawnArea.y + centerPosition.y) / 2, (spawnArea.y + centerPosition.y) / 2);
            float randomZ = Random.Range((-spawnArea.z + centerPosition.z) / 2, (spawnArea.z + centerPosition.z) / 2);

            cardParent.transform.position = new Vector3(randomX, randomY, randomZ);

            // --- 4. 隨機旋轉 (選用) ---
            if (randomRotation)
            {
                // 隨機產生一個旋轉角度
                cardParent.transform.rotation = Random.rotation;
            }

            // --- 5. 加入翻轉控制 ---
            CardFlip flip = cardParent.AddComponent<CardFlip>();
            flip.flipDuration = 0.5f; // 可調整速度
            flip.startFaceUp = true;  // 一開始顯示正面
            flip.front = frontQuad;
            flip.back = backQuad;
        }
    }

    // 在編輯器中畫出範圍框，方便預覽
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(centerPosition, spawnArea );
    }

    void OnDestroy()
    {
        foreach (Material mat in createdMaterials)
        {
            if (mat != null) Destroy(mat);
        }
    }
}

// 控制撲克牌正反翻轉（點擊觸發），使用 Quaternion.Lerp 平順旋轉
public class CardFlip : MonoBehaviour
{
    [Tooltip("翻轉所需時間（秒）")]
    public float flipDuration = 0.5f;

    [Tooltip("是否從正面開始")]
    public bool startFaceUp = true;

    [HideInInspector]
    public GameObject front;
    [HideInInspector]
    public GameObject back;

    private bool isFaceUp;
    private bool isFlipping;
    private float flipElapsed;
    private Quaternion fromRot;
    private Quaternion toRot;

    void Awake()
    {
        isFaceUp = startFaceUp;
        // 設定初始旋轉（父物件朝向決定要顯示哪一面）
        transform.rotation = isFaceUp ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);
    }

    void Update()
    {
        if (!isFlipping) return;

        flipElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(flipElapsed / flipDuration);
        transform.rotation = Quaternion.Lerp(fromRot, toRot, t);

        if (t >= 1f)
        {
            isFlipping = false;
            isFaceUp = !isFaceUp;
        }
    }

    void OnMouseDown()
    {
        if (isFlipping) return;

        // 由目前狀態決定目標旋轉
        fromRot = transform.rotation;
        toRot = isFaceUp ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.identity;
        flipElapsed = 0f;
        isFlipping = true;
    }
}