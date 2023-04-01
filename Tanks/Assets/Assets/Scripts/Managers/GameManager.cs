using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;              
    public GameObject m_TankPrefab;         
    public TankManager[] m_Tanks;           


    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;

    //Timer
    public int timeLimit = 60;
    public int timeLeft = 60;
    public Text time;

    private void Start()
    {
        //Create the delays so they only have to be made once
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();

        //Once the tanks have been created the camara eill use them as targets
        StartCoroutine(GameLoop());
    }

    void Update()
    {
        time.text = ("Time Left: " + timeLeft);
    }

    private void SpawnAllTanks()
    {
        //For all tanks
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            //create them, set their player number and references needed for control
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        //Create a collection of transforms the same size as the number of tanks
        Transform[] targets = new Transform[m_Tanks.Length];

        //For each transform
        for (int i = 0; i < targets.Length; i++)
        {
            //Set it to the apropiate tank transform
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        //These are the targets the camara should follow
        m_CameraControl.m_Targets = targets;
    }

    //This is call from the start and will run each phase ofthe game
    private IEnumerator GameLoop()
    {
        //Satrt off by running RoundStarting() but don't return unitl it's finished
        yield return StartCoroutine(RoundStarting());
        //Once RoundStarting() has finish run RoundPlaying() but don't return unitl it's finished
        yield return StartCoroutine(RoundPlaying());
        //Once execution has returned here, run RoundEnding() but don't return unitl it's finished
        yield return StartCoroutine(RoundEnding());

        //This code is not run until RoundEnding() has ended, check is a game winner has been found
        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //Application.LoadLevel(Application.loadLevel);
        }
        else
        {
            //If there isn't a winner restart the coroutine
            //Note that this coroutine doesn't yield. This means that the current version of the GameLoop will end
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        //As soon as the round starts reset the tanks and make sure they can not move
        ResetAllTanks();
        DisableTankControl();
        
        //Reset Time Limit
        timeLeft = timeLimit;

        //Snap the camera's zoom and position to something apropiate for the tanks
        m_CameraControl.SetStartPositionAndSize();

        //Increment the round number and display text showing the players ehat round it is
        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        //Wait for the specified lenth of time until yielding control back to the GameLoop
        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        StartCoroutine("LoseTime");
        //As soon as the round begins playing let the players control o the tanks
        EnableTankControl();

        //Clear text from screen
        m_MessageText.text = string.Empty;

        //While there is not one tank left
        while (!OneTankLeft())
        {
            time.text = ("Time Left: " + timeLeft);
            //return the next frame
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        StopCoroutine("LoseTime");
        //Stop tanks from moving
        DisableTankControl();

        //Vlear the winner from the previous round
        m_RoundWinner = null;

        //See if there is a winner noe that round is over
        m_RoundWinner = GetRoundWinner();

        //If there is winner increment their score
        if (m_RoundWinner != null)
            m_RoundWinner.m_Wins++;

        //Now that the winner score has been increased, see if some has won the game
        m_GameWinner = GetGameWinner();

        //Get a messege based on the scores and whether or not there is a game winner
        string message = EndMessage();
        m_MessageText.text = message;

        //Wait for the specified lenth of time until yielding control back to the GameLoop
        yield return m_EndWait;
    }

    IEnumerator LoseTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            timeLeft--;
        }
    }

    private bool OneTankLeft()
    {
        //Start the count of tanks left at zero
        int numTanksLeft = 0;

        //Go through all the tanks
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            //if they are active incease the counter
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        if(timeLeft <= 0)
        {
            return true;
        }
        //If there are one or fewer tanks remaining return true, otherwise retunr false
        return numTanksLeft <= 1;
    }

    //This function is to find out if there is a winner of the round
    //This function is called with the assumption that 1 or fewer tanks are currently active
    private TankManager GetRoundWinner()
    {
        int left = 0;
        int tankLeft = 0;

        //Go through the tanks
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            //if one of them is active it is the winner so return it
            if (m_Tanks[i].m_Instance.activeSelf)
            {
                left++;
                tankLeft = i;
            }
                //return m_Tanks[i];
        }

        if (left > 1)
            return null;
        else
            return m_Tanks[tankLeft]; ;
        //If none of the tanks are active it is a draw
        //return null;
    }


    private TankManager GetGameWinner()
    {
        //Go through the tanks
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            //if one of them has enough rounds to win the game, return it
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        //If no tanks have enough rounds won return null
        return null;
    }

    //Return a sting message to dislay at the end of each round
    private string EndMessage()
    {
        //By default when a round ends ther are no winners
        string message = "DRAW!";

        //If there is a winner change the message
        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        //Add some line breaks
        message += "\n\n\n\n";

        //Go through the tanks and add each of their scores
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        //If there is a game winner
        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }

    //This function is used to reset all the tanks to ther propertirs nd positions 
    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}