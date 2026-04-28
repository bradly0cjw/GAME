using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NumberQuadGen : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Your student id. Each character should be a digit 0-9.")]
    [SerializeField] private string studentId = "111590453";

    [Tooltip("Texture atlas (Numbers-4.jpg)")]
    [SerializeField] private Texture2D numbersTexture;

    [Header("Layout")]
    [SerializeField] private float quadSize = 1f;
    [SerializeField] private float spacing = 0.1f;
    [SerializeField] private Vector3 startLocalPosition = Vector3.zero;

    [Header("Material")]
    [Tooltip("Optional material to use. If null, a default Standard material will be created so the quads can cast/receive shadows.")]
    [SerializeField] private Material sharedMaterial;

    [Header("Crop")]
    [Tooltip("Crop inwards on each side by this many pixels (prevents showing the border around digits).")]
    [SerializeField] private int insetPixels = 2;

    [Tooltip("Move sampled content in pixel units on X (applied to all digits). Positive moves content right.")]
    [SerializeField] private int contentPixelOffsetX = 0;

    [Tooltip("Move sampled content in pixel units on Y (applied to all digits). Positive moves content up.")]
    [SerializeField] private int contentPixelOffsetY = 0;

    [Tooltip("Extra X offset in pixels per digit (0..9). Applied in addition to Content Pixel Offset X.")]
    [SerializeField] private int[] perDigitXOffset = new int[10];

    [Tooltip("Extra Y offset in pixels per digit (0..9). Applied in addition to Content Pixel Offset Y.")]
    [SerializeField] private int[] perDigitYOffset = new int[10];

    private static readonly Dictionary<char, RectInt> DigitRects = new Dictionary<char, RectInt>
    {

        { '0', new RectInt(12,  578 - (1   + 289), 208, 289) },
        { '1', new RectInt(212, 578 - (1   + 289), 208, 289) },
        { '2', new RectInt(404, 578 - (1   + 289), 208, 289) },
        { '3', new RectInt(606, 578 - (1   + 289), 208, 289) },
        { '4', new RectInt(814, 578 - (1   + 289), 208, 289) },
        { '5', new RectInt(9,   578 - (289 + 289), 208, 289) },
        { '6', new RectInt(207, 578 - (289 + 289), 208, 289) },
        { '7', new RectInt(410, 578 - (289 + 289), 208, 289) },
        { '8', new RectInt(618, 578 - (288 + 289), 208, 289) },
        { '9', new RectInt(825, 578 - (289 + 289), 208, 289) },
    };

    private void Start()
    {
        if (numbersTexture == null)
        {
            Debug.LogError($"{nameof(NumberQuadGen)}: Please assign Numbers-4.jpg to '{nameof(numbersTexture)}'.", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(studentId))
        {
            Debug.LogError($"{nameof(NumberQuadGen)}: '{nameof(studentId)}' is empty.", this);
            return;
        }

        if (sharedMaterial == null)
        {
            var shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Unlit/Texture");
            sharedMaterial = new Material(shader);
        }

        sharedMaterial.mainTexture = numbersTexture;

        Generate();
    }

    private void Generate()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(child.gameObject);
            else
#endif
                Destroy(child.gameObject);
        }

        float step = quadSize + spacing;
        float offset = (studentId.Length - 1) * step * 0.5f;

        for (int i = 0; i < studentId.Length; i++)
        {
            char c = studentId[i];
            if (!DigitRects.TryGetValue(c, out var rect))
            {
                Debug.LogWarning($"{nameof(NumberQuadGen)}: Unsupported char '{c}' at index {i}.", this);
                continue;
            }

            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = $"Digit_{c}_{i}";
            quad.transform.SetParent(transform, false);
            quad.transform.localPosition = startLocalPosition + new Vector3(i * step - offset, 0f, 0f);
            quad.transform.localRotation = Quaternion.identity;
            quad.transform.localScale = Vector3.one * quadSize;

            var col = quad.GetComponent<Collider>();
            if (col != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(col);
                else
#endif
                    Destroy(col);
            }

            var mr = quad.GetComponent<MeshRenderer>();
            mr.sharedMaterial = sharedMaterial;
            mr.shadowCastingMode = ShadowCastingMode.On;
            mr.receiveShadows = true;

            var mf = quad.GetComponent<MeshFilter>();

            mf.mesh = Instantiate(mf.sharedMesh);

            var r = Inset(rect, insetPixels);

            int digit = c - '0';
            int extraX = (perDigitXOffset != null && perDigitXOffset.Length > digit) ? perDigitXOffset[digit] : 0;
            int extraY = (perDigitYOffset != null && perDigitYOffset.Length > digit) ? perDigitYOffset[digit] : 0;
            var totalOffset = new Vector2Int(contentPixelOffsetX + extraX, contentPixelOffsetY + extraY);

            r = OffsetRect(r, totalOffset);
            ApplyUv(mf.mesh, r, numbersTexture.width, numbersTexture.height);
        }
    }

    private static RectInt OffsetRect(RectInt r, Vector2Int offset)
    {
        return new RectInt(r.x + offset.x, r.y + offset.y, r.width, r.height);
    }

    private static RectInt Inset(RectInt r, int pixels)
    {
        if (pixels <= 0) return r;
        int x = r.x + pixels;
        int y = r.y + pixels;
        int w = Mathf.Max(1, r.width - pixels * 2);
        int h = Mathf.Max(1, r.height - pixels * 2);
        return new RectInt(x, y, w, h);
    }

    private static void ApplyUv(Mesh mesh, RectInt pixelRect, int texW, int texH)
    {
        float uMin = (float)pixelRect.xMin / texW;
        float uMax = (float)pixelRect.xMax / texW;
        float vMin = (float)pixelRect.yMin / texH;
        float vMax = (float)pixelRect.yMax / texH;

        mesh.uv = new[]
        {
            new Vector2(uMin, vMin),
            new Vector2(uMax, vMin),
            new Vector2(uMin, vMax),
            new Vector2(uMax, vMax),
        };
    }
}
