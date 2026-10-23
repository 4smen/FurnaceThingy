using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public abstract class Enemy : MonoBehaviour, IDamageable, IHealable
{
    [SerializeField] protected NPCSO npcInfo;
    [SerializeField] protected int currentHP;

    [SerializeField] protected GameObject target;
    [SerializeField] protected bool targetSpotted;

    [SerializeField] protected float viewRadius;
    [SerializeField] [Range(0, 360)] protected float viewAngle;
    [SerializeField] protected float closeDetectionRadius;
    protected float lastAttackTime;

    [SerializeField] private LayerMask characterMask;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected NavMeshPath nav_path;

    protected virtual void Start()
    {
        currentHP = npcInfo.maxHP;
        nav_path = new NavMeshPath();
    }

    protected GameObject SearchForPlayer()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, viewRadius, characterMask);

        if (targets.Length == 0)
        {
            return null;
        }

        Transform target = targets[0].transform;
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
            {
                return target.gameObject;
            }
        }

        return null;
    }

    protected abstract void MoveTowardsTarget();

    protected abstract void Attack();

    protected virtual void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP <= 0) Death();
    }

    protected virtual void TakeHeal(int amount)
    {
        if (currentHP >= npcInfo.maxHP) return;
        currentHP += amount;
    }

    protected virtual void Death()
    {
        Destroy(gameObject);
    }

    #region Gizmos
    protected virtual void OnDrawGizmos()
    {
        // 1. Рисуем окружность (радиус видимости)
        Gizmos.color = Color.white;
        // Нарисуем проволочную сферу, сплюснутую по вертикали, чтобы она выглядела как круг на земле
        GizmosExtensions.DrawWireSkinnyCircle(transform.position, viewRadius);

        // 2. Рисуем крайние линии угла обзора
        Gizmos.color = Color.yellow;
        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2, false);
        Vector3 rightBoundary = DirFromAngle(viewAngle / 2, false);

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);

        // 3. Подсвечиваем игрока красным, если он в зоне видимости (опционально, для теста)
        GameObject player = SearchForPlayer();
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.transform.position);
        }
    }

    // Вспомогательный метод для расчета направления лучей под углом
    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void IDamageable.TakeDamage(int amount)
    {
        throw new System.NotImplementedException();
    }

    void IHealable.TakeHeal(int amount)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}

public class MeleeEnemy : Enemy
{
    [SerializeField] protected Base.AIState currentState = Base.AIState.Patrol;

    [Header("Prefabs & Points")]
    [SerializeField] protected GameObject pointPrefab;
    private GameObject currentPointMarker; // Ссылка на созданный маркер, чтобы удалять его

    [Header("Patrol Settings")]
    [SerializeField] protected float minTimeToGo;
    [SerializeField] protected float maxTimeToGo;
    private bool isPatrolWaiting = false;

    [Header("Combat Settings")]
    [SerializeField] protected float random_point_radius;
    [SerializeField] protected float distanceToPlayer;
    [SerializeField] protected float angularSpeed;

    private Vector3 currentTargetPoint; // Хранит выбранную в данный момент точку NavMesh
    private bool hasGeneratedCombatPoint = false; // Защита от спама точек в Update

    protected override void Start()
    {
        base.Start();
        currentState = Base.AIState.Patrol;
    }

    protected virtual void Update()
    {
        // 1. Постоянно обновляем цель через Raycast/OverlapSphere
        target = SearchForPlayer();

        // 2. Логика переключения состояний
        if (target != null)
        {
            if (!targetSpotted)
            {
                // Только что заметили игрока
                targetSpotted = true;
                isPatrolWaiting = false;
                StopAllCoroutines(); // Мгновенно отменяем патрульные ожидания

                // Переходим к маневру (случайная точка у игрока)
                currentState = Base.AIState.Chase;
                hasGeneratedCombatPoint = false;
            }
        }
        else
        {
            if (targetSpotted)
            {
                // Потеряли игрока из виду
                targetSpotted = false;
                if (currentPointMarker != null) Destroy(currentPointMarker);
                agent.ResetPath();
                currentState = Base.AIState.Patrol;
            }
        }

        // 3. Выполнение логики в зависимости от текущего состояния
        switch (currentState)
        {
            case Base.AIState.Patrol:
                Patrol();
                break;

            case Base.AIState.Chase:
                MoveTowardsTarget();
                break;
        }

        // Вращение в сторону движения (если агент движется)
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 lookPos = agent.steeringTarget - transform.position;
            lookPos.y = 0;
            if (lookPos != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, angularSpeed * Time.deltaTime);
            }
        }
    }

    // 1. Состояние патрулирования
    private void Patrol()
    {
        // Если агент идет — ждем окончания пути
        if (agent.hasPath)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.ResetPath();
            }
            return;
        }

        // Если агент остановился и еще не ждет таймер — запускаем таймер отдыха
        if (!isPatrolWaiting)
        {
            StartCoroutine(PatrolTimer());
        }
    }

    IEnumerator PatrolTimer()
    {
        isPatrolWaiting = true;
        float timeToGo = Random.Range(minTimeToGo, maxTimeToGo);
        yield return new WaitForSeconds(timeToGo);

        // Выбираем точку вокруг СЕБЯ и идем туда
        GoToRandomPoint(transform.position);
        isPatrolWaiting = false;
    }


    // Универсальный метод поиска точки на NavMesh и отправки агента
    private void GoToRandomPoint(Vector3 centerOrigin)
    {
        if (currentPointMarker != null) Destroy(currentPointMarker);

        bool correct_point = false;
        Vector3 chosenPoint = transform.position;

        int attempts = 0; // Защита от бесконечного цикла while, если вокруг нет NavMesh
        while (!correct_point && attempts < 10)
        {
            attempts++;
            NavMeshHit nav_hit;
            Vector3 randomSpherePoint = Random.insideUnitSphere * random_point_radius + centerOrigin;

            if (NavMesh.SamplePosition(randomSpherePoint, out nav_hit, random_point_radius, NavMesh.AllAreas))
            {
                agent.CalculatePath(nav_hit.position, nav_path);
                if (nav_path.status == NavMeshPathStatus.PathComplete)
                {
                    chosenPoint = nav_hit.position;
                    correct_point = true;
                }
            }
        }

        currentTargetPoint = chosenPoint;
        agent.SetDestination(currentTargetPoint);

        // Спавним префаб-визуализатор точки
        if (pointPrefab != null)
        {
            currentPointMarker = Instantiate(pointPrefab, currentTargetPoint, Quaternion.identity);
        }
    }

    // 3. Состояние прямой погони: бежим прямо на игрока и атакуем
    protected override void MoveTowardsTarget()
    {
        if (target == null) return;

        // Постоянно обновляем путь до бегущего игрока
        agent.SetDestination(target.transform.position);

        float distance = Vector3.Distance(transform.position, target.transform.position);

        // Условие атаки
        if (distance <= npcInfo.attackRange && Time.time >= lastAttackTime + npcInfo.attackCooldown)
        {
            Attack();
        }
    }

    protected override void Attack()
    {
        lastAttackTime = Time.time;
        Debug.Log($"{npcInfo.npcName} ударил игрока на {npcInfo.damage} урона!");
    }

    #region Gizmos
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.green;
        GizmosExtensions.DrawWireSkinnyCircle(transform.position, random_point_radius);
    }
    #endregion
}