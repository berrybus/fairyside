using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour {
    public static ObjectPool instance;

    public IObjectPool<GameObject> basicBullets;
    public GameObject basicBullet;
    public int playerBulletsToPool;

    public IObjectPool<GameObject> basicBulletParticles;
    public GameObject basicBulletParticle;
    public int basicBulletParticlesToPool;

    public IObjectPool<GameObject> grassBullets;
    public GameObject grassBullet;

    public IObjectPool<GameObject> grassBulletParticles;
    public GameObject grassBulletParticle;
    public int grassBulletParticlesToPool;

    public IObjectPool<GameObject> waterBullets;
    public GameObject waterBullet;

    public IObjectPool<GameObject> waterBulletParticles;
    public GameObject waterBulletParticle;
    public int waterBulletParticlesToPool;

    public IObjectPool<GameObject> fireBullets;
    public GameObject fireBullet;

    public IObjectPool<GameObject> fireBulletParticles;
    public GameObject fireBulletParticle;
    public int fireBulletParticlesToPool;

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
        basicBullets = new ObjectPool<GameObject>(
          createFunc: () => Instantiate(basicBullet),
          actionOnGet: (obj) => obj.SetActive(false),
          actionOnRelease: (obj) => obj.SetActive(false),
          actionOnDestroy: (obj) => Destroy(obj),
          collectionCheck: true,
          defaultCapacity: playerBulletsToPool,
          maxSize: 100
        );

        grassBullets = new ObjectPool<GameObject>(
          createFunc: () => Instantiate(grassBullet),
          actionOnGet: (obj) => obj.SetActive(false),
          actionOnRelease: (obj) => obj.SetActive(false),
          actionOnDestroy: (obj) => Destroy(obj),
          collectionCheck: true,
          defaultCapacity: playerBulletsToPool,
          maxSize: 100
        );

        waterBullets = new ObjectPool<GameObject>(
          createFunc: () => Instantiate(waterBullet),
          actionOnGet: (obj) => obj.SetActive(false),
          actionOnRelease: (obj) => obj.SetActive(false),
          actionOnDestroy: (obj) => Destroy(obj),
          collectionCheck: true,
          defaultCapacity: playerBulletsToPool,
          maxSize: 100
        );

        fireBullets = new ObjectPool<GameObject>(
          createFunc: () => Instantiate(fireBullet),
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

        basicBulletParticles = new ObjectPool<GameObject>(
          createFunc: () =>
          {
              var particleObj = Instantiate(basicBulletParticle);
              particleObj.GetComponent<PooledParticle>().pool = basicBulletParticles;
              return particleObj;
          },
          actionOnGet: (obj) => obj.SetActive(true),
          actionOnRelease: (obj) => obj.SetActive(false),
          actionOnDestroy: (obj) => Destroy(obj),
          collectionCheck: true,
          defaultCapacity: basicBulletParticlesToPool,
          maxSize: 100
        );

        grassBulletParticles = new ObjectPool<GameObject>(
          createFunc: () =>
          {
              var particleObj = Instantiate(grassBulletParticle);
              particleObj.GetComponent<PooledParticle>().pool = grassBulletParticles;
              return particleObj;
          },
          actionOnGet: (obj) => obj.SetActive(true),
          actionOnRelease: (obj) => obj.SetActive(false),
          actionOnDestroy: (obj) => Destroy(obj),
          collectionCheck: true,
          defaultCapacity: grassBulletParticlesToPool,
          maxSize: 100
        );

        waterBulletParticles = new ObjectPool<GameObject>(
          createFunc: () =>
          {
              var particleObj = Instantiate(waterBulletParticle);
              particleObj.GetComponent<PooledParticle>().pool = waterBulletParticles;
              return particleObj;
          },
          actionOnGet: (obj) => obj.SetActive(true),
          actionOnRelease: (obj) => obj.SetActive(false),
          actionOnDestroy: (obj) => Destroy(obj),
          collectionCheck: true,
          defaultCapacity: waterBulletParticlesToPool,
          maxSize: 100
        );

        fireBulletParticles = new ObjectPool<GameObject>(
          createFunc: () =>
          {
              var particleObj = Instantiate(fireBulletParticle);
              particleObj.GetComponent<PooledParticle>().pool = fireBulletParticles;
              return particleObj;
          },
          actionOnGet: (obj) => obj.SetActive(true),
          actionOnRelease: (obj) => obj.SetActive(false),
          actionOnDestroy: (obj) => Destroy(obj),
          collectionCheck: true,
          defaultCapacity: fireBulletParticlesToPool,
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


