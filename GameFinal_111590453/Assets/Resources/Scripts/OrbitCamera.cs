using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform target; // 建議放一個空物件在方塊陣列的中心 (width/2, height/2, depth/2)
    public float distance = 20.0f;
    public float sensitivity = 5.0f;

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
    {
        if (target)
        {
            // 按住滑鼠右鍵旋轉
            if (Input.GetMouseButton(1))
            {
                x += Input.GetAxis("Mouse X") * sensitivity;
                y -= Input.GetAxis("Mouse Y") * sensitivity;
            }

            // 滾輪縮放
            distance -= Input.GetAxis("Mouse ScrollWheel") * 10f;
            distance = Mathf.Clamp(distance, 5f, 50f);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }
}