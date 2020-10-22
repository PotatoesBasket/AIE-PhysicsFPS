using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateCollectable : MonoBehaviour
{
    [SerializeField] float spinSpeed = 0;
    [SerializeField] float floatSpeed = 0;
    float timer = 0;
    bool reverse = false;

    private void Update()
    {
        if (timer >= 1)
            reverse = true;
        if (timer <= -1)
            reverse = false;

        transform.Rotate(new Vector3(0, spinSpeed * Time.deltaTime, 0));
        transform.position += new Vector3(0, Mathf.Sin(timer * floatSpeed), 0);

        if (reverse)
            timer -= Time.deltaTime;
        else
            timer += Time.deltaTime;
    }
}