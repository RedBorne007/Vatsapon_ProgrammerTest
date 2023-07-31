using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Tooltip("Transform that contains all HUDs")]
    [SerializeField] private Transform hudTransform;

    [SerializeField] private GameObject pauseScreen;

    public Transform HUDTransform => hudTransform;
    public GameObject PauseScreen => pauseScreen;
}
