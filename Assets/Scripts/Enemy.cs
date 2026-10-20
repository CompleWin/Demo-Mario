using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private AudioClip deathSound;
    private Rigidbody2D rigidbody2d;
    private Vector2 direction = Vector2.left;

    private Player player;
    private float xMax;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        
        xMax = Camera.main.orthographicSize * Camera.main.aspect;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
        rigidbody2d.velocity = direction * speed;
    }

    private void FixedUpdate()
    {
        if (transform.position.x > xMax || transform.position.x < -xMax)
        {
            ChangeDirection(true);
        }
        rigidbody2d.velocity = new Vector2(direction.x * speed, rigidbody2d.velocity.y);
    }

    private void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            bool dead = collision.contacts.All(c => c.point.y > transform.position.y);
            if (dead || player.StarPower)
            {
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(Death(collision.gameObject));
            }
        }
        else
        {
            ChangeDirection(!collision.contacts.All(c => c.point.y < transform.position.y));
        }
    }

    private void ChangeDirection(bool makeChange)
    {
        if (makeChange)
        {
            direction = -direction;
            Vector2 velocity = rigidbody2d.velocity;
            velocity.x = direction.x * speed;
            rigidbody2d.velocity = velocity;
        }
    }
    
    private IEnumerator Death(GameObject playerObject)
    {
        var audioSource = playerObject.GetComponent<AudioSource>();

        audioSource.PlayOneShot(deathSound);

        yield return new WaitForSeconds(deathSound.length);

        SceneManager.LoadScene(0);
    }
}
