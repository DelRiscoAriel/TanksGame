using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color m_PlayerColor;            
    public Transform m_SpawnPoint;         
    [HideInInspector] public int m_PlayerNumber;             
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_Instance;          
    [HideInInspector] public int m_Wins;                     


    private TankMovement m_Movement;       
    private TankShooting m_Shooting;
    private GameObject m_CanvasGameObject;


    public void Setup()
    {
        //Get refererence to the components
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        //Set the player numbers to be consistent acrosss the scripts
        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        //Create a string using the correct color that says "PLAYER 1" etc based on the tank's color and the player's number
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        //Get all of the renderers of the tank
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        //Go through all the  renderers
        for (int i = 0; i < renderers.Length; i++)
        {
            //Set the material color to the color of the tank
            renderers[i].material.color = m_PlayerColor;
        }
    }

    //Used during  the phases of the game where the player should not be able to control their tanks 
    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }

    //Used during  the phases of the game where the player should be able to control their tanks
    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }

    //Used at the start of each round to put the tank into it's deafoult state
    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
