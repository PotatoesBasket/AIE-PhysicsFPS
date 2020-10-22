using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour, IDamageable, IHealable
{
    // stats
    public float CurrentHealth { get; private set; }
    [SerializeField] float maxHealth = 100;

    // UI
    [SerializeField] Slider healthBar = null;

    // sounds
    AudioSource audioSource;
    AudioClip damageSound;
    RandomizedSound deathSounds;

    // on death event trigger
    public delegate void Death();
    public event Death OnDeath;

    bool isDead = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        damageSound = AudioManager.current.fleshShotHit;
        deathSounds = AudioManager.current.humanDeathGrunts;

        CurrentHealth = maxHealth;

        healthBar.minValue = 0;
        healthBar.maxValue = maxHealth;

        UpdateBar();
    }

    //---PUBLIC

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        CurrentHealth = Mathf.Max(CurrentHealth - Mathf.Abs(amount), 0);
        UpdateBar();

        if (audioSource != null && damageSound != null)
            audioSource.PlayOneShot(damageSound);

        if (CurrentHealth <= 0)
            Die();
    }

    public void RestoreHealth(float amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + Mathf.Abs(amount), maxHealth);
        UpdateBar();
    }

    //---PRIVATE

    void UpdateBar() // update health bar UI
    {
        if (healthBar != null)
            healthBar.value = CurrentHealth;
    }

    void Die() // stuff to do on death
    {
        OnDeath?.Invoke();

        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        if (audioSource != null && deathSounds != null)
            audioSource.PlayOneShot(deathSounds.GetRandomizedClip());

        isDead = true;
    }
}