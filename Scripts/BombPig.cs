using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPig : Bird
{
    public GameObject explosionEffect;
    public float explosionRadius = 3f;
    public float explosionForce = 500f;
    private bool hasExploded = false;

    protected override void Update()
    {
        base.Update();

        // Only allow explode if bird is flying and not yet exploded
        if (Input.GetMouseButtonDown(0) && !hasExploded && HasLaunched)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        if (explosionEffect)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = rb.position - (Vector2)transform.position;
                rb.AddForce(direction.normalized * explosionForce);
            }
        }

        GameObject handler = new GameObject("BombPig_NotifyHandler");
        handler.AddComponent<NotifyDelay>().Init(1f, NotifyFinished);

        Destroy(gameObject);
    }
}
