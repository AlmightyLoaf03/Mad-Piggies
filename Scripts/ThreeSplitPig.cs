using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeSplitPig : Bird
{
    public GameObject pigClonePrefab;
    public GameObject explosionEffect;
    public float splitSpreadAngle = 20f; // degrees
    public float splitSpeedMultiplier = 1f; // speed factor for split pigs
    public float followDuration = 3f;       // how long camera follows middle pig before return

    private bool hasSplit = false;

    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(0) && HasLaunched && !hasSplit)
        {
            Split();
        }
    }

    void Split()
    {
        if (hasSplit) return;
        hasSplit = true;

        if (explosionEffect)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 originalVelocity = rb.velocity;

        // Spawn middle pig first and save reference to it
        GameObject middlePig = SpawnSplitPig(0f, originalVelocity);

        // Spawn top and bottom pigs
        SpawnSplitPig(+splitSpreadAngle, originalVelocity);
        SpawnSplitPig(-splitSpreadAngle, originalVelocity);

        Launcher launcher = FindObjectOfType<Launcher>();
        if (launcher != null)
        {
            launcher.FollowTargetThenReturn(middlePig.transform, followDuration);
        }

        Destroy(gameObject);
    }

    GameObject SpawnSplitPig(float angleOffset, Vector2 originalVelocity)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, angleOffset);
        Vector2 newDirection = rotation * originalVelocity.normalized;

        GameObject pig = Instantiate(pigClonePrefab, transform.position, Quaternion.identity);
        Rigidbody2D pigRb = pig.GetComponent<Rigidbody2D>();
        pigRb.isKinematic = false;
        pigRb.velocity = newDirection * originalVelocity.magnitude * splitSpeedMultiplier;

        Bird pigScript = pig.GetComponent<Bird>();
        pigScript.Launch(pigRb.velocity);

        return pig;
    }

    IEnumerator FollowMiddlePigThenReturn(GameObject middlePig)
    {
        // Tell launcher to follow middle pig immediately
        NotifyFollowTarget(middlePig.transform);

        // Wait for the desired follow duration
        yield return new WaitForSeconds(followDuration);

        // Then notify launcher that this bird is finished (to go back to launcher)
        NotifyFinished();
    }

    // Send follow message to Launcher or camera controller
    void NotifyFollowTarget(Transform target)
    {
        // You may want to get a reference to your launcher or camera controller here.
        // For example, if Launcher is singleton or find it by tag:
        Launcher launcher = FindObjectOfType<Launcher>();
        if (launcher != null)
        {
            launcher.FollowTarget(target);
        }
    }
}
