using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyChase : MonoBehaviour
{
    public NavMeshAgent agent;
    public int maxActionPoints = 10;
    private float currentActionPoints;
    private EnemyGroupController groupController;

    private GameObject player;
    private Vector3 lastPosition;
    private bool isChasing = false;

    private EnemyWeapon weapon;

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
        {
            Debug.LogWarning("EnemyWeapon não encontrado.");
        }

        ResetActionPoints();
    }

    void Update()
    {
        if (!isChasing) return;

        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (distanceMoved > 0f)
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

        // 👉 Faz o inimigo sempre olhar para o player, sem gastar PA
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0; // Mantém a rotação apenas no eixo Y
        if (direction.magnitude > 0.01f) // Evita olhar para direção nula
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f); // Suaviza a rotação
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

            // 👉 Se está muito perto, afasta
            if (distanceToPlayer <= 4f)
            {
                Debug.Log($"{gameObject.name} está muito perto do player, afastando...");

                yield return MoveAwayFromPlayer(4f);
                yield return RotateTowardsPlayerSmoothly();

                // Após rotacionar, verifica se tem PA e se está no alcance para atacar
                distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

                if (distanceToPlayer <= attackRange && currentActionPoints >= attackCost)
                {
                    Attack();
                    currentActionPoints -= attackCost;
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }
                else
                {
                    Debug.Log($"{gameObject.name} não tem PA suficiente ou não está no alcance após afastar.");
                    break;
                }
            }
            // 👉 Se está no alcance, ataca
            else if (distanceToPlayer <= attackRange)
            {
                if (currentActionPoints >= attackCost)
                {
                    // Primeiro rotaciona suavemente
                    yield return RotateTowardsPlayerSmoothly();

                    // Depois ataca
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
            else
            {
                // 👉 Não está no alcance, move-se até o player
                agent.SetDestination(player.transform.position);

                yield return new WaitForSeconds(0.1f);

                if (!agent.pathPending && agent.remainingDistance <= attackRange)
                {
                    agent.ResetPath();
                    Debug.Log($"{gameObject.name} parou, chegou no alcance.");
                    continue;
                }

                if (currentActionPoints <= 0f)
                {
                    Debug.Log($"{gameObject.name} ficou sem PA após mover.");
                    break;
                }
            }
        }

        // 👉 Fim do turno
        StopChasing();
    }

    private IEnumerator MoveAwayFromPlayer(float distance)
    {
        Vector3 directionAway = (transform.position - player.transform.position).normalized;
        Vector3 targetPos = player.transform.position + directionAway * distance;

        agent.SetDestination(targetPos);

        while (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
        {
            // Espera até chegar no destino
            yield return null;
        }

        agent.ResetPath();
    }
    
    private IEnumerator RotateTowardsPlayerSmoothly()
    {
        float rotationSpeed = 180f; // graus por segundo

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
        directionToPlayer.y = 0; // Ignora a inclinação

        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        return angle < 5f; // Considera alinhado se o ângulo for menor que 5 graus
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
