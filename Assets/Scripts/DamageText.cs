using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour {
    public TMP_Text damage;
    // Start is called before the first frame update
    void Start() {
        StartCoroutine(RemoveSelfAfterTimePeriod());
    }

    // Update is called once per frame
    void FixedUpdate() {
        var color = damage.color;
        color.a -= 2.0f * Time.deltaTime;
        damage.color = color;
        gameObject.transform.position += new Vector3(0, 4.0f, 0) * Time.deltaTime;

    }

    IEnumerator RemoveSelfAfterTimePeriod() {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
