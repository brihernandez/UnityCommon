using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public float height = 6.0f;
    public float distance = 20.0f;
    public float mouseSensitivity = 10.0f;
    public float scrollSpeed = 20.0f;

    public bool invertVertical = true;
    public bool maintainHorizon = true;

    [Tooltip("Target to orbit around. Defaults to whatever the object is parented to at game start if not specified.")]
    public Transform orbitTarget;

    Camera cam;

    float orbitAzimuth, orbitElevation;

    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        if (transform.parent != null)
        {
            orbitTarget = transform.parent;
            transform.SetParent(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cam != null)
        {
            // Scroll camera in and out using mousewheel. Scroll faster with distance.
            distance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * (distance * 0.02f);
            distance = Mathf.Clamp(distance, 1.0f, float.MaxValue);

            // Offset the camera from the reference point.
            cam.transform.localPosition = new Vector3(0.0f, height, -distance);

            // Follow the orbit target.
            if (orbitTarget != null)
            {
                transform.position = orbitTarget.position;
            }


            if (maintainHorizon)
            {
                // Rotate using manually tracked euler angles.
                orbitAzimuth += Input.GetAxis("Mouse X") * mouseSensitivity;
                orbitElevation += Input.GetAxis("Mouse Y") * mouseSensitivity * ((invertVertical) ? -1.0f : 1.0f);
                orbitElevation = Mathf.Clamp(orbitElevation, -70.0f, 70.0f);

                transform.rotation = Quaternion.Euler(new Vector3(orbitElevation, orbitAzimuth, 0.0f));
            }
            else
            {
                // Just let the engine handle the rotations completely on its own.
                float vertical = Input.GetAxis("Mouse Y");
                vertical *= (invertVertical) ? -1.0f : 1.0f;
                transform.Rotate(new Vector3(vertical, Input.GetAxis("Mouse X"), 0.0f) * mouseSensitivity, Space.Self);
            }
        }
    }
}
