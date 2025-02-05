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
        // Verifique se gridBase não é nulo antes de acessá-lo
        if (gridBase != null)
        {
            if (gridBase.activeSelf)
            {
                // Limpa a grid e objetos relacionados
                GridBase gridBaseComponent = gridBase.GetComponent<GridBase>();
                if (gridBaseComponent != null)
                {
                    gridBaseComponent.ClearLevel();
                }

                // Destroi as linhas, verificando se GameManager.singleton não é nulo
                if (GameManager.singleton != null)
                {
                    if (GameManager.singleton.pathBlue != null)
                    {
                        DestroyImmediate(GameManager.singleton.pathBlue.gameObject);
                    }
                    if (GameManager.singleton.pathRed != null)
                    {
                        DestroyImmediate(GameManager.singleton.pathRed.gameObject);
                    }
                }

                // Reinicia a condição de levelHasInited para garantir que o nível seja carregado novamente
                if (LevelEditor.singleton != null)
                {
                    LevelEditor.singleton.levelHasInited = false; // Resetando a variável
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
        }

        // Sincroniza as posições entre MainController e UnitController
        if (gridBase != null && gridBase.activeSelf) // Ativando o UnitController
        {
            if (mainController != null && mainController.agent != null)
            {
                // Parando imediatamente o movimento do agente do MainController
                if (mainController.agent.isOnNavMesh)
                {
                    mainController.agent.isStopped = true; // Impede o movimento
                    mainController.agent.velocity = Vector3.zero; // Resetando a velocidade do agente
                }

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

            // Resetando actionPoints ao ativar a grid
            if (GameManager.singleton != null && GameManager.singleton.curUnit != null)
            {
                GameManager.singleton.curUnit.EndTurn();
            }

            // Chama o LoadLevel com atraso de 1 segundo
            if (LevelEditor.singleton != null && !LevelEditor.singleton.levelHasInited)
            {
                StartCoroutine(DelayedLoadLevel());
            }
        }
        else if (mainController != null && !gridBase.activeSelf) // Ativando o MainController
        {
            UnitController playerUnit = GameManager.singleton != null ? GameManager.singleton.curUnit : null;

            if (playerUnit != null)
            {
                Vector3 unitPosition = playerUnit.transform.position;

                mainController.agent.Warp(unitPosition); // Ajusta a posição do NavMeshAgent
                mainController.agent.isStopped = false;
                mainController.enabled = true; // Reativa o MainController
            }
        }

        if (isFirstActivation)
        {
            if (mainController != null && mainController.agent != null)
            {
                mainController.agent.isStopped = true;
            }
            isFirstActivation = false;
        }
    }

    // Função de Coroutine que espera 1 segundo antes de chamar o LoadLevel
    private IEnumerator DelayedLoadLevel()
    {
        yield return new WaitForSeconds(0.5f);
        LevelEditor.singleton.LoadLevel();
    }

    void OnDestroy()
    {
        if (gridBase != null && gridBase.activeSelf)
        {
            Destroy(gridBase);
        }
    }
}
