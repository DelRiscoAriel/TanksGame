using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;

    
    private string m_FireButton;         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;                


    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }
    

    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
        //The slider should have a default value of the minum lauunch force
        m_AimSlider.value = m_MinLaunchForce;

        //If the max force has been exeeded and the shell has'nt yet been launched
        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        //Otherwise if the fire button has just statted being pressed
        else if (Input.GetButtonDown (m_FireButton))
        {
            //reset hr fire flag and reset the launch force
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            //Change the clip to the charging clip and start playing it
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        //Otherwise the fire button is being held and the shell hasn't launched
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            //Increase the launch forve and update the slider
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        //Otherwise id the fire button is realesed and the shell hasn't launched
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            //Lunch the shel
            Fire();
        }
    }


    private void Fire()
    {
        // Instantiate and launch the shell.
        //Set the  fire flag so onlt fire is called once
        m_Fired = true;

        //Create an instance of the shell and store a reference to it's rigidbody
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        //Set the shell's velocity to the launch force in the fire position forward direction
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        //Change the clip to the firing clip and play it
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        //Reset the launch force. This is a precaution in case of missing button events
        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}