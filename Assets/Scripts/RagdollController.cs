using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    Animator animator;
    Rigidbody[] rbs;

    private void Start()
    {
        //animator = GetComponent<Animator>();
        rbs = GetComponentsInChildren<Rigidbody>();

        RagdollActive(false);
    }

    public void RagdollActive(bool state)
    {
        foreach (Rigidbody rb in rbs)
            rb.isKinematic = !state;
    }
}