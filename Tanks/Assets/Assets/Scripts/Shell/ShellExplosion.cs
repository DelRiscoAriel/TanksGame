using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        //Collect all the colliders in a sphere from the shell's current position to a radius od the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        //Go through all the colliders
        for (int i = 0; i < colliders.Length; i++)
        {
            // find the Rigidbody
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            //If they don't have a rigidbody, go on to the next collider
            if (!targetRigidbody)
                continue;

            //Add an explotion force
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            //Find the TankHealth script associated with the rigidody
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            //If there is no TankHealth continue
            if (!targetHealth)
                continue;

            //Calculate the amount of damage the target should take based on it's distance from shell
            float damage = CalculateDamage(targetRigidbody.position);

            //Deal this damge to the tank
            targetHealth.TakeDamage(damage);
            
        }
        
        //Unparent the particle system
        m_ExplosionParticles.transform.parent = null;

        //Play particle system
        
        m_ExplosionParticles.Play();

        //Play the audio
        m_ExplosionAudio.Play();

        //Once the particle has finish destroy the gameOnject they are on
        //Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);

        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        //Create a vector from the shell to the target
        Vector3 explotionToTarget = targetPosition - transform.position;

        //Calculate the distance from the shell to the target
        float explotionDistance = explotionToTarget.magnitude;

        //Calculate the proportion of the maximun distance
        float relativeDistance = (m_ExplosionRadius - explotionDistance) / m_ExplosionRadius;

        //Calculate damage as this proportion of the maximum posible damge
        float damage = relativeDistance * m_MaxDamage;

        //Make sure the minimum damge is 0
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}