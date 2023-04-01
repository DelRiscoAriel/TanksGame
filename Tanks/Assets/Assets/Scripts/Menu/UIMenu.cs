using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMenu : MonoBehaviour
{
    public GameObject[] menu;
    public GameObject[] playerSelect;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject obj in playerSelect)
        {
            obj.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        foreach (GameObject obj in menu)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in playerSelect)
        {
            obj.SetActive(true);
        }
    }

    public void Controls()
    {
        SceneManager.LoadScene("Controls");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void TwoPlayer()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ThreePlayer()
    {
        SceneManager.LoadScene("3Players");
    }

    public void Back()
    {
        foreach (GameObject obj in menu)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in playerSelect)
        {
            obj.SetActive(false);
        }
    }
}
