using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    Image bar;
    public float amount;

    void Awake()
    {
        bar = GetComponent<Image>();
    }

    void Update()
    {
        bar.fillAmount = amount / 2.5f;
    }
}
