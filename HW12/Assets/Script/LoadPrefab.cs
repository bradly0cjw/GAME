using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPrefab : MonoBehaviour
{
    [SerializeField] private string prefabName = "Prefab/Cross3D";
    [SerializeField] private Vector2 randomScaleRange = new Vector2(0.2f, 1.2f);
    [SerializeField] private float spawnHeight = 10f;
    [SerializeField] private Vector2 spawnXRange = new Vector2(-5f, 5f);
    [SerializeField] private Vector2 spawnZRange = new Vector2(-5f, 5f);

    private GameObject cachedPrefab;

    void Start()
    {
        // Cache the prefab once
        cachedPrefab = Resources.Load<GameObject>(prefabName);
        if (cachedPrefab == null)
        {
            Debug.LogError($"Prefab {prefabName} not found in Resources!");
        }
    }

    void Update()
    {
        if (cachedPrefab == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 pos = new Vector3(
                Random.Range(spawnXRange.x, spawnXRange.y),
                spawnHeight,
                Random.Range(spawnZRange.x, spawnZRange.y)
            );
            SpawnPrefab(pos);
        }
    }

    private void SpawnPrefab(Vector3 position)
    {
        GameObject g = Instantiate(cachedPrefab, position, Quaternion.identity);

        // Random uniform scale for the whole object
        float uniformScale = Random.Range(randomScaleRange.x, randomScaleRange.y);
        g.transform.localScale = Vector3.one * uniformScale;

        // Randomize colors per renderer (so each arm in the prefab can have different hues)
        var renderers = g.GetComponentsInChildren<Renderer>(includeInactive: true);
        foreach (var r in renderers)
        {
            // Bright, saturated random color
            r.material.color = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.7f, 1f);
        }

        // Ensure it has a Rigidbody to fall
        var rb = g.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = g.AddComponent<Rigidbody>();
        }
        rb.mass = Mathf.Max(0.1f, uniformScale); // simple mass scaling
        rb.angularDrag = 0.05f;
    }
}
