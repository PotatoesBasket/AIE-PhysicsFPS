using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves an object back and forth along it's y axis

public class OscillatingObject : MonoBehaviour
{
    [SerializeField] float loopTime = 3.0f;
    [SerializeField] float distance = 0.15f;

    Vector3 origin = Vector3.zero;

    private void Start()
    {
        origin = transform.position;
        origin.y += distance; // offset so that objects in editor appear at the lowest point of the loop to avoid floating into floors
    }

    private void Update()
    {
        float y = distance * Mathf.Cos(2.0f * 3.14159f * Time.time / loopTime);

        transform.position = new Vector3(origin.x, origin.y + y, origin.z);
    }
}