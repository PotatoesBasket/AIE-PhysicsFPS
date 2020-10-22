using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExplosiveItem : MonoBehaviour
{
    protected Rigidbody body;

    [SerializeField] float explosionRange = 0;
    [SerializeField] float explosionForce = 0;
    [SerializeField] float damagePoints = 0;

    // effects
    [SerializeField] GameObject effectsObject = null; // for unparenting on detonation to allow effects to play when item is disabled
    AudioSource audioSource;
    AudioClip explosionSound;
    ParticleSystem explosionParticles;

    void Awake()
    {
        gameObject.SetActive(false);

        body = GetComponent<Rigidbody>();

        audioSource = GetComponentInChildren<AudioSource>();
        explosionSound = AudioManager.current.grenadeExplosion;

        explosionParticles = GetComponentInChildren<ParticleSystem>();
    }

    public abstract bool Activate();
    protected abstract void OnReset();

    public void ResetObject(Vector3 position, Quaternion rotation) // reset used object ready to use again in pool
    {
        OnReset();

        // reset effects object
        effectsObject.transform.parent = gameObject.transform;
        effectsObject.transform.position = gameObject.transform.position;

        // reset main object
        body.isKinematic = true;
        transform.position = position;
        transform.rotation = rotation;
        body.isKinematic = false;
    }

    public void Detonate()
    {
        PlayExplosionEffects();

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRange);
        
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(damagePoints);

            if (hit.TryGetComponent(out Rigidbody body))
                body.AddExplosionForce(explosionForce, transform.position, explosionRange);
        }

        gameObject.SetActive(false);
    }

    void PlayExplosionEffects()
    {
        if (effectsObject == null)
            return;

        effectsObject.transform.parent = transform.parent; // move effects object to where item is

        if (audioSource != null & explosionSound != null)
            audioSource.PlayOneShot(explosionSound);

        if (explosionParticles != null)
            explosionParticles.Play();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, explosionRange);
    }
}