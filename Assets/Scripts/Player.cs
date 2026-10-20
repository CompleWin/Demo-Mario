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

    [SerializeField] private float jumpHeight = 3.5f;
    [SerializeField] private float timeToApex = 0.4f;
    private float jumpVelocity;
    
    [SerializeField] private AudioClip jumpSound;
    
    private Vector2 velocity;
    private bool isGrounded;
    private bool groundContact;
    private float xMax;
    private Rigidbody2D rigidbody2d;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource;
    
    [SerializeField] private Text scoreText;
    private int score;
    
    public bool StarPower { get; private set; }
    
    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        xMax = Camera.main.orthographicSize * Camera.main.aspect;
        
        // g = 2h / t^2
        float gravity = 2f * jumpHeight / (timeToApex * timeToApex);
        // v_0 = g * t
        jumpVelocity = gravity * timeToApex;
        rigidbody2d.gravityScale = gravity / -Physics2D.gravity.y;
    }

    private void FixedUpdate()
    {
        isGrounded = groundContact;
        groundContact = false;
        
        float inputAxis = Input.GetAxisRaw("Horizontal");
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
        {
            return;
        }

        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y > 0.5f)
            {
                groundContact = true;
                return;
            }
        }
    }

    private void Jump()
    {
        rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, jumpVelocity);
        audioSource.PlayOneShot(jumpSound);
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
