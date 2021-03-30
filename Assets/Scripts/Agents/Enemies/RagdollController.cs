using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    Rigidbody[] rbs;
    Animator animator;

    private void Start()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();

        SetRagdollActiveState(false);
    }

    public void SetRagdollActiveState(bool state)
    {
        if (animator != null)
            animator.enabled = !state;

        foreach (Rigidbody rb in rbs)
            rb.isKinematic = !state;
    }
}