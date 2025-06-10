using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyChase : MonoBehaviour
{
    [Header("Configuração")]
    public NavMeshAgent agent;
    public int maxActionPoints = 10;

    private float currentActionPoints;
    private EnemyGroupController groupController;
    private EnemyWeapon weapon;
    private GameObject player;

    private Vector3 lastPosition;
    private bool isChasing = false;
    private bool isActing = false;

    public bool IsChasing() => isChasing;
    public float GetCurrentActionPoints() => currentActionPoints;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        weapon = GetComponent<EnemyWeapon>();
        groupController = GetComponentInParent<EnemyGroupController>();

        if (player == null)
        {
            Debug.LogWarning("Player não encontrado.");
            enabled = false;
        }

        if (weapon == null)
            Debug.LogWarning("EnemyWeapon não encontrado.");

        ResetActionPoints();
    }

    void Update()
    {
        if (!isChasing) return;

        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (distanceMoved > 0.05f)
        {
            float paToSpend = distanceMoved;

            if (paToSpend >= currentActionPoints)
            {
                currentActionPoints = 0;
                StopChasing();
                return;
            }

            currentActionPoints -= paToSpend;
            lastPosition = transform.position;
        }

        LookAtPlayer();
    }

    private void LookAtPlayer()
    {
        Vector3 direction = (player.transform.position - transform.position);
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    public void StartChasing()
    {
        if (isActing) return;

        isChasing = true;
        lastPosition = transform.position;
        isActing = true;

        StartCoroutine(EnemyTurnLogic());
    }

    IEnumerator EnemyTurnLogic()
    {
        Debug.Log($"{gameObject.name} iniciou seu turno.");

        while (currentActionPoints > 0f)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            float attackRange = weapon.GetAttackRange();
            int attackCost = weapon.GetActionPointsCost();

            // ✅ Se está muito perto, afasta
            if (distanceToPlayer <= 3f)
            {
                Debug.Log($"{gameObject.name} está muito perto do player, afastando...");
                yield return MoveAwayFromPlayer(3f);
                yield return RotateTowardsPlayerSmoothly();
                distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            }

            // ✅ Se dentro do alcance, ataca
            if (distanceToPlayer <= attackRange)
            {
                if (currentActionPoints >= attackCost)
                {
                    yield return RotateTowardsPlayerSmoothly();

                    Attack();
                    currentActionPoints -= attackCost;

                    yield return new WaitForSeconds(0.5f);
                    continue;
                }
                else
                {
                    Debug.Log($"{gameObject.name} sem PA para atacar.");
                    break;
                }
            }

            // ✅ Se não está no alcance, move-se até o player
            if (currentActionPoints > 0)
            {
                Vector3 destination = player.transform.position;

                agent.SetDestination(destination);

                while (agent.pathPending)
                    yield return null;

                // Espera mover um pouco ou até estar no alcance
                while (agent.remainingDistance > attackRange && currentActionPoints > 0)
                {
                    yield return null;
                }

                agent.ResetPath();

                if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
                {
                    Debug.Log($"{gameObject.name} chegou no alcance.");
                    continue;
                }
                else
                {
                    Debug.Log($"{gameObject.name} não conseguiu alcançar o player.");
                    break;
                }
            }
        }

        StopChasing();
    }

    private IEnumerator MoveAwayFromPlayer(float distance)
    {
        Vector3 directionAway = (transform.position - player.transform.position).normalized;
        Vector3 targetPos = transform.position + directionAway * distance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            Debug.Log($"{gameObject.name} não encontrou posição para se afastar.");
            yield break;
        }

        while (agent.pathPending)
            yield return null;

        while (agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        agent.ResetPath();
    }

    private IEnumerator RotateTowardsPlayerSmoothly()
    {
        float rotationSpeed = 360f;

        while (!IsFacingPlayer())
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            direction.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            yield return null;
        }
    }

    private bool IsFacingPlayer()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        directionToPlayer.y = 0;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        return angle < 5f;
    }

    public void Attack()
    {
        if (weapon == null) return;

        if (IsFacingPlayer())
        {
            weapon.Atirar();
            Debug.Log($"{gameObject.name} atacou.");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} tentou atacar sem estar mirando no player.");
        }
    }

    public void ResetActionPoints()
    {
        currentActionPoints = maxActionPoints;
    }

    public void DisableMovement()
    {
        StopAllCoroutines();
        isChasing = false;
        isActing = false;
        agent.ResetPath();
    }

    private void StopChasing()
    {
        isChasing = false;
        isActing = false;
        agent.ResetPath();
        Debug.Log($"{gameObject.name} terminou seu turno.");
    }

    public void Die()
    {
        if (groupController != null)
            groupController.RemoveEnemy(this);

        Destroy(gameObject);
    }
}
