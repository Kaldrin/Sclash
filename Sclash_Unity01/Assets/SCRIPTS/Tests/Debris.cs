using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
    private GameObject child;
    private Rigidbody rb;

    void Awake()
    {
        child = new GameObject("Debris");
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
            if (rb == null)
            {
                rb = transform.parent.gameObject.AddComponent<Rigidbody>();
            }
            rb.useGravity = false;
            rb.AddForce(new Vector3(0, 0, Mathf.Sign(Random.Range(-10f, 10f)) * 3f), ForceMode.Impulse);
        }
        else
        {
            Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
            rb2d.angularVelocity = 0;
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
