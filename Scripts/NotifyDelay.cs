using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NotifyDelay : MonoBehaviour
{
    private Action callback;
    public void Init(float delay, Action callback)
    {
        this.callback = callback;
        StartCoroutine(WaitAndNotify(delay));
    }

    private IEnumerator WaitAndNotify(float delay)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
        Destroy(gameObject); // Clean up handler
    }
}
