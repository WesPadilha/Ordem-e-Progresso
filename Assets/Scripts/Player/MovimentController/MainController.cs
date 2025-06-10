using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MainController : MonoBehaviour
{
    public Camera mainCamera;
    private RaycastHit hit;
    public NavMeshAgent agent;
    public ScreenController screenController;
    public GameObject selectionPrefab;

    public float stopDistance = 4f;
    private bool clickedOnNPC = false;
    private bool clickedOnStorage = false;
    private GameObject currentSelection;

    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (screenController != null && (screenController.IsAnyUIActive() || Pause.GameIsPaused || screenController.IsStorageOpen()))
            return;

        // Atualiza animação de corrida: só corre se tiver caminho e velocidade significativa
        if (animator != null)
        {
            animator.SetBool("Run", agent.hasPath && agent.velocity.magnitude > 0.1f);
        }

        if (Input.GetMouseButtonDown(1))
        {
            MoveToGround();
        }

        if (Input.GetMouseButtonDown(0))
        {
            MoveToNPC();
            MoveToStorage();
        }

        CheckArrivalToNPC();
        CheckArrivalToStorage();
    }

    private void MoveToGround()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                agent.SetDestination(hit.point);
                clickedOnNPC = false;
                clickedOnStorage = false;

                if (currentSelection != null)
                    Destroy(currentSelection);

                Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);
                currentSelection = Instantiate(selectionPrefab, hit.point + new Vector3(0, 0.25f, 0), rotation);
                StartCoroutine(AnimateSelection(currentSelection));
            }
        }
    }

    private IEnumerator AnimateSelection(GameObject selection)
    {
        float duration = 2f;
        float elapsedTime = 0f;
        Vector3 initialScale = selection.transform.localScale;

        while (elapsedTime < duration)
        {
            if (selection == null) yield break;

            selection.transform.localScale = initialScale + Vector3.one * (elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (selection != null)
            Destroy(selection);
    }

    private void MoveToNPC()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("NPC"))
            {
                Vector3 npcPosition = hit.collider.transform.position;
                Vector3 direction = (transform.position - npcPosition).normalized;
                Vector3 targetPosition = npcPosition + direction * stopDistance;

                agent.SetDestination(targetPosition);
                clickedOnNPC = true;
                clickedOnStorage = false; // garante que só um tipo é clicado
            }
        }
    }

    private void MoveToStorage()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Storage"))
            {
                Vector3 storagePosition = hit.collider.transform.position;
                Vector3 direction = (transform.position - storagePosition).normalized;
                Vector3 targetPosition = storagePosition + direction * stopDistance;

                agent.SetDestination(targetPosition);
                clickedOnStorage = true;
                clickedOnNPC = false; // garante que só um tipo é clicado
            }
        }
    }

    private void CheckArrivalToNPC()
    {
        if (clickedOnNPC && !agent.pathPending && agent.remainingDistance <= stopDistance)
        {
            hit.collider.GetComponent<DialogNPC>()?.StartConversation();
            clickedOnNPC = false;
            StopAgentAndAnimation();
        }
    }

    private void CheckArrivalToStorage()
    {
        if (clickedOnStorage && !agent.pathPending && agent.remainingDistance <= stopDistance)
        {
            Storage storageScript = hit.collider.GetComponent<Storage>();
            if (storageScript != null)
                storageScript.OpenStorage();

            clickedOnStorage = false;
            StopAgentAndAnimation();
        }
    }

    private void StopAgentAndAnimation()
    {
        if (agent != null && agent.hasPath)
        {
            agent.ResetPath();
        }

        if (animator != null)
        {
            animator.SetBool("Run", false);
        }
    }

    public void StopMovement()
    {
        if (agent != null && agent.hasPath)
            agent.ResetPath();

        if (currentSelection != null)
        {
            Destroy(currentSelection);
            currentSelection = null;
        }

        if (animator != null)
        {
            animator.SetBool("Run", false);
        }

        clickedOnNPC = false;
        clickedOnStorage = false;
    }
}
