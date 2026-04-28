using UnityEngine;

public class CamOrbit : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("攝影機要繞著誰轉？(通常是塔的中心點)")]
    public Transform target;

    [Tooltip("旋轉速度 (度/秒)")]
    public float rotationSpeed = 20f;

    void LateUpdate()
    {
        if (target != null)
        {
            // 核心邏輯：繞著 target 的位置，以世界座標的 Y 軸 (Vector3.up) 為軸心旋轉
            transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);

            // 確保鏡頭始終盯著目標看 (修正視角偏移)
            transform.LookAt(target);
        }
    }
}