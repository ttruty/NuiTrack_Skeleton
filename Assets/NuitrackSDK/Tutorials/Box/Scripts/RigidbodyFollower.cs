using UnityEngine;

public class RigidbodyFollower : MonoBehaviour
{
    [SerializeField] Transform target;

    Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rigidbody.MovePosition(target.position);
        rigidbody.MoveRotation(target.rotation);
    }
}