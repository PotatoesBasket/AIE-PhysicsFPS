using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainbowText : MonoBehaviour
{
    [SerializeField] float speed = 0;

    Text text;
    Color color;
    float hue;
    float timer;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void Update()
    {
        if (timer > 1)
            timer -= 1;

        hue = Mathf.Lerp(0, 1, timer);

        color = Color.HSVToRGB(hue, 1, 1);
        text.color = color;

        timer += Time.deltaTime * speed;
    }
}