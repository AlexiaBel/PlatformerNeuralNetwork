using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool playerControlled = false;

    public float speed;

    public float jumpForce;

    public float jumpCooldown;

    public float viewDistance;

    public bool dead = false;

    Rigidbody2D rb;

    AiBrain aiBrain;

    Vector2 velocity = Vector2.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        aiBrain = GetComponent<AiBrain>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dead) { return; }

        if (playerControlled)
        {
            velocity = Vector2.zero;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                velocity = new Vector2(-1, 0);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                velocity = new Vector2(1, 0);
            }
        }
        else
        {
            (float[], bool[]) raycasts = createRaycasts();
            AiBrain.AiDecision aiDecision = aiBrain.MakeDecision(new AiBrain.AiInput(velocity.x, jumponcooldown, transform.position, EvolutionController.flag.position.x, EvolutionController.flag.position.y, raycasts.Item1, raycasts.Item2));

            velocity.x = aiDecision.Direction;

            if (aiDecision.jumpNow) { Jump(); }
        }
    }

    private void FixedUpdate()
    {
        if (dead) { return; }

        Move(velocity);
    }

    public (float[], bool[]) createRaycasts()
    {
        List<float> dir = new List<float>();
        List<bool> isD = new List<bool>();

        if (Physics2D.Raycast(transform.position, Vector2.down, viewDistance))
        {
            dir.Add(Physics2D.Raycast(transform.position, Vector2.down, viewDistance).distance);
            isD.Add(Physics2D.Raycast(transform.position, Vector2.down, viewDistance).collider.gameObject.CompareTag("DeadZone"));
            Debug.DrawRay(transform.position, Vector2.down * viewDistance, Color.green);
        }
        else
        {
            dir.Add(-1);
            isD.Add(false);
            Debug.DrawRay(transform.position, Vector2.down * viewDistance, Color.red);
        }

        if (Physics2D.Raycast((Vector2)transform.position + new Vector2(1, 0), Vector2.down, viewDistance))
        {
            dir.Add(Physics2D.Raycast((Vector2)transform.position + new Vector2(1, 0), Vector2.down, viewDistance).distance);
            isD.Add(Physics2D.Raycast((Vector2)transform.position + new Vector2(1, 0), Vector2.down, viewDistance).collider.gameObject.CompareTag("DeadZone"));
            Debug.DrawRay((Vector2)transform.position + new Vector2(1, 0), Vector2.down * viewDistance, Color.green);
        }
        else
        {
            dir.Add(-1);
            isD.Add(false);
            Debug.DrawRay((Vector2)transform.position + new Vector2(1, 0), Vector2.down * viewDistance, Color.red);
        }

        if (Physics2D.Raycast((Vector2)transform.position + new Vector2(-1, 0), Vector2.down, viewDistance))
        {
            dir.Add(Physics2D.Raycast((Vector2)transform.position + new Vector2(-1, 0), Vector2.down, viewDistance).distance);
            isD.Add(Physics2D.Raycast((Vector2)transform.position + new Vector2(-1, 0), Vector2.down, viewDistance).collider.gameObject.CompareTag("DeadZone"));
            Debug.DrawRay((Vector2)transform.position + new Vector2(-1, 0), Vector2.down * viewDistance, Color.green);
        }
        else
        {
            dir.Add(-1);
            isD.Add(false);
            Debug.DrawRay((Vector2)transform.position + new Vector2(-1, 0), Vector2.down * viewDistance, Color.red);
        }

        if (Physics2D.Raycast(transform.position, Vector2.right, viewDistance))
        {
            dir.Add(Physics2D.Raycast(transform.position, Vector2.right, viewDistance).distance);
            isD.Add(Physics2D.Raycast(transform.position, Vector2.right, viewDistance).collider.gameObject.CompareTag("DeadZone"));
            Debug.DrawRay(transform.position, Vector2.right * viewDistance, Color.green);
        }
        else
        {
            dir.Add(-1);
            isD.Add(false);
            Debug.DrawRay(transform.position, Vector2.right * viewDistance, Color.red);
        }

        if (Physics2D.Raycast(transform.position, Vector2.left, viewDistance))
        {
            dir.Add(Physics2D.Raycast(transform.position, Vector2.left, viewDistance).distance);
            isD.Add(Physics2D.Raycast(transform.position, Vector2.left, viewDistance).collider.gameObject.CompareTag("DeadZone"));
            Debug.DrawRay(transform.position, Vector2.left * viewDistance, Color.green);
        }
        else
        {
            dir.Add(-1);
            isD.Add(false);
            Debug.DrawRay(transform.position, Vector2.left * viewDistance, Color.red);
        }

        if (Physics2D.Raycast(transform.position, Vector2.up + Vector2.right, viewDistance))
        {
            dir.Add(Physics2D.Raycast(transform.position, Vector2.up + Vector2.right, viewDistance).distance);
            isD.Add(Physics2D.Raycast(transform.position, Vector2.up + Vector2.right, viewDistance).collider.gameObject.CompareTag("DeadZone"));
            Debug.DrawRay(transform.position, (Vector2.up + Vector2.right) * viewDistance, Color.green);
        }
        else
        {
            dir.Add(-1);
            isD.Add(false);
            Debug.DrawRay(transform.position, (Vector2.up + Vector2.right) * viewDistance, Color.red);
        }

        if (Physics2D.Raycast(transform.position, Vector2.up + Vector2.left, viewDistance))
        {
            dir.Add(Physics2D.Raycast(transform.position, Vector2.up + Vector2.left, viewDistance).distance);
            isD.Add(Physics2D.Raycast(transform.position, Vector2.up + Vector2.left, viewDistance).collider.gameObject.CompareTag("DeadZone"));
            Debug.DrawRay(transform.position, (Vector2.up + Vector2.left) * viewDistance, Color.green);
        }
        else
        {
            dir.Add(-1);
            isD.Add(false);
            Debug.DrawRay(transform.position, (Vector2.up + Vector2.left) * viewDistance, Color.red);
        }

        return (dir.ToArray(), isD.ToArray());
    }

    void Move(Vector2 velocity)
    {
        rb.velocity = new Vector2(velocity.x * speed * Time.timeScale, rb.velocity.y);
    }

    void Jump()
    {
        if (jumponcooldown) { return; }

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        StartCoroutine(JumpCooldown());
    }

    public bool jumponcooldown;
    IEnumerator JumpCooldown()
    {
        jumponcooldown = true;
        yield return new WaitForSeconds(jumpCooldown);
        jumponcooldown = false;
    }
}
