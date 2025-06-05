using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MovimentCombat : MonoBehaviour
{
    public Camera mainCamera;
    public NavMeshAgent agent;
    public GameObject selectionPrefab;
    public MovementUI ui; // UI para exibir pontos de ação
    public ScreenController screenController; // Bloquear movimento se UI estiver aberta

    public CharacterData characterData; // Referência para pegar actionPoints

    private float currentActionPoints;
    private Vector3 lastPosition;
    private GameObject currentSelection;

    private bool isMoving = false;
    private Vector3 targetPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentActionPoints = characterData.actionPoints;
        ui?.UpdateActionPoints(characterData.actionPoints);
        lastPosition = transform.position;
    }

    void Update()
    {
        if (screenController != null && (screenController.IsAnyUIActive() || Pause.GameIsPaused))
            return;

        // Atualiza gasto de PA conforme distância percorrida
        if (isMoving)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);

            if (distanceMoved > 0f)
            {
                // Gasta PA baseado na distância percorrida
                float paToSpend = distanceMoved;

                if (paToSpend > currentActionPoints)
                {
                    // Sem PA suficiente para continuar, para o agente
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

            // Verifica se chegou ao destino
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

                // Se a distância é maior que PA disponível, limita o destino
                if (distanceToDestination > currentActionPoints)
                {
                    Vector3 direction = (destination - transform.position).normalized;
                    destination = transform.position + direction * currentActionPoints;
                    distanceToDestination = currentActionPoints;
                }

                // Atualiza destino do agente
                agent.SetDestination(destination);
                isMoving = true;
                targetPosition = destination;
                lastPosition = transform.position;

                // Atualiza seleção visual
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

    public float GetCurrentActionPoints()
    {
        return currentActionPoints;
    }
    
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
        // Não precisamos fazer nada especial aqui pois o movimento
        // é ativado quando o jogador clica com o mouse
        // Podemos apenas resetar o estado se necessário
        isMoving = false;
        agent.ResetPath();
        DestroySelection();
        
        // Atualiza a UI para mostrar os pontos de ação disponíveis
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
