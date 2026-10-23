using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkRandomizer : MonoBehaviour
{
    public Animator anim;
    public int minRange;
    public int maxRange;
    public float timer;
    int choosedTime;

    void Start()
    {
        choosedTime = Random.Range(minRange, maxRange);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= choosedTime)
        {
            timer = 0;
            blinking();
        }
    }

    public void blinking()
    {
        anim.SetTrigger("blink");
        choosedTime = Random.Range(minRange, maxRange);
    }
}
