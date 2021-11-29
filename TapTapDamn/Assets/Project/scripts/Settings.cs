using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [Tooltip("Максимальная частота кадров")]
  [SerializeField]  private int FPS = 30;
    private void Awake()
    {
        Application.targetFrameRate = FPS;
    }

}
