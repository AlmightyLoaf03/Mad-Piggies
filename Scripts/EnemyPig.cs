using UnityEngine;

public class EnemyPig : MonoBehaviour
{
    public float health = 50f;
    public GameObject deathEffect;
    private bool isDead = false;
    public int scorePerKill = 300;

    void Start()
    {
        LevelManager.Instance.RegisterEnemy();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce > 2f)
        {
            TakeDamage(impactForce * 5f);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        ScoreManager.Instance.AddScore(scorePerKill);
        LevelManager.Instance.EnemyDefeated();
        gameObject.SetActive(false);
    }
}
