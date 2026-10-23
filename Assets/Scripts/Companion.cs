using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Companion : MonoBehaviour
{
    [SerializeField] Transform target;
    //[SerializeField] float random_point_radius;
    //[SerializeField] Transform patrol_zone_center;
    //[SerializeField] GameObject pointPrefab;
    //[SerializeField] float minTimeToGo;
    //[SerializeField] float maxTimeToGo;
    [SerializeField] float angularSpeed;
    float timeToGo;
    Vector3 random_point;

    NavMeshAgent agent;
    NavMeshPath nav_path;

    //public string savePosX;
    //public string savePosZ;

    //public string saveRotY;

    void Awake()
    {
        //savePosX = "Xpos Of " + gameObject.name;
        //savePosZ = "Zpos Of " + gameObject.name;
        //saveRotY = "Yrot Of " + gameObject.name;

        //if (PlayerPrefs.HasKey(savePosX))
        //{
        //    transform.position = new Vector3(
        // PlayerPrefs.GetFloat(savePosX),
        // transform.position.y,
        // PlayerPrefs.GetFloat(savePosZ)
        //    );

        //    transform.eulerAngles = new Vector3(0, PlayerPrefs.GetFloat(saveRotY), 0);
        //}

        agent = GetComponent<NavMeshAgent>();
        nav_path = new NavMeshPath();
        //StartCoroutine("RandomMoving");
    }

    void Start()
    {
        agent.SetDestination(target.position);
    }

    //void createPoint()
    //{
    //    //bool correct_point = false;
    //    //while (!correct_point)
    //    //{
    //    //    NavMeshHit nav_hit;
    //    //    NavMesh.SamplePosition(Random.insideUnitSphere * random_point_radius + patrol_zone_center.position, out nav_hit, random_point_radius, NavMesh.AllAreas);
    //    //    random_point = nav_hit.position;
    //    //    Instantiate(pointPrefab, nav_hit.position, Quaternion.identity);
    //    //    agent.CalculatePath(random_point, nav_path);
    //    //    if (nav_path.status == NavMeshPathStatus.PathComplete) correct_point = true;
    //    //}
    //    //agent.SetDestination(target.position);


    //    //Vector3 lookPos = target.position - transform.position;
    //    //lookPos.y = 0;
    //    //Quaternion rotation = Quaternion.LookRotation(lookPos);
    //    //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, angularSpeed);
    //}

    //IEnumerator RandomMoving()
    //{
    //    timeToGo = Random.Range(minTimeToGo, maxTimeToGo);
    //    yield return new WaitForSeconds(timeToGo);
    //    createPoint();
    //    yield return StartCoroutine("RandomMoving");
    //    yield break;
    //}

    //public void savePosition()
    //{
    //    PlayerPrefs.SetFloat(savePosX, transform.position.x);
    //    PlayerPrefs.SetFloat(savePosZ, transform.position.z);

    //    PlayerPrefs.SetFloat(saveRotY, transform.eulerAngles.y);
    //}
}
