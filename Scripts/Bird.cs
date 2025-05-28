using UnityEngine;
using System;
using System.Collections;

public class Bird : MonoBehaviour
{
    public event Action<Bird> OnBirdStopped; // Event for launcher

    private Rigidbody2D _rb;
    private bool _launched = false;
    public bool HasLaunched => _launched; // Small flag to track when the bird is actually launched

    private float _stopThreshold = 0.2f;   // Velocity magnitude threshold to consider bird stopped
    private float _checkTime = 1f;         // Time the bird's velocity must stay below threshold before triggering return
    private float _timer = 0f;              // Timer for velocity check

    [Header("Despawn Settings")]
    public float despawnDelay = 2f;         // Seconds after stopping before disappearing

    [Header("Timing")]
    public float returnToLauncherDelay = 1.5f; // Uniform delay before notifying launcher to return camera and proceed

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 force)
    {
        if (_launched) return;

        _launched = true;
        _rb.isKinematic = false;
        _rb.AddForce(force, ForceMode2D.Impulse);

        StartCoroutine(ForceFadeAfterTime(10f)); // Failsafe timeout
    }

    protected virtual void Update()
    {
        if (!_launched) return;

        if (_rb.velocity.magnitude < _stopThreshold)
        {
            _timer += Time.deltaTime;
            if (_timer > _checkTime)
            {
                StartCoroutine(NotifyAndFadeAfterDelay(returnToLauncherDelay));
                enabled = false;
            }
        }
        else
        {
            _timer = 0f; // Reset timer if bird moves again
        }
    }

    protected void NotifyFinished()
    {
        OnBirdStopped?.Invoke(this);
    }

    protected IEnumerator NotifyAndFadeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NotifyFinished();
        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        float fadeDuration = despawnDelay;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator ForceFadeAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (this != null && gameObject.activeSelf) // Double check it's still alive
        {
            NotifyFinished();
            StartCoroutine(FadeAndDestroy());
        }
    }
}
