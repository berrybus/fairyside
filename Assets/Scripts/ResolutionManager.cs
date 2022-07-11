using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{

  public void ToggleFullscreen() {
    bool isFullscreen = !Screen.fullScreen;
    Screen.fullScreen = !Screen.fullScreen;
    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, isFullscreen);
  }

  public void Set1080() {
    Screen.SetResolution(1920, 1080, false);
  }

  public void Set720() {
    Screen.SetResolution(1280, 720, false);
  }
}
