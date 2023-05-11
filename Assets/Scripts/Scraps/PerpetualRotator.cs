﻿using System;
using NaughtyAttributes;
using UnityEngine;

[DisallowMultipleComponent]
public class PerpetualRotator : MonoBehaviour
{
  [Header("Settings")]
  [SerializeField][OnValueChanged("SetTurnStrategy")] bool isGlobal = true;
  [SerializeField] float TurnSpeed = 5f;
  [SerializeField] Vector3 TurnAxis = Vector3.forward;
  [SerializeField] bool RandomAngleOnStart = true;

  Action TurnStrategy;

  void Start()
  {
    if (RandomAngleOnStart) SetRandomAngle();
    SetTurnStrategy();
  }

  void OnEnable() => SetTurnStrategy();
  void OnDisable() => TurnStrategy = null;

  void Update() => TurnStrategy?.Invoke();

  void SetTurnStrategy() => TurnStrategy = isGlobal ? TurnGlobally : TurnLocally;

  Quaternion GetRotation() => Quaternion.AngleAxis(TurnSpeed * Time.deltaTime, TurnAxis);

  void TurnGlobally() => transform.rotation = GetRotation() * transform.rotation;

  void TurnLocally() => transform.localRotation = GetRotation() * transform.localRotation;

  void SetRandomAngle() => transform.rotation = Quaternion.Euler(TurnAxis * UnityEngine.Random.Range(0f, 360f));
}
