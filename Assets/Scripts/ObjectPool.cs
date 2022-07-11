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

    public IObjectPool<GameObject> enemyBullets;
    public GameObject enemyBullet;
    public int enemyBulletsToPool;

    public IObjectPool<GameObject> enemyBulletParticles;
    public GameObject enemyBulletParticle;
    public int enemyBulletParticlesToPool;

    private void Awake() {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
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


