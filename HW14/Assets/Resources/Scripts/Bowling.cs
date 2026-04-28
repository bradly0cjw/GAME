using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowling : MonoBehaviour
{
    // Prefabs for pins (Cylinder) and ball (Sphere). Assign in Inspector.
    [Header("Prefabs")]
    public GameObject pinPrefab;
    public GameObject ballPrefab;

    [Header("Layout Settings")]
    // Distance between adjacent pins (center-to-center)
    public float pinSpacing = 0.6f;
    // Base position of the head pin (front-most pin)
    public Vector3 headPinPosition = new Vector3(0f, 0f, 5f);
    // Uniform scale for pins (applied to prefab instance)
    public Vector3 pinScale = new Vector3(0.2f, 0.5f, 0.2f);
    // Uniform scale (radius) for ball
    public float ballRadius = 0.25f;

    [Header("Ball Settings")]
    public Vector3 ballStartPosition = new Vector3(0f, 0.25f, -5f);
    public float throwForce = 8f;
    public float spinTorque = 2f;

    [Header("Runtime")]
    public List<GameObject> pins = new List<GameObject>();
    private Rigidbody ballRb;

    void Start()
    {
        SpawnPins();
        SpawnBall();
    }

    void Update()
    {
        HandleInput();
    }

    private void SpawnPins()
    {
        if (pinPrefab == null)
        {
            Debug.LogWarning("Bowling: pinPrefab is not assigned.");
            return;
        }

        // Clear previous pins if any
        foreach (var p in pins)
        {
            if (p != null) Destroy(p);
        }
        pins.Clear();

        // Bowling pin positions in a triangular formation (rows: 1-2-3-4)
        // Use equilateral triangle spacing; row depth offset = pinSpacing * sin(60deg) ~ 0.866 * pinSpacing
        float depthOffset = pinSpacing * 0.8660254f; // sin(60°)

        // Row 1 (head pin)
        Vector3 r1 = headPinPosition;
        pins.Add(InstantiatePin(r1));

        // Row 2
        Vector3 r2Left = headPinPosition + new Vector3(-pinSpacing * 0.5f, 0f, depthOffset);
        Vector3 r2Right = headPinPosition + new Vector3(pinSpacing * 0.5f, 0f, depthOffset);
        pins.Add(InstantiatePin(r2Left));
        pins.Add(InstantiatePin(r2Right));

        // Row 3
        Vector3 r3Left = headPinPosition + new Vector3(-pinSpacing, 0f, depthOffset * 2f);
        Vector3 r3Mid = headPinPosition + new Vector3(0f, 0f, depthOffset * 2f);
        Vector3 r3Right = headPinPosition + new Vector3(pinSpacing, 0f, depthOffset * 2f);
        pins.Add(InstantiatePin(r3Left));
        pins.Add(InstantiatePin(r3Mid));
        pins.Add(InstantiatePin(r3Right));

        // Row 4
        Vector3 r4L = headPinPosition + new Vector3(-pinSpacing * 1.5f, 0f, depthOffset * 3f);
        Vector3 r4L2 = headPinPosition + new Vector3(-pinSpacing * 0.5f, 0f, depthOffset * 3f);
        Vector3 r4R2 = headPinPosition + new Vector3(pinSpacing * 0.5f, 0f, depthOffset * 3f);
        Vector3 r4R = headPinPosition + new Vector3(pinSpacing * 1.5f, 0f, depthOffset * 3f);
        pins.Add(InstantiatePin(r4L));
        pins.Add(InstantiatePin(r4L2));
        pins.Add(InstantiatePin(r4R2));
        pins.Add(InstantiatePin(r4R));
    }

    private GameObject InstantiatePin(Vector3 position)
    {
        var pin = Instantiate(pinPrefab, position, Quaternion.identity);
        // Apply scale so cylinder resembles a pin roughly
        pin.transform.localScale = pinScale;

        // Ensure physics components exist
        var rb = pin.GetComponent<Rigidbody>();
        if (rb == null) rb = pin.AddComponent<Rigidbody>();
        rb.mass = 1.5f; // reasonable mass for stability
        rb.angularDrag = 0.2f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        var collider = pin.GetComponent<Collider>();
        if (collider == null)
        {
            var cyl = pin.AddComponent<CapsuleCollider>();
            cyl.height = 1f;
            cyl.radius = Mathf.Max(pinScale.x, pinScale.z) * 0.5f;
            cyl.center = Vector3.zero;
        }

        return pin;
    }

    private void SpawnBall()
    {
        if (ballPrefab == null)
        {
            Debug.LogWarning("Bowling: ballPrefab is not assigned.");
            return;
        }

        var ball = Instantiate(ballPrefab, ballStartPosition, Quaternion.identity);
        ball.transform.localScale = Vector3.one * (ballRadius * 2f); // diameter

        ballRb = ball.GetComponent<Rigidbody>();
        if (ballRb == null) ballRb = ball.AddComponent<Rigidbody>();
        ballRb.mass = 6.0f; // typical bowling ball mass is higher than pins
        ballRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        var col = ball.GetComponent<SphereCollider>();
        if (col == null) col = ball.AddComponent<SphereCollider>();
        col.radius = ballRadius;
    }
    
    //bool isShoot = false;
    private void HandleInput()
    {
        if (ballRb == null) return;

        // Press Space to throw the ball forward
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Reset position and velocity
            //if (isShoot)
            //{

            //    ballRb.velocity = Vector3.zero;
            //    ballRb.angularVelocity = Vector3.zero;
            //    ballRb.transform.position = ballStartPosition;
            //}
            //isShoot= true;

            // Apply forward impulse towards the pins (positive Z)
            ballRb.AddForce(Vector3.forward * throwForce, ForceMode.VelocityChange);
            // Add a small side spin
            ballRb.AddTorque(Vector3.right * spinTorque, ForceMode.VelocityChange);
        }
        //Move ball left/right with A/D keys before shooting
        if (Input.GetKey(KeyCode.A))
        {
            ballRb.AddForce(Vector3.left * throwForce * 0.005f, ForceMode.VelocityChange);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            ballRb.AddForce(Vector3.right * throwForce * 0.005f, ForceMode.VelocityChange);
        }


        // R resets pins and ball
        if (Input.GetKeyDown(KeyCode.R))
        {
            //isShoot= false;
            ResetScene();
        }
    }

    private void ResetScene()
    {
        SpawnPins();
        if (ballRb != null)
        {
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
            ballRb.transform.position = ballStartPosition;
        }
        else
        {
            SpawnBall();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize pin layout in editor
        Gizmos.color = Color.yellow;
        float depthOffset = pinSpacing * 0.8660254f;
        // Row 1
        Gizmos.DrawWireSphere(headPinPosition, 0.05f);
        // Row 2
        Gizmos.DrawWireSphere(headPinPosition + new Vector3(-pinSpacing * 0.5f, 0f, depthOffset), 0.05f);
        Gizmos.DrawWireSphere(headPinPosition + new Vector3(pinSpacing * 0.5f, 0f, depthOffset), 0.05f);
        // Row 3
        Gizmos.DrawWireSphere(headPinPosition + new Vector3(-pinSpacing, 0f, depthOffset * 2f), 0.05f);
        Gizmos.DrawWireSphere(headPinPosition + new Vector3(0f, 0f, depthOffset * 2f), 0.05f);
        Gizmos.DrawWireSphere(headPinPosition + new Vector3(pinSpacing, 0f, depthOffset * 2f), 0.05f);
        // Row 4
        Gizmos.DrawWireSphere(headPinPosition + new Vector3(-pinSpacing * 1.5f, 0f, depthOffset * 3f), 0.05f);
        Gizmos.DrawWireSphere(headPinPosition + new Vector3(-pinSpacing * 0.5f, 0f, depthOffset * 3f), 0.05f);
        Gizmos.DrawWireSphere(headPinPosition + new Vector3(pinSpacing * 0.5f, 0f, depthOffset * 3f), 0.05f);
        Gizmos.DrawWireSphere(headPinPosition + new Vector3(pinSpacing * 1.5f, 0f, depthOffset * 3f), 0.05f);

        // Ball start
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(ballStartPosition, ballRadius);
    }
}
