using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
  public enum Type {
    left, right, top, bottom
  }

  public Wall.Type direction;
  public GameObject door;
}
