using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jenga : MonoBehaviour
{
    [Header("Existing Scene Objects")]
    [Tooltip("Existing Plane in the scene (stage). Used as center reference for pushing blocks.")]
    [SerializeField] private Transform stagePlane;

    [Header("Tower")]
    [SerializeField, Min(1)] private int layers = 5;
    [SerializeField] private Vector3 blockSize = new Vector3(3f, 0.6f, 1f);
    [SerializeField] private float blockGap = 0.02f;
    [SerializeField] private Vector3 towerBasePosition = new Vector3(0f, 0.3f, 0f);

    [Header("Materials")]
    [Tooltip("Assign 3 wood materials here. Will be randomly chosen per generated block.")]
    [SerializeField] private Material[] woodMaterials = new Material[3];

    [Header("Interaction")]
    [SerializeField] private float impulseForce = 6f;

    private Transform _towerRoot;
    private readonly List<Rigidbody> _blocks = new List<Rigidbody>();

    void Start()
    {
        Bootstrap();
        BuildGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BuildGame();
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryKickSelectedBlock();
        }
    }

    private void Bootstrap()
    {
        ResolveStagePlaneFromScene();

        if (_towerRoot == null)
        {
            var root = GameObject.Find("JengaTower");
            if (root == null) root = new GameObject("JengaTower");
            _towerRoot = root.transform;
        }
    }

    private void ResolveStagePlaneFromScene()
    {
        if (stagePlane != null) return;

        var go = GameObject.Find("StagePlane");
        if (go != null)
        {
            stagePlane = go.transform;
            return;
        }

        go = GameObject.Find("Plane");
        if (go != null) stagePlane = go.transform;
    }

    private void BuildGame()
    {
        Bootstrap();
        ClearTower();
        GenerateTower();
    }

    private void ClearTower()
    {
        _blocks.Clear();

        if (_towerRoot == null) return;

        for (int i = _towerRoot.childCount - 1; i >= 0; i--)
        {
            var child = _towerRoot.GetChild(i);
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(child.gameObject);
            else
#endif
                Destroy(child.gameObject);
        }
    }

    private void GenerateTower()
    {
        if (_towerRoot == null) return;

        for (int layer = 0; layer < layers; layer++)
        {
            bool alongX = (layer % 2 == 0);

            for (int i = 0; i < 3; i++)
            {
                var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                block.name = $"Block_L{layer}_I{i}";
                block.transform.SetParent(_towerRoot, false);

                float y = towerBasePosition.y + layer * (blockSize.y + blockGap);

                Vector3 pos;
                Quaternion rot;

                if (alongX)
                {
                    float z = (i - 1) * (blockSize.z + blockGap);
                    pos = new Vector3(towerBasePosition.x, y, towerBasePosition.z + z);
                    rot = Quaternion.identity;
                    block.transform.localScale = blockSize;
                }
                else
                {
                    float x = (i - 1) * (blockSize.z + blockGap);
                    pos = new Vector3(towerBasePosition.x + x, y, towerBasePosition.z);
                    rot = Quaternion.Euler(0f, 90f, 0f);
                    block.transform.localScale = blockSize;
                }

                block.transform.position = pos;
                block.transform.rotation = rot;

                var rb = block.AddComponent<Rigidbody>();
                rb.mass = 1f;
                rb.interpolation = RigidbodyInterpolation.Interpolate;

                var renderer = block.GetComponent<MeshRenderer>();
                var mat = PickRandomWoodMaterial();
                if (mat != null) renderer.sharedMaterial = mat;

                _blocks.Add(rb);
            }
        }
    }

    private Material PickRandomWoodMaterial()
    {
        if (woodMaterials == null || woodMaterials.Length == 0) return null;

        var valid = new List<Material>(woodMaterials.Length);
        for (int i = 0; i < woodMaterials.Length; i++)
        {
            if (woodMaterials[i] != null) valid.Add(woodMaterials[i]);
        }

        if (valid.Count == 0) return null;
        return valid[Random.Range(0, valid.Count)];
    }

    private void TryKickSelectedBlock()
    {
        var cam = Camera.main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, 200f)) return;

        var rb = hit.collider != null ? hit.collider.attachedRigidbody : null;
        if (rb == null) return;

        Vector3 longAxisWorld = rb.transform.right;

        Vector3 center = (stagePlane != null) ? stagePlane.position : Vector3.zero;
        Vector3 away = (rb.worldCenterOfMass - center);
        if (away.sqrMagnitude > 0.0001f)
        {
            if (Vector3.Dot(longAxisWorld, away) < 0f) longAxisWorld = -longAxisWorld;
        }

        rb.AddForce(longAxisWorld.normalized * impulseForce, ForceMode.Impulse);
    }
}
