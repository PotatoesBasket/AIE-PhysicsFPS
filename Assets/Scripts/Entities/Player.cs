using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float pushForce = 0;
    [SerializeField] float kickForce = 0;
    [SerializeField] GameObject deathScreen = null;

    [SerializeField] ExplosiveItemController grenades = null;
    [SerializeField] ExplosiveItemController mines = null;

    public HealthController HealthController { get; private set; }
    FirstPersonController FPController;

    AudioSource audioSource = null;
    AudioClip ammoPickupSound = null;

    Vector3 startPos;
    Quaternion startRot;

    private void Awake()
    {
        HealthController = GetComponent<HealthController>();
        FPController = GetComponent<FirstPersonController>();

        audioSource = GetComponent<AudioSource>();
        ammoPickupSound = AudioManager.current.ammoPickup;

        startPos = transform.position;
        startRot = transform.rotation;
    }

    void OnDeath()
    {
        deathScreen.SetActive(true);
        GameManager.current.SetPauseState(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ammo") && audioSource != null && ammoPickupSound != null)
        {
            grenades.AddToAmount(10);
            mines.AddToAmount(5);

            audioSource.PlayOneShot(ammoPickupSound);
            other.gameObject.SetActive(false);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.rigidbody != null)
        {
            Vector3 force = (hit.transform.position - transform.position).normalized;

            if (hit.transform.CompareTag("Kickable"))
                force *= kickForce;
            else
                force *= pushForce;

            hit.rigidbody.AddForceAtPosition(force, hit.collider.ClosestPoint(transform.position));
        }

        if (hit.gameObject.TryGetComponent(out Mine mine))
            mine.Detonate();

        if (hit.gameObject.CompareTag("Death"))
        {
            FPController.ResetVelocity();
            transform.position = startPos;
            transform.rotation = startRot;
        }
    }

    private void OnEnable()
    {
        HealthController.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        HealthController.OnDeath -= OnDeath;
    }
}