using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;
    Animator m_Animator;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation =
        Quaternion.identity;
    Rigidbody m_Rigidbody;

    public GameObject enemyToBonk; // stored reference to enemy 

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");

        float vertical = Input.GetAxis("Vertical");

        m_Movement.Set(horizontal, 0f, vertical);

        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);

        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);

        bool isWalking = hasHorizontalInput || hasVerticalInput;

        m_Animator.SetBool("isWalking", isWalking);

        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop();
        }

        Vector3 desiredForward =
            Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);

        m_Rotation =
            Quaternion.LookRotation(desiredForward);

        if (Input.GetKeyDown(KeyCode.LeftShift) && enemyToBonk != null)

        {
            Destroy(enemyToBonk);
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // press left shift if we have an enemy to bonk
        }


    }
    void OnAnimatorMove()
    {
        m_Rigidbody.MovePosition
            (m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        m_Rigidbody.MoveRotation(m_Rotation);
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BonkZone"))
        {
            enemyToBonk = other.gameObject.transform.parent.gameObject;
            Time.timeScale = 0.5f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("BonkZone"))
        {
            if(other.gameObject.transform.parent.gameObject == enemyToBonk)
            {
                enemyToBonk = null;
                Time.timeScale = 1.0f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }
        }
    }
}
