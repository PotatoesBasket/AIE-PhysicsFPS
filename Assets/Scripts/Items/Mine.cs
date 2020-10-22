using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : ExplosiveItem
{
    [SerializeField] float placementRange = 0;
    [SerializeField] LayerMask layerMask = 0;

    public override bool Activate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.pixelRect.center);

        if (Physics.Raycast(ray, out RaycastHit hit, placementRange, layerMask, QueryTriggerInteraction.Ignore))
        {
            body.isKinematic = true;
            transform.position = hit.point;
            transform.up = hit.normal;
            body.isKinematic = false;

            gameObject.SetActive(true);
            return true;
        }

        return false;
    }

    protected override void OnReset() {}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 8) // ignore environment layer
            Detonate();
    }
}