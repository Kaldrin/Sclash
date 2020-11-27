using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
    private GameObject child;
    private Rigidbody rb;

    void Start()
    {
        child = new GameObject("Collider"/*, typeof(BoxCollider), typeof(Rigidbody)*/);
        child.transform.position = transform.position;
        transform.parent = child.transform;
    }

    private void Update()
    {
        HandleChildPosition();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(other.collider, other.otherCollider, true);
            rb = child.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.AddForce(new Vector3(0, 0, Mathf.Sign(Random.Range(-10f, 10f)) * 3f), ForceMode.Impulse);
        }
    }

    public void HandleChildPosition()
    {
        if (child.transform.position.z < -2.5f || child.transform.position.z > 2.5f)
            rb.useGravity = true;


        if (child.transform.position.y < -5f)
            Destroy(child);
    }

    public void HandleGravity()
    {

    }
}
