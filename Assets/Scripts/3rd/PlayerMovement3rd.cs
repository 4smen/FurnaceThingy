using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class PlayerMovement3rd : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private Transform moveAnimation;
    [SerializeField] private LayerMask walkableLayer;
    public float moveSpeed;

    public static event Action<RaycastHit> OnDestinationChangeCallback;

    public bool canClick = true;

    void Start()
    {
        SetSpeed(moveSpeed);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canClick)
        {
            StartCoroutine(ClickTimer());

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f, walkableLayer))
            {
                print(hit.collider.gameObject.name);

                Instantiate(moveAnimation, hit.point, Quaternion.identity);
                SetDestination(hit);
            }
        }
    }

    public void SetSpeed(float newSpeed)
    {
        agent.speed = newSpeed;
    }

    public void SetDestination(RaycastHit hit)
    {
        agent.destination = hit.point;
        //agent.transform.rotation = Quaternion.Euler(Mathf.Atan2(point.y - agent.transform.position.y, point.x - agent.transform.position.x) * Mathf.Rad2Deg - 90, agent.transform.rotation.y, agent.transform.rotation.x);

        OnDestinationChangeCallback.Invoke(hit);
    }

    IEnumerator ClickTimer()
    {
        canClick = false;
        yield return new WaitForSeconds(0.1f);
        canClick = true;
    }
}
