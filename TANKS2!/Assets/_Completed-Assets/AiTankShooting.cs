using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class AiTankShooting : MonoBehaviour
    {
        public Rigidbody m_Shell;                   // Prefab of the shell
        public Transform m_FireTransform;           // Where shells are spawned
        public float m_LaunchForce = 20f;          // Shell launch force
        public float m_FireRate = 0.5f;            // Time between shots

        private float m_NextFireTime;              // When AI can fire next
        private AudioSource m_ShootingAudio;       // Reference to audio source
        private bool m_CanFire = true;             // Flag to control firing

        private void Start()
        {
            m_ShootingAudio = GetComponent<AudioSource>();
        }

        public void Fire()
        {
            if (!m_CanFire || Time.time < m_NextFireTime)
                return;

            // Create shell instance
            Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation);

            // Set velocity
            shellInstance.velocity = m_LaunchForce * m_FireTransform.forward;

            // Play audio
            if (m_ShootingAudio != null)
                m_ShootingAudio.Play();

            // Set next fire time
            m_NextFireTime = Time.time + m_FireRate;
        }

        public void EnableFiring()
        {
            m_CanFire = true;
        }

        public void DisableFiring()
        {
            m_CanFire = false;
        }
    }
}
