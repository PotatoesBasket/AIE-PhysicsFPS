using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : ExplosiveItem
{
    [SerializeField] float throwForce = 0;
    [SerializeField] float detonateTime = 0;
    float timer = 0;

    void Update()
    {
        if (!GameManager.IsPaused)
        {
            CountdownToDetonation();
        }
    }

    public override bool Activate()
    {
        gameObject.SetActive(true);
        body.AddForce(transform.forward * throwForce);
        return true;
    }

    protected override void OnReset()
    {
        timer = 0;
    }

    void CountdownToDetonation()
    {
        if (timer >= detonateTime)
            Detonate();

        timer += Time.deltaTime;
    }
}