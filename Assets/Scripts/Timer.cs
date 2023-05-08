using System;
using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
  public void Set(float time, Action<Timer> callback) => StartCoroutine(Countdown(time, callback));

  IEnumerator Countdown(float delay, Action<Timer> callback)
  {
    yield return new WaitForSeconds(delay);

    callback(this);
  }
}
