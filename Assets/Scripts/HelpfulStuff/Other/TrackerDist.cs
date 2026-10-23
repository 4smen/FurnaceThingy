using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerDist : MonoBehaviour
{
    GameObject playerPH;
    public float distPlayer;
    public float wantedDist;

    void Start()
    {
        playerPH = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        distPlayer = Vector3.Distance(playerPH.transform.position, transform.position);
    }
}
