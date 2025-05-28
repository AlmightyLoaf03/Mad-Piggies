using System.Collections;
using UnityEngine;

public class SpeedBird : Bird
{
    public float speedBoostMultiplier = 2f;
    private bool hasBoosted = false;
    private Rigidbody2D _rb;

    [Header("Boost Effects")]
    public GameObject speedBoostEffectPrefab;
    public GameObject smokeTrailPrefab;
    private GameObject activeSmokeTrail;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    protected override void Update()
    {
        base.Update();

        if (!_rb || hasBoosted) return;

        if (Input.GetMouseButtonDown(0))
        {
            Boost();
        }
    }

    private void Boost()
    {
        if (_rb.velocity.magnitude > 0.1f)
        {
            _rb.AddForce(_rb.velocity.normalized * speedBoostMultiplier, ForceMode2D.Impulse);
            hasBoosted = true;

            if (speedBoostEffectPrefab)
            {
                GameObject effect = Instantiate(speedBoostEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }

            if (smokeTrailPrefab)
            {
                GameObject trail = Instantiate(smokeTrailPrefab, transform.position, Quaternion.identity);
                trail.transform.SetParent(transform);
                trail.transform.localPosition = Vector3.zero;

                ParticleSystem ps = trail.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();
                    StartCoroutine(StopTrailAfterDelay(ps, 1.5f));
                }

                Destroy(trail, 3f);
            }
        }
    }

    private void OnDestroy()
    {
        if (activeSmokeTrail != null)
            Destroy(activeSmokeTrail);
    }

    private IEnumerator StopTrailAfterDelay(ParticleSystem ps, float delay)
    {
        yield return new WaitForSeconds(delay);
        var emission = ps.emission;
        emission.enabled = false;
    }
}
