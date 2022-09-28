using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BulletParticle : MonoBehaviour {
    public ParticleSystem ps;

    private void Awake() {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnEnable() {
        var main = ps.main;
        main.startColor = PlayerManager.instance.ParticleColor();
        ps.Play();
    }
}
