using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneSwitcher : MonoBehaviour
{
  public static string currentLoadingScene;

  public void GoToSchool(InputAction.CallbackContext ctx) {
    if (ctx.action.triggered) {
      currentLoadingScene = "School1";
      SceneManager.LoadScene("Loading");
    }
  }
}
