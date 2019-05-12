using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Orientation
    float initialXScale;

    //Movements
    [SerializeField]
    [Range(1f, 20f)]
    float movementsMultiplier = 10f;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Range(1f, 10f)]
    public float jumpHeight;

    bool jumpRequest;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Awake()
    {
        initialXScale = transform.localScale.x;
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(ExecOnAwake());
    }

    // Update is called once per frame
    void Update()
    {

        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * movementsMultiplier, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            jumpRequest = true;
        }
    }

    void FixedUpdate()
    {
        if (jumpRequest)
        {
            rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
            jumpRequest = false;
        }

        if (!GetComponent<PlayerAttack>().isDashing)
        {
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = lowJumpMultiplier;
            }
            else
            {
                rb.gravityScale = 1f;
            }
        }
    }

    IEnumerator ExecOnAwake()
    {
        yield return new WaitForSeconds(0.2f);
        OrientTowardsEnemy();
    }

    void OrientTowardsEnemy()
    {
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(players.Length);
        //Sets itself inactive in order not to find itself but the other one

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != gameObject)
            {
                player = players[i];
            }
        }

        float sign = transform.position.x - player.transform.position.x;
        sign = Mathf.Sign(sign);
        Debug.Log(gameObject);
        Debug.Log(sign);

        if (player.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(initialXScale * sign, transform.localScale.y, transform.localScale.z);
        }
    }
}
