using UnityEngine;

public class CircleMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 1f;      // angular speed (rad/sec)
    public float radius = 0.5f;   // circle radius

    [Header("Model Offset")]
    public float rotationOffsetY = 180f; // if model faces wrong direction around Y

    private float angle = 0f;
    private float fixedX;         // store initial local X rotation (pitch) to keep it constant
    private float initialLocalY;  // preserve small local Y position (height)

    void Start()
    {
        // store the X euler we want to keep (Unity returns 0..360)
        fixedX = transform.localEulerAngles.x;
        initialLocalY = transform.localPosition.y;
    }

    void Update()
    {
        // advance angle
        angle += speed * Time.deltaTime;

        // compute new XZ position on circle
        float px = Mathf.Sin(angle) * radius;
        float pz = Mathf.Cos(angle) * radius;

        // maintain the object's original local Y (e.g. 0.001)
        transform.localPosition = new Vector3(px, initialLocalY, pz);

        // tangent (direction of motion) for that parameterization
        Vector3 tangent = new Vector3(Mathf.Cos(angle), 0f, -Mathf.Sin(angle));

        // if tangent is tiny (shouldn't be), skip rotation
        if (tangent.sqrMagnitude < 1e-6f) return;

        // get a rotation that looks along the tangent
        Quaternion lookRot = Quaternion.LookRotation(tangent, Vector3.up);

        // extract Y and Z from that rotation, keep X as fixedX
        Vector3 e = lookRot.eulerAngles;
        e.x = fixedX;                 // preserve original pitch (e.g. -90 -> stored as 270)
        e.y += rotationOffsetY;       // optional adjustment if model faces differently around Y

        // apply local rotation using Euler composed from fixed X and computed Y,Z
        transform.localRotation = Quaternion.Euler(e);
    }
}
