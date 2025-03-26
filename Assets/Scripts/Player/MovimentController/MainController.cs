using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainController : MonoBehaviour
{
    public Camera mainCamera;
    private RaycastHit hit;
    public NavMeshAgent agent;
    private string groundTag = "Ground";
    private string npcTag = "NPC";
    private string storageTag = "Storage"; // Tag para Storage

    public ScreenController screenController;
    public GameObject selectionPrefab; // Prefab do selection

    // Distância mínima até o NPC ou Storage
    public float stopDistance = 4f;

    private bool clickedOnNPC = false; // Flag para saber se o NPC foi clicado
    private bool clickedOnStorage = false; // Flag para saber se o Storage foi clicado

    private GameObject currentSelection; // Referência para o selection atual

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Evita movimento se a UI estiver ativa ou se o storage estiver aberto
        if (screenController != null && (screenController.IsAnyUIActive() || Pause.GameIsPaused || screenController.IsStorageOpen()))
            return;

        // Movimento para terreno com clique direito
        if (Input.GetMouseButtonDown(1))
        {
            MoveToGround();
        }

        // Movimento para NPC com clique esquerdo
        if (Input.GetMouseButtonDown(0))
        {
            MoveToNPC();
            MoveToStorage(); // Tenta mover para o storage também
        }

        // Verifica se chegou ao destino (para NPC ou Storage)
        CheckArrivalToNPC();
        CheckArrivalToStorage();
    }

    private void MoveToGround()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag(groundTag))
            {
                agent.SetDestination(hit.point);
                clickedOnNPC = false; // Reset da flag
                clickedOnStorage = false; // Reset da flag

                // Destrói o selection anterior, se existir
                if (currentSelection != null)
                {
                    Destroy(currentSelection);
                }

                // Define a rotação desejada (90 graus no eixo X)
                Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);

                // Instancia o novo selection com a rotação ajustada
                currentSelection = Instantiate(selectionPrefab, hit.point + new Vector3(0, 0.25f, 0), rotation);
                StartCoroutine(AnimateSelection(currentSelection));
            }
        }
    }

    private IEnumerator AnimateSelection(GameObject selection)
    {
        float duration = 2f; // Duração total da animação
        float elapsedTime = 0f;
        Vector3 initialScale = selection.transform.localScale;

        while (elapsedTime < duration)
        {
            // Verifica se o objeto ainda existe
            if (selection == null)
            {
                yield break; // Sai da corrotina se o objeto foi destruído
            }

            // Aumenta a escala em 1 cm por segundo
            selection.transform.localScale = initialScale + Vector3.one * (elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Verifica novamente se o objeto ainda existe antes de destruí-lo
        if (selection != null)
        {
            Destroy(selection);
        }
    }

    private void MoveToNPC()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag(npcTag))
            {
                Vector3 npcPosition = hit.collider.transform.position;

                // Calcula a direção e o ponto a 4 metros do NPC
                Vector3 direction = (transform.position - npcPosition).normalized;
                Vector3 targetPosition = npcPosition + direction * stopDistance;

                // Configura o destino no NavMeshAgent
                agent.SetDestination(targetPosition);

                // Marca que foi clicado no NPC
                clickedOnNPC = true;
            }
        }
    }

    private void MoveToStorage()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag(storageTag))
            {
                Vector3 storagePosition = hit.collider.transform.position;

                // Calcula a direção e o ponto a 4 metros do storage
                Vector3 direction = (transform.position - storagePosition).normalized;
                Vector3 targetPosition = storagePosition + direction * stopDistance;

                // Configura o destino no NavMeshAgent
                agent.SetDestination(targetPosition);

                // Marca que foi clicado no storage
                clickedOnStorage = true;
            }
        }
    }

    private void CheckArrivalToNPC()
    {
        if (clickedOnNPC && !agent.pathPending && agent.remainingDistance <= stopDistance)
        {
            // Envia um evento ou notifica que chegou perto do NPC
            hit.collider.GetComponent<DialogNPC>()?.StartConversation();
            clickedOnNPC = false; // Reseta a flag para evitar iniciar a conversa várias vezes
        }
    }

    private void CheckArrivalToStorage()
    {
        if (clickedOnStorage && !agent.pathPending && agent.remainingDistance <= stopDistance)
        {
            // Aqui você verifica se o storage foi clicado e está dentro do range
            Storage storageScript = hit.collider.GetComponent<Storage>();
            if (storageScript != null)
            {
                storageScript.OpenStorage(); // Chama a função para abrir o Storage
            }

            clickedOnStorage = false; // Reseta a flag para não chamar novamente
        }
    }

    public void SyncWithUnitController(Vector3 position)
    {
        if (agent != null)
        {
            agent.Warp(position); // Ajusta o agente para a nova posição
        }
    }

    void OnDisable()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            // Para o agente de forma segura
            agent.isStopped = true;
        }
    }
    
    void OnEnable()
    {
        if (agent != null)
        {
            agent.enabled = true;  // Reativa o NavMeshAgent
        }
    }
}