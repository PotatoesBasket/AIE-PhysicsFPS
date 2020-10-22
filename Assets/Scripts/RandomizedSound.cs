using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizedSound : MonoBehaviour
{
    public List<AudioClip> clips;

    public AudioClip GetRandomizedClip()
    {
        return clips[Random.Range(0, clips.Count)];
    }
}