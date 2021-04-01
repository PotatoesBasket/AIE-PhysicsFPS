using UnityEngine;
using UnityEngine.UI;

public class FirearmController : MonoBehaviour
{
    [SerializeField] float shotForce = 0;
    [SerializeField] float damage = 0;
    [SerializeField] float shotRange = 0;
    [SerializeField] LayerMask layerMask = 0;
    [SerializeField] Image reticle = null;

    AudioSource audioSource;
    RandomizedSound shotSound;
    ParticleSystem muzzleFlash;

    bool reticleHighlighted = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        shotSound = AudioManager.current.assaultRifleShots;
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        UpdateReticle();
    }

    // Checks if a damageable object (ie. any solid collider in enemy or object layers) is being targeted and updates UI reticle color accordingly
    void UpdateReticle()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, shotRange, layerMask, QueryTriggerInteraction.Ignore)
            && (hit.transform.gameObject.layer == 10 || hit.transform.gameObject.layer == 11))
        {
            if (!reticleHighlighted && reticle != null)
            {
                reticle.color = Color.red;
                reticleHighlighted = true;
            }
        }
        else if (reticleHighlighted)
        {
            reticle.color = Color.white;
            reticleHighlighted = false;
        }
    }

    public void Shoot()
    {
        if (audioSource != null && shotSound != null)
            audioSource.PlayOneShot(shotSound.GetRandomizedClip());

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, shotRange, layerMask))
        {
            IDamageable damageable = hit.transform.GetComponentInParent<IDamageable>();

            if (damageable != null)
                damageable.TakeDamage(damage);

            if (hit.rigidbody != null)
                hit.rigidbody.AddForceAtPosition(transform.forward * shotForce, hit.point);

            if (hit.transform.gameObject.TryGetComponent(out ExplosiveItem explosive))
                explosive.Detonate();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * shotRange);
    }
}