using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 rate;
    public bool useFixed = false;

    new Rigidbody rigidbody;

    public void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!useFixed)
            Rotate();
    }

    void FixedUpdate()
    {
        if (useFixed)
            Rotate();
    }

    void Rotate()
    {
        if (rigidbody)
            rigidbody.angularVelocity = rate * Time.deltaTime;
        else
            transform.Rotate(rate * Time.deltaTime, Space.Self);
    }
}
