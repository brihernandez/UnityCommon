using UnityEngine;

public class Mover : MonoBehaviour
{
    public Vector3 move;
    public bool useFixed = false;

    new Rigidbody rigidbody;

    public void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (useFixed)
            MoveObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (!useFixed)
            MoveObject();
    }

    void MoveObject()
    {
        if (rigidbody)
            rigidbody.velocity = transform.TransformDirection(move);
        else
            transform.Translate(move * Time.deltaTime, Space.Self);
    }
}