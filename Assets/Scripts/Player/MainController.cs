using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainController : MonoBehaviour
{
    public Camera camera;
    private RaycastHit hit;
    private NavMeshAgent agent;
    private string groundTag = "Ground";
    private string npcTag = "NPC";

    public ScreenController screenController;

    // Distância mínima até o NPC
    public float stopDistance = 4f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Evita interação enquanto a UI está ativa
        if (screenController != null && screenController.IsAnyUIActive())
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
        }
    }

    private void MoveToGround()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag(groundTag))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    private void MoveToNPC()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

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
            }
        }
    }
}
