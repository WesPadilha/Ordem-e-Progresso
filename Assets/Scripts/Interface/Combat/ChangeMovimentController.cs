using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMovimentController : MonoBehaviour
{
    public GameObject gridBase;
    public GameObject gameManager;
    public MainController mainController;
    public GameObject mouseViz;

    private bool isFirstActivation = true;

    // Método que alterna o estado da gridBase
    public void ToggleGrid()
    {
        if (gridBase.activeSelf)
        {
            // Limpa a grid e objetos relacionados
            GridBase gridBaseComponent = gridBase.GetComponent<GridBase>();
            if (gridBaseComponent != null)
            {
                gridBaseComponent.ClearLevel();
            }

            // Destroi as linhas
            if (GameManager.singleton.pathBlue != null)
            {
                DestroyImmediate(GameManager.singleton.pathBlue.gameObject);
            }
            if (GameManager.singleton.pathRed != null)
            {
                DestroyImmediate(GameManager.singleton.pathRed.gameObject);
            }
        }
        else
        {
            // Reativa e recria o grid
            GridBase gridBaseComponent = gridBase.GetComponent<GridBase>();
            if (gridBaseComponent != null && !isFirstActivation)
            {
                gridBaseComponent.InitPhase();
            }
        }

        // Alterna o estado dos objetos
        gridBase.SetActive(!gridBase.activeSelf);
        mouseViz.SetActive(!mouseViz.activeSelf);

        if (isFirstActivation)
        {
            if (mainController.agent != null && mainController.agent.enabled)
            {
                mainController.agent.isStopped = true;
            }
            isFirstActivation = false;
        }

        // Sincroniza as posições entre MainController e UnitController
        if (gridBase.activeSelf) // Ativando o UnitController
        {
            if (mainController != null && mainController.agent != null)
            {
                Vector3 playerPosition = mainController.agent.transform.position;
                UnitController playerUnit = GameManager.singleton.curUnit;

                if (playerUnit != null)
                {
                    playerUnit.x1 = Mathf.FloorToInt(playerPosition.x);
                    playerUnit.z1 = Mathf.FloorToInt(playerPosition.z);

                    Vector3 targetPosition = GridBase.singleton.GetWorldCoordinatesFromNode(playerUnit.x1, 0, playerUnit.z1);
                    playerUnit.transform.position = targetPosition;

                    // Desativa o MainController
                    mainController.agent.isStopped = true;
                    mainController.enabled = false;
                }
            }
        }
        else // Ativando o MainController
        {
            if (mainController != null)
            {
                UnitController playerUnit = GameManager.singleton.curUnit;

                if (playerUnit != null)
                {
                    Vector3 unitPosition = playerUnit.transform.position;

                    mainController.agent.Warp(unitPosition); // Ajusta a posição do NavMeshAgent
                    mainController.agent.isStopped = false;
                    mainController.enabled = true; // Reativa o MainController
                }
            }
        }
    }
}
