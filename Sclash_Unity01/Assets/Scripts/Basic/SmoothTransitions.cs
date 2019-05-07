using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothTransitions : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SmoothMoveToLocal(GameObject objectToMove, Vector3 destination, float speed)
    {
        StopCoroutine(SmoothMoveToLocalCoroutine(objectToMove, destination, speed));
        StartCoroutine(SmoothMoveToLocalCoroutine(objectToMove, destination, speed));
    }

    public void SmoothMoveTo2D(GameObject objectToMove, Vector3 destination)
    {
        StartCoroutine(SmoothMoveTo2DCoroutine(objectToMove, destination));
    }

    IEnumerator SmoothMoveToLocalCoroutine(GameObject objectToMove, Vector3 destination, float speed)
    {
        

        Rigidbody rigidbody;
        Collider2D collider2D;
        bool
            hadCollider2D = false;
        
        if (objectToMove && objectToMove.GetComponent<Collider2D>())
        {
            collider2D = objectToMove.GetComponent<Collider2D>();
            hadCollider2D = true;
            Destroy(collider2D);
        }
        

        if (!objectToMove.GetComponent<Rigidbody>())
            rigidbody = objectToMove.AddComponent(typeof(Rigidbody)).GetComponent<Rigidbody>();
        else
            rigidbody = objectToMove.GetComponent<Rigidbody>();

        for (int debug = 0; debug < 10000; debug++)
        {
            if (rigidbody)
                rigidbody.velocity = (destination - objectToMove.transform.localPosition) * speed;
            else
                break;

            if (VectorProximity(objectToMove.transform.localPosition, destination, 1))
                break;

            yield return new WaitForSeconds(0.01f);
        }

        if (rigidbody)
        {
            rigidbody.velocity = new Vector3(0, 0);
            Destroy(rigidbody);
        }

        if (objectToMove)
            objectToMove.transform.localPosition = destination;


        if (hadCollider2D)
            collider2D = objectToMove.AddComponent(typeof(Collider2D)).GetComponent<Collider2D>();

        yield return new WaitForSeconds(0);
    }

    IEnumerator SmoothMoveTo2DCoroutine(GameObject objectToMove, Vector3 destination)
    {
        Rigidbody2D rigidbody2D;
        rigidbody2D = objectToMove.AddComponent(typeof(Rigidbody2D)).GetComponent<Rigidbody2D>();

        for (int debug = 0; debug < 10000; debug++)
        {
            rigidbody2D.velocity = destination - objectToMove.transform.position;

            if (VectorProximity(objectToMove.transform.position, destination, 1))
                break;

            yield return new WaitForSeconds(0.01f);
        }

        rigidbody2D.velocity = new Vector2(0, 0);
        objectToMove.transform.position = destination;

        yield return new WaitForSeconds(0);
    }

    bool VectorProximity(Vector3 vector1, Vector3 vector2, float proximity)
    {
        Vector3
            dif = vector1 - vector2;

        if ((Mathf.Abs(dif.x) <= proximity) && (Mathf.Abs(dif.y) <= proximity) && (Mathf.Abs(dif.z) <= proximity))
        {
            return true;
        }

        return false;
    }
}
