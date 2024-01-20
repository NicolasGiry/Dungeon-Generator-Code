using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void OpenScene (int numScene)
    {
        SceneManager.LoadScene (numScene);
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0 && Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
