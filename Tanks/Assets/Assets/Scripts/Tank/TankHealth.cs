using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    public float m_CurrentHealth;  
    private bool m_Dead;            


    private void Awake()
    {
        //Instantiate the explotion prefab and get a reference to the particle system
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        //Get a reference to the sudio source on the intantiated prefab
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
        //Disable the prefab so it can be activated when required
        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        //When Tank is enabled the tannk's health and if it is dead
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;
        //Update the  health slider value and color
        SetHealthUI();
    }
    

    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        //Reduce current health by the damge done
        m_CurrentHealth -= amount;

        //Change the UI elemts approperialty
        SetHealthUI();

        //If the current health is at or lower then zero call Ondeath()
        if(m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
    }

    /*void Update()
    {
        SetHealthUI();

        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
    }*/

    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        //Set the slider value
        m_Slider.value = m_CurrentHealth;

        //Interpolate the color od the bar between the choosen colourus based on the current health
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        m_Dead = true;

        //Move the instantiated explotion prefab to the tank's position and turn it on
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        //Play the particle systemof the tank exploding
        m_ExplosionParticles.Play();

        //Play the tank explosion sounf effect
        m_ExplosionAudio.Play();

        //Turn the tank off
        gameObject.SetActive(false);
    }
}