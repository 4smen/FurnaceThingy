using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationParticle : MonoBehaviour
{
    public bool isPlay;

    void Update()
    {
        if (isPlay == true) play();

        if (isPlay == false) stop();
    }

    public void play()
    {
        GetComponent<ParticleSystem>().Play();
    }

    public void stop()
    {
        GetComponent<ParticleSystem>().Stop();
    }
}
