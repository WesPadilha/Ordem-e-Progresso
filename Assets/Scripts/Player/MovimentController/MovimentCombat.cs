using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class MovimentCombat : MonoBehaviour
{
    public Camera mainCamera;
    private RaycastHit hit;
    public NavMeshAgent agent;
    private string groundTag = "Ground";
    
    public ScreenController screenController;
    public GameObject mouseFollower;
    
    public TextMeshProUGUI distanceText; // Referência para o texto que exibirá a distância

    // Sistema de Pontos de Ação
    private int maxActionPoints = 6; // Define o valor máximo
    public int actionPoints;
    private Vector3 lastPosition;
    public TextMeshProUGUI actionPointsText;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ResetActionPoints(); // Garante que inicie com o valor correto
        lastPosition = transform.position;
    }

    void Update()
    {
        // Só processa movimentação e pontos de ação se MovimentCombat estiver ativo
        if (screenController != null && screenController.IsAnyUIActive() || Pause.GameIsPaused || !enabled)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            MoveToGround();
        }

        mouseFollower.transform.position = Input.mousePosition;
        UpdateActionPoints();
        DisplayDistanceToMouse();
    }

    private void MoveToGround()
    {
        if (actionPoints <= 0) return; // Sem pontos, sem movimento
        
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag(groundTag))
            {
                Vector3 targetPosition = hit.point;
                float maxDistance = actionPoints;
                float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
                
                if (distanceToTarget > maxDistance)
                {
                    Vector3 direction = (targetPosition - transform.position).normalized;
                    targetPosition = transform.position + direction * maxDistance;
                }
                
                agent.SetDestination(targetPosition);
            }
        }
    }

    private void UpdateActionPoints()
    {
        // Consome pontos de ação apenas se estiver no MovimentCombat
        if (enabled)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            if (distanceMoved >= 1f)
            {
                int pointsUsed = Mathf.FloorToInt(distanceMoved);
                actionPoints -= pointsUsed;
                if (actionPoints < 0) actionPoints = 0;
                lastPosition = transform.position;
                UpdateActionPointsUI();
                
                if (actionPoints == 0)
                {
                    agent.SetDestination(transform.position); // Para imediatamente
                }
            }
        }
    }

    public void RestoreActionPoints(int points)
    {
        actionPoints += points;
        if (actionPoints > maxActionPoints) actionPoints = maxActionPoints;
        UpdateActionPointsUI();
    }

    public void ResetActionPoints()
    {
        actionPoints = maxActionPoints;
        UpdateActionPointsUI();
    }

    private void UpdateActionPointsUI()
    {
        if (actionPointsText != null)
        {
            actionPointsText.text = actionPoints.ToString();
        }
    }

    private void DisplayDistanceToMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            float distance = Vector3.Distance(transform.position, hit.point);
            int roundedDistance = Mathf.RoundToInt(distance);
            if (distanceText != null)
            {
                distanceText.text = roundedDistance.ToString();
            }
        }
    }
}
