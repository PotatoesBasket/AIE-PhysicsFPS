using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplosiveItemController : MonoBehaviour
{
    [SerializeField] List<ExplosiveItem> pool = null;   // reuseable pool of item
    [SerializeField] int beginningAmount = 0;           // how many in inventory at beginning of game
    [SerializeField] int maxAmount = 0;                 // how many player can hold
    [SerializeField] Text HUDCounter = null;            // where to display count on HUD

    public int AmountRemaining { get; private set; }

    int currentPoolIdx = 0;
    AudioSource audioSource; // used item sound

    private void Awake()
    {
        AmountRemaining = beginningAmount;
        audioSource = GetComponent<AudioSource>();
        UpdateHUDCounter();
    }

    public void Use()
    {
        if (AmountRemaining > 0)
        {
            ExplosiveItem obj = NextInPool();

            obj.ResetObject(transform.position, transform.rotation);

            if (obj.Activate())
            {
                --AmountRemaining;
                UpdateHUDCounter();

                if (audioSource != null)
                    audioSource.PlayOneShot(AudioManager.current.throwItem);
            }
        }
    }

    public void AddToAmount(int amount)
    {
        AmountRemaining += amount;
        AmountRemaining = Mathf.Clamp(AmountRemaining, 0, maxAmount);
        UpdateHUDCounter();
    }

    void UpdateHUDCounter()
    {
        if (HUDCounter != null)
            HUDCounter.text = "x" + AmountRemaining.ToString();
    }

    ExplosiveItem NextInPool()
    {
        ++currentPoolIdx;

        if (currentPoolIdx >= pool.Count)
            currentPoolIdx = 0;

        return pool[currentPoolIdx];
    }
}