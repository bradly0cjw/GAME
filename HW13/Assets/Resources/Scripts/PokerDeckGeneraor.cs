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

    // 暫存材質以便清理
    private List<Material> createdMaterials = new List<Material>();

    void Start()
    {
        if (frontTextures.Length != 52 || backTexture == null)
        {
            Debug.LogError("請確認貼圖素材數量是否正確 (52張正面 + 1張背面)");
            return;
        }

        GenerateDeck();
    }

    void GenerateDeck()
    {
        Shader cardShader = Shader.Find("Unlit/Texture"); // 或 Mobile/Unlit
        if (cardShader == null) cardShader = Shader.Find("Standard");

        for (int i = 0; i < 52; i++)
        {
            // --- 1. 建立結構 (保持不變) ---
            GameObject cardParent = new GameObject($"Card_{i}");
            cardParent.transform.SetParent(this.transform);

            // 正面 Quad
            GameObject frontQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            frontQuad.transform.SetParent(cardParent.transform);
            frontQuad.transform.localPosition = Vector3.zero;
            frontQuad.transform.localRotation = Quaternion.identity;
            Destroy(frontQuad.GetComponent<MeshCollider>());

            // 背面 Quad (轉 180 度)
            GameObject backQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            backQuad.transform.SetParent(cardParent.transform);
            backQuad.transform.localPosition = Vector3.zero;
            backQuad.transform.localRotation = Quaternion.Euler(0, 180, 0);
            Destroy(backQuad.GetComponent<MeshCollider>());

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