using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;

    public float dashSpeedMultiplier = 2.5f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 1f;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    private bool isDashing = false;
    private bool canDash = true;
    private float dashTimeLeft;
    private float cooldownTimeLeft;

    void Start ()
    {
        m_Animator = GetComponent<Animator> ();
        m_Rigidbody = GetComponent<Rigidbody> ();
        m_AudioSource = GetComponent<AudioSource> ();
    }

    void FixedUpdate ()
    {
        float horizontal = Input.GetAxis ("Horizontal");
        float vertical = Input.GetAxis ("Vertical");

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize ();

        // Handle dash cooldown
        if (!canDash)
        {
            cooldownTimeLeft -= Time.fixedDeltaTime;
            if (cooldownTimeLeft <= 0)
            {
                canDash = true;
            }
        }

        // Check for dash input
        if (Input.GetKey(KeyCode.LeftShift) && canDash && (horizontal != 0 || vertical != 0))
        {
            isDashing = true;
            canDash = false;
            dashTimeLeft = dashDuration;
            cooldownTimeLeft = dashCooldown;
        }

        // Update dash timer
        if (isDashing)
        {
            dashTimeLeft -= Time.fixedDeltaTime;
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
            }
        }

        bool hasHorizontalInput = !Mathf.Approximately (horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately (vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool ("IsWalking", isWalking);

        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop ();
        }

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);
    }

    void OnAnimatorMove()
    {
        float speedMultiplier = isDashing ? dashSpeedMultiplier : 1f;
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude * speedMultiplier);
        m_Rigidbody.MoveRotation(m_Rotation);
    }
}
