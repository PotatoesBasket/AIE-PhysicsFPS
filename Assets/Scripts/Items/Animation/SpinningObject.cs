using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningObject : MonoBehaviour
{
    [SerializeField] float spinSpeed = 50;

    private void Update()
    {
        transform.Rotate(new Vector3(0, spinSpeed * Time.deltaTime, 0));
    }
}