using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour {
    public static ObjectPool instance;

    public IObjectPool<GameObject>bullets;
    public GameObject bullet;
    public int playerBulletsToPool;

    public IObjectPool<GameObject> bulletParticles;
    public GameObject bulletParticle;
    public int bulletParticlesToPool;

    public IObjectPool<GameObject> enemyBullets;
    public GameObject enemyBullet;
    public int enemyBulletsToPool;

    public IObjectPool<GameObject> enemyBulletParticles;
    public GameObject enemyBulletParticle;
    public int enemyBulletParticlesToPool;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        bullets = new ObjectPool<GameObject>(
          createFunc: () => Instantiate(bullet),
          actionOnGet: (obj) => obj.SetActive(false),
          actionOnRelease: (obj) => obj.SetActive(false),
          actionOnDestroy: (obj) => Destroy(obj),
          collectionCheck: true,
          defaultCapacity: playerBulletsToPool,
          maxSize: 100
        );

        enemyBullets = new ObjectPool<GameObject>(
          createFunc: () => Instantiate(enemyBullet),
          actionOnGet: (obj) => obj.SetActive(false),
          actionOnRelease: (obj) => obj.SetActive(false),
          actionOnDestroy: (obj) => Destroy(obj),
          collectionCheck: true,
          defaultCapacity: enemyBulletsToPool,
          maxSize: 100
        );

        bulletParticles = new ObjectPool<GameObject>(
          createFunc: () =>
          {
              var particleObj = Instantiate(bulletParticle);
              particleObj.GetComponent<PooledParticle>().pool = bulletParticles;
              return particleObj;
          },
          actionOnGet: (obj) => obj.SetActive(true),
          actionOnRelease: (obj) => obj.SetActive(false),
          actionOnDestroy: (obj) => Destroy(obj),
          collectionCheck: true,
          defaultCapacity: bulletParticlesToPool,
          maxSize: 100
        );

        enemyBulletParticles = new ObjectPool<GameObject>(
          createFunc: () =>
          {
              var particleObj = Instantiate(enemyBulletParticle);
              particleObj.GetComponent<PooledParticle>().pool = enemyBulletParticles;
              return particleObj;
          },
          actionOnGet: (obj) => obj.SetActive(true),
          actionOnRelease: (obj) => obj.SetActive(false),
          actionOnDestroy: (obj) => Destroy(obj),
          collectionCheck: true,
          defaultCapacity: enemyBulletParticlesToPool,
          maxSize: 100
        );
    }
}


