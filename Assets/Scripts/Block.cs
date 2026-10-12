using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private Sprite brokenBlockSprite;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Vector3 itemOffset = Vector3.up;
    [SerializeField] private float centerTolerance = 0.25f;

    [SerializeField] private float itemRiseDuration = 0.5f;
    
    private SpriteRenderer _spriteRenderer;
    private bool _isUsed = false;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (var collisionContact in collision.contacts)
        {
            Debug.Log(collisionContact.point);
        }
        if (!_isUsed &&
            collision.gameObject.CompareTag("Player") && 
            collision.contacts.All(c => c.point.y < transform.position.y) &&
            Mathf.Abs(collision.transform.position.x - transform.position.x) <= centerTolerance)
        {
            Hit();
        }
    }

    private void Hit()
    {
        _isUsed = true;

        if (brokenBlockSprite != null)
        {
            _spriteRenderer.sprite = brokenBlockSprite;
        }

        if (itemPrefab != null)
        {
            StartCoroutine(SpawnItem());
        }
    }

    private IEnumerator SpawnItem()
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = transform.position + itemOffset;

        GameObject item = Instantiate(itemPrefab, startPosition, Quaternion.identity);

        Collider2D itemCollider = item.GetComponent<Collider2D>();
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }

        SpriteRenderer itemRenderer = item.GetComponent<SpriteRenderer>();
        int originalOrder = itemRenderer.sortingOrder;
        itemRenderer.sortingOrder = _spriteRenderer.sortingOrder - 1;

        float elapsed = 0f;
        while (elapsed < itemRiseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / itemRiseDuration);
            item.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
        
        itemRenderer.sortingOrder = originalOrder;
        if (itemCollider != null)
        {
            itemCollider.enabled = true;
        }
    }
}
