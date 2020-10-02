using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision c)
    {
        RagdollController rdc = c.gameObject.GetComponentInParent<RagdollController>();

        if (rdc != null)
        {
            rdc.RagdollActive(true);
        }
    }
}