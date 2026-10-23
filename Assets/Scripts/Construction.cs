using Mono.Cecil.Cil;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class Construction : MonoBehaviour
{
    [SerializeField] private GameObject contraptionPrefab;
    [SerializeField] private LayerMask walkableLayer;

    [SerializeField] private PlayerLocal playerLocal;

    GameObject movePH;
    Ray ray;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f))
            {
                playerLocal.isConstructing = !playerLocal.isConstructing;
                SetConstructionState(hit);
            }
        }
        if (movePH != null && playerLocal.isConstructing)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f, walkableLayer))
            {
                movePH.transform.SetPositionAndRotation(hit.point, Quaternion.identity);
            }

            if (Input.GetKeyDown(Controls.Singletone.confirmKey))
            {
                playerLocal.isConstructing = false;
                Destroy(movePH);
                movePH = null;

                PlaceContraption(contraptionPrefab, hit.point);
            }
        }
    }

    void SetConstructionState(RaycastHit hit)
    {
        if (playerLocal.isConstructing)
        {
            movePH = Instantiate(contraptionPrefab, hit.point, Quaternion.identity);
            movePH.GetComponent<Animator>().enabled = false;
            movePH.GetComponent<NavMeshObstacle>().enabled = false;
        }
        else
        {
            Destroy(movePH);
            movePH = null;
        }
    }

    void PlaceContraption(GameObject contraptionPrefab, Vector3 position)
    {
        var contraption = Instantiate(contraptionPrefab, position, Quaternion.identity);
    }
}
