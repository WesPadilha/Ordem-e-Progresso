using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    public NavMeshAgent agent;
    public int maxActionPoints = 10;
    public float attackDistance = 5f; 
    private float currentActionPoints;
    private EnemyGroupController groupController;

    private GameObject player;
    private Vector3 lastPosition;
    private bool isChasing = false;

    public bool IsChasing()
    {
        return isChasing;
    }

    public float GetCurrentActionPoints()
    {
        return currentActionPoints;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentActionPoints = maxActionPoints;
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("Player não encontrado na cena com tag 'Player'");
            enabled = false;
            return;
        }

        lastPosition = transform.position;
        isChasing = false;
        groupController = GetComponentInParent<EnemyGroupController>();
    }

    void Update()
    {
        if (!isChasing || player == null) return;

        // Distância atual até o player
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Se estiver na distância de ataque ou menos, para o inimigo e faz ataque
        if (distanceToPlayer <= attackDistance)
        {
            // Gasta o restante dos pontos de ação
            currentActionPoints = 0;

            // Para o agente e a perseguição
            agent.ResetPath();
            isChasing = false;

            Debug.Log("ATAQUE");
            return;
        }

        // Atualiza destino para perseguir o player
        agent.SetDestination(player.transform.position);

        // Gasta PA conforme a distância percorrida
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (distanceMoved > 0f)
        {
            float paToSpend = distanceMoved;

            if (paToSpend > currentActionPoints)
            {
                // Sem PA suficiente, para o agente
                agent.ResetPath();
                isChasing = false;
                currentActionPoints = 0;
                Debug.Log("Inimigo ficou sem PA e parou de se mover.");
                return;
            }

            currentActionPoints -= paToSpend;
            lastPosition = transform.position;
        }

        // Se chegar muito perto do player, pode parar
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.ResetPath();
            isChasing = false;
            Debug.Log("Inimigo chegou no player e parou.");
        }
    }

    public void StartChasing()
    {
        if (currentActionPoints <= 0f)
        {
            Debug.Log("Inimigo não tem PA para começar a perseguir.");
            return;
        }

        isChasing = true;
        lastPosition = transform.position;
    }

    public void ResetActionPoints()
    {
        currentActionPoints = maxActionPoints;
    }

    public void DisableMovement()
    {
        isChasing = false;
        agent.ResetPath();
    }

    public void EnableMovement()
    {
        if (currentActionPoints > 0)
        {
            isChasing = true;
        }
    }
    public void Die()
    {
        if (groupController != null)
        {
            groupController.RemoveEnemy(this);
        }
        
        // Desativa o inimigo ou destrói o GameObject
        Destroy(gameObject);
        // Ou: gameObject.SetActive(false);
    }
}