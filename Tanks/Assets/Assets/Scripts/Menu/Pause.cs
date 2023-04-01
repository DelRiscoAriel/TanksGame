using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject[] menu;
    public GameObject pause;
    public GameObject resume;

    // Start is called before the first frame update
    void Start()
    {
        resume.SetActive(false);
        foreach (GameObject obj in menu)
        {
            obj.SetActive(false);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pause.SetActive(false);
        resume.SetActive(true);
        foreach (GameObject obj in menu)
        {
            obj.SetActive(true);
        }
    }

    public void KeepPlaying()
    {
        Time.timeScale = 1;
        pause.SetActive(true);
        resume.SetActive(false);
        foreach (GameObject obj in menu)
        {
            obj.SetActive(false);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
