using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialObstacle : MonoBehaviour
{
    public enum MaterialType { Ice, Wood, Stone }

    [Header("Material Settings")]
    public MaterialType materialType;

    [Header("Health & Damage Settings")]
    public float maxHealth = 100f;
    public float damageThreshold;       // Will be set based on material type
    public float damageMultiplier;      // Will be set based on material type

    private float currentHealth;

    [Header("Damage Sprites")]
    public Sprite undamagedSprite;
    public Sprite damagedSprite;
    public Sprite veryDamagedSprite;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = undamagedSprite;

        // 👇 Adjust sensitivity based on material type
        switch (materialType)
        {
            case MaterialType.Ice:
                damageThreshold = 4f;
                damageMultiplier = 4f;
                break;
            case MaterialType.Wood:
                damageThreshold = 6f;
                damageMultiplier = 3f;
                break;
            case MaterialType.Stone:
                damageThreshold = 8f;
                damageMultiplier = 2.5f;
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Apply damage from any collision with sufficient force
        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce >= damageThreshold)
        {
            float damage = impactForce * damageMultiplier;
            ApplyDamage(damage);
        }
    }

    void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        UpdateSprite();

        if (currentHealth <= 0f)
        {
            AwardScore();
            Destroy(gameObject);
        }
    }

    void AwardScore()
    {
        int points = 0;

        switch (materialType)
        {
            case MaterialType.Ice:
                points = 100;
                break;
            case MaterialType.Wood:
                points = 200;
                break;
            case MaterialType.Stone:
                points = 300;
                break;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(points);
        }
    }

    void UpdateSprite()
    {
        if (currentHealth <= maxHealth * 0.3f)
        {
            spriteRenderer.sprite = veryDamagedSprite;
        }
        else if (currentHealth <= maxHealth * 0.7f)
        {
            spriteRenderer.sprite = damagedSprite;
        }
        else
        {
            spriteRenderer.sprite = undamagedSprite;
        }
    }
}
