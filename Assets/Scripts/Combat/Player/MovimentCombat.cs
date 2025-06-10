using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MovimentCombat : MonoBehaviour
{
    public Camera mainCamera;
    public NavMeshAgent agent;
    public GameObject selectionPrefab;
    public MovementUI ui;
    public ScreenController screenController;
    public CharacterData characterData;

    private float currentActionPoints;
    private Vector3 lastPosition;
    private GameObject currentSelection;
    private bool isMoving = false;
    private Vector3 targetPosition;

    private Animator animator; // << NOVO

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // << NOVO

        currentActionPoints = characterData.actionPoints;
        ui?.UpdateActionPoints(characterData.actionPoints);
        lastPosition = transform.position;
    }

    void Update()
    {
        if (screenController != null && (screenController.IsAnyUIActive() || Pause.GameIsPaused))
            return;

        // Atualiza animação de corrida
        if (animator != null)
        {
            animator.SetBool("Run", agent.velocity.magnitude > 0.1f);
        }

        if (isMoving)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);

            if (distanceMoved > 0f)
            {
                float paToSpend = distanceMoved;

                if (paToSpend > currentActionPoints)
                {
                    agent.ResetPath();
                    isMoving = false;
                    currentActionPoints = 0;
                    ui?.UpdateActionPoints((int)currentActionPoints);
                    return;
                }

                currentActionPoints -= paToSpend;
                ui?.UpdateActionPoints((int)currentActionPoints);
                lastPosition = transform.position;
            }

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                isMoving = false;
                DestroySelection();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            TrySetNewDestination();
        }
    }

    private void TrySetNewDestination()
    {
        if (isMoving)
        {
            Debug.Log("Já está se movendo. Espere chegar ao destino.");
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                Vector3 destination = hit.point;
                float distanceToDestination = Vector3.Distance(transform.position, destination);

                if (distanceToDestination < 1f)
                {
                    Debug.Log("Distância muito curta, não move nem gasta PA.");
                    return;
                }

                if (currentActionPoints <= 0f)
                {
                    Debug.Log("Sem pontos de ação para se mover.");
                    return;
                }

                if (distanceToDestination > currentActionPoints)
                {
                    Vector3 direction = (destination - transform.position).normalized;
                    destination = transform.position + direction * currentActionPoints;
                    distanceToDestination = currentActionPoints;
                }

                agent.SetDestination(destination);
                isMoving = true;
                targetPosition = destination;
                lastPosition = transform.position;

                DestroySelection();
                Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);
                currentSelection = Instantiate(selectionPrefab, destination + new Vector3(0, 0.25f, 0), rotation);
                StartCoroutine(AnimateSelection(currentSelection));
            }
        }
    }

    private void DestroySelection()
    {
        if (currentSelection != null)
        {
            Destroy(currentSelection);
            currentSelection = null;
        }
    }

    private IEnumerator AnimateSelection(GameObject selection)
    {
        float duration = 2f;
        float elapsedTime = 0f;
        Vector3 initialScale = selection.transform.localScale;

        while (elapsedTime < duration)
        {
            if (selection == null)
                yield break;

            selection.transform.localScale = initialScale + Vector3.one * (elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (selection != null)
            Destroy(selection);
    }

    public float GetCurrentActionPoints() => currentActionPoints;

    public void SpendActionPoints(int amount)
    {
        currentActionPoints = Mathf.Max(0, currentActionPoints - amount);
        ui?.UpdateActionPoints((int)currentActionPoints);
    }

    public void DisableMovement()
    {
        isMoving = false;
        agent.ResetPath();
        DestroySelection();
    }

    public void ResetActionPoints()
    {
        currentActionPoints = characterData.actionPoints;
        ui?.UpdateActionPoints((int)currentActionPoints);
    }

    public void EnableMovement()
    {
        isMoving = false;
        agent.ResetPath();
        DestroySelection();
        ui?.UpdateActionPoints((int)currentActionPoints);
    }

    public void StopMovement()
    {
        if (agent != null && agent.hasPath)
        {
            agent.ResetPath();
        }

        isMoving = false;
        DestroySelection();
    }
}
