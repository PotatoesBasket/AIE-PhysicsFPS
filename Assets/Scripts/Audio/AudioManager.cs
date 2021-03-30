using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager current = null;

    private void Awake()
    {
        if (current == null)
            current = this;
        else
            Destroy(gameObject);
    }

    public AudioClip fleshShotHit;
    public AudioClip grenadeExplosion;
    public AudioClip ammoPickup;

    public RandomizedSound assaultRifleShots;
    public RandomizedSound humanDeathGrunts;
}