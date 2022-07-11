using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PooledParticle : MonoBehaviour
{
  public ParticleSystem system;
  public IObjectPool<GameObject> pool;

  void Start() {
    system = GetComponent<ParticleSystem>();
    var main = system.main;
    main.stopAction = ParticleSystemStopAction.Callback;
  }

  void OnParticleSystemStopped() {
    // Return to the pool
    pool.Release(gameObject);
  }
}
