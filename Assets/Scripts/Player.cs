using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float speedCoefficient;
    [SerializeField] private float jumpForce;
    private Vector2 velocity;
    private bool isGrounded;
    private float xMax;
    private Rigidbody2D rigidbody2d;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    
    [SerializeField] private Text scoreText;
    private int score;
    
    public bool StarPower { get; private set; }
    
    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        xMax = Camera.main.orthographicSize * Camera.main.aspect;
    }

    private void FixedUpdate()
    {
        float inputAxis = Input.GetAxis("Horizontal");
        velocity = rigidbody2d.velocity;

        if (inputAxis < 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (inputAxis > 0)
        {
            spriteRenderer.flipX = true;
        }

        if (isGrounded)
        {
            if (inputAxis != 0)
            {
                animator.SetInteger("State", 1);
            }
            else
            {
                animator.SetInteger("State", 0);
            }
        }
        else
        {
            animator.SetInteger("State", 2);
        }
        
        if (transform.position.x < -xMax + 0.5f && inputAxis <= 0 ||
            transform.position.x > xMax - 0.5f && inputAxis >= 0)
        {
            velocity.x = 0;
        }
        else
        {
            velocity.x = inputAxis * speed;
        }
        
        rigidbody2d.velocity = velocity;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isGrounded && collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = collision.contacts.All(c => c.point.y < transform.position.y);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = !collision.contacts.All(c => c.point.y > transform.position.y);
        }
    }

    private void Jump()
    {
        rigidbody2d.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    public void AddCoin(int count)
    {
        score += count;
        scoreText.text = score.ToString();
    }

    public void StarPowerActive(float duration = 5f)
    {
        StartCoroutine(StarPowerAnimation(duration));
    }
    
    private IEnumerator StarPowerAnimation(float duration)
    {
        StarPower = true;
        speed *= speedCoefficient;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (Time.frameCount % 4 == 0)
            {
                spriteRenderer.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }
            yield return null;
            elapsed += Time.deltaTime;
        }
        
        speed /= speedCoefficient;
        spriteRenderer.color = Color.white;
        StarPower = false;
    }
}
