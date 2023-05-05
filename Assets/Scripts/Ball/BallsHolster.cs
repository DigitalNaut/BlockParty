using System;
using System.Collections.Generic;
using UnityEngine;

public class BallsHolster : MonoBehaviour
{
  List<BallProjectile> ballsList = new List<BallProjectile>();
  uint lucidBallsAllowed = 1;
  uint minLucidBallsLimit = 0;
  uint maxLucidBallsLimit = 5;

  public bool CanAddBalls => ballsList.Count < lucidBallsAllowed;
  public bool CanRemoveBalls => ballsList.Count > minLucidBallsLimit;
  public uint ChangeLucidBallsMaxLimit(uint increment = 1) => (uint)Mathf.Clamp(lucidBallsAllowed += increment, minLucidBallsLimit, maxLucidBallsLimit);
  public void SetLucidBallsMaxLimit(uint newLimit) => lucidBallsAllowed = (uint)Mathf.Clamp(newLimit, minLucidBallsLimit, maxLucidBallsLimit);

  public bool AddBall(BallProjectile ball)
  {
    if (ballsList.Count >= lucidBallsAllowed)
      return false;

    ballsList.Add(ball);
    return true;
  }

  public void RemoveBall(BallProjectile ball)
  {
    if (ballsList.Count == 0)
      return;

    ballsList.Remove(ball);
  }

  public BallProjectile GetOldestLucidBall(bool isActive = false)
  {
    if (ballsList.Count == 0) return null;

    var oldestBall = ballsList[0];
    ballsList.RemoveAt(0);

    oldestBall.gameObject.SetActive(isActive);

    return oldestBall;
  }
}
