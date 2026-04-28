using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hourglass : MonoBehaviour
{
    [Header("Board Settings")]
    [SerializeField] float planeSize = 10f;
    [SerializeField] float tiltDegreesX = 15f;
    [SerializeField] float tiltDegreesZ = 0f;

    [Header("Walls")]
    [SerializeField] float wallHeight = 1f;
    [SerializeField] float wallThickness = 0.2f;

    [Header("Inner Walls (Pattern 3)")]
    [SerializeField] float innerThickness = 0.1f;
    [SerializeField] float innerLen = 4.3f;

    [Header("Spheres")]
    [SerializeField] int sphereCount = 12;
    [SerializeField] float sphereDiameter = 0.86f;

    [Header("Rotation")]
    [SerializeField] float spinDegreesPerSecond = 20f;

    [Header("Camera Adjustment")]
    [SerializeField] float cameraDistance = 15f;
    [SerializeField] float cameraRollAngle = 180f;

    Transform boardRoot;

    void Start()
    {
        Build();
    }

    void Update()
    {
        if (boardRoot != null)
            boardRoot.Rotate(Vector3.up, spinDegreesPerSecond * Time.deltaTime, Space.Self);
    }

    void LateUpdate()
    {
        UpdateCamera();
    }

    void UpdateCamera()
    {
        if (Camera.main != null && boardRoot != null)
        {
            Quaternion tiltRotation = Quaternion.Euler(tiltDegreesX, 0f, tiltDegreesZ);

            Vector3 boardNormal = tiltRotation * Vector3.up;
            Vector3 boardForward = tiltRotation * Vector3.forward;

            Camera.main.transform.position = transform.position + (boardNormal * cameraDistance);

            Quaternion baseRotation = Quaternion.LookRotation(-boardNormal, boardForward);

            Camera.main.transform.rotation = baseRotation * Quaternion.Euler(0f, 0f, cameraRollAngle);
        }
    }

    void Build()
    {
        if (boardRoot != null)
            Destroy(boardRoot.gameObject);

        boardRoot = new GameObject("HourglassBoard").transform;
        boardRoot.SetParent(transform, false);
        boardRoot.position = transform.position;
        boardRoot.rotation = Quaternion.identity;

        boardRoot.rotation = Quaternion.Euler(tiltDegreesX, 0f, tiltDegreesZ);

        var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "Plane";
        plane.transform.SetParent(boardRoot, false);
        plane.transform.localPosition = Vector3.zero;
        plane.transform.localRotation = Quaternion.identity;
        plane.transform.localScale = Vector3.one * (planeSize / 10f);
        EnsureColliderAndRB(plane, isKinematic: true);

        var planeRenderer = plane.GetComponent<Renderer>();
        if (planeRenderer) planeRenderer.material.color = new Color(0.9f, 0.9f, 0.9f);

        CreateOuterWalls(boardRoot, planeSize);
        CreateInnerWallsPattern3(boardRoot, planeSize);

        CreateInvisibleCeiling(boardRoot, planeSize);

        CreateSpheres(boardRoot, planeSize);
    }

    void CreateInvisibleCeiling(Transform parent, float size)
    {
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "InvisibleCeiling";
        ceiling.transform.SetParent(parent, false);


        float ceilingThickness = 0.1f;
        ceiling.transform.localPosition = new Vector3(0, wallHeight + (ceilingThickness * 0.5f), 0);

        ceiling.transform.localScale = new Vector3(size, ceilingThickness, size);

        Destroy(ceiling.GetComponent<Renderer>());

        EnsureColliderAndRB(ceiling, isKinematic: true);
    }

    void CreateInnerWallsPattern3(Transform parent, float size)
    {
        float half = size * 0.5f;
        float len = innerLen;
        float h = wallHeight;
        float t = innerThickness;
        float offset = half - len;

        CreateWall(parent, "TR_Vert", new Vector3(offset, h / 2, half - len * 0.5f), new Vector3(t, h, len));
        CreateWall(parent, "TR_Horz", new Vector3(half - len * 0.5f, h / 2, offset), new Vector3(len, h, t));

        CreateWall(parent, "BL_Vert", new Vector3(-offset, h / 2, -(half - len * 0.5f)), new Vector3(t, h, len));
        CreateWall(parent, "BL_Horz", new Vector3(-(half - len * 0.5f), h / 2, -offset), new Vector3(len, h, t));
    }

    void CreateSpheres(Transform parent, float size)
    {
        float maxOffset = (size * 0.5f) - wallThickness - (sphereDiameter * 0.6f);
        float minOffset = 0.5f;

        float spawnY = 0.5f;
        float minSeparation = sphereDiameter;
        float minSeparationSqr = minSeparation * minSeparation;
        const int maxAttemptsPerSphere = 200;

        var accepted = new List<Vector3>(sphereCount);

        for (int i = 0; i < sphereCount; i++)
        {
            Vector3 localPos;
            bool found = TryGetNonOverlappingSpawn(parent, minOffset, maxOffset, spawnY, accepted, minSeparationSqr, maxAttemptsPerSphere, out localPos);
            if (!found)
            {
                int zone = Random.Range(0, 2);
                float x = 0, z = 0;
                if (zone == 0)
                {
                    x = Random.Range(-maxOffset, -minOffset);
                    z = Random.Range(minOffset, maxOffset);
                }
                else
                {
                    x = Random.Range(minOffset, maxOffset);
                    z = Random.Range(-maxOffset, -minOffset);
                }
                localPos = new Vector3(x, spawnY + (i * 0.15f), z);
            }

            accepted.Add(new Vector3(localPos.x, spawnY, localPos.z));

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = $"Sphere_{i}";
            sphere.transform.localScale = Vector3.one * sphereDiameter;
            sphere.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);

            sphere.transform.SetParent(parent, false);
            sphere.transform.localPosition = localPos;

            if (sphere.GetComponent<SphereCollider>() == null)
                sphere.AddComponent<SphereCollider>();

            Rigidbody rb = sphere.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }

    static bool TryGetNonOverlappingSpawn(
        Transform parent,
        float minOffset,
        float maxOffset,
        float yBase,
        List<Vector3> acceptedXZ,
        float minSeparationSqr,
        int maxAttempts,
        out Vector3 localPos)
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int zone = Random.Range(0, 2);
            float x, z;

            if (zone == 0)
            {
                x = Random.Range(-maxOffset, -minOffset);
                z = Random.Range(minOffset, maxOffset);
            }
            else
            {
                x = Random.Range(minOffset, maxOffset);
                z = Random.Range(-maxOffset, -minOffset);
            }

            var candidate = new Vector3(x, yBase, z);

            bool ok = true;
            for (int i = 0; i < acceptedXZ.Count; i++)
            {
                Vector3 p = acceptedXZ[i];
                float dx = candidate.x - p.x;
                float dz = candidate.z - p.z;
                if ((dx * dx + dz * dz) < minSeparationSqr)
                {
                    ok = false;
                    break;
                }
            }

            if (!ok)
                continue;

            localPos = new Vector3(candidate.x, yBase, candidate.z); 
            return true;
        }

        localPos = default;
        return false;
    }

    void CreateOuterWalls(Transform parent, float size)
    {
        float half = size * 0.5f;
        CreateWall(parent, "Wall_N", new Vector3(0, wallHeight / 2, half - wallThickness / 2), new Vector3(size, wallHeight, wallThickness));
        CreateWall(parent, "Wall_S", new Vector3(0, wallHeight / 2, -half + wallThickness / 2), new Vector3(size, wallHeight, wallThickness));
        CreateWall(parent, "Wall_E", new Vector3(half - wallThickness / 2, wallHeight / 2, 0), new Vector3(wallThickness, wallHeight, size));
        CreateWall(parent, "Wall_W", new Vector3(-half + wallThickness / 2, wallHeight / 2, 0), new Vector3(wallThickness, wallHeight, size));
    }

    void CreateWall(Transform parent, string name, Vector3 localPos, Vector3 size)
    {
        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.SetParent(parent, false);
        wall.transform.localPosition = localPos;
        wall.transform.localRotation = Quaternion.identity;
        wall.transform.localScale = size;
        wall.GetComponent<Renderer>().material.color = new Color(0.8f, 0.3f, 0.3f);
        EnsureColliderAndRB(wall, isKinematic: true);
    }

    static void EnsureColliderAndRB(GameObject go, bool isKinematic)
    {
        if (go.GetComponent<Collider>() == null)
            go.AddComponent<BoxCollider>();

        var rb = go.GetComponent<Rigidbody>();
        if (rb == null)
            rb = go.AddComponent<Rigidbody>();

        rb.isKinematic = isKinematic;
        rb.useGravity = !isKinematic;
    }
}