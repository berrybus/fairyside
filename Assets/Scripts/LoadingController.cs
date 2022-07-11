using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
  private AsyncOperation loading;
    void Start() {
      StartCoroutine(LoadScene());
    }

  //void FixedUpdate() {
  //  if (loading.progress >= 0.9f) {
  //    loading.allowSceneActivation = true;
  //    print("done loading!");
  //  } else {
  //    print("not done loading!");
  //  }

  //print("updating");
  //}


  IEnumerator LoadScene() {
    loading = SceneManager.LoadSceneAsync(SceneSwitcher.currentLoadingScene);
    while (!loading.isDone) {
      yield return null;
    }
    yield return new WaitForEndOfFrame();
  }
}
