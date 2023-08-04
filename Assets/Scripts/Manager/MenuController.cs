using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Function to load scene by name.
    public void LoadScene(string name) => SceneManager.LoadScene(name);

    // Function to quit the game.
    public void Quit() => Application.Quit();
}
