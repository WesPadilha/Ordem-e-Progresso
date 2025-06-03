using UnityEngine;
using TMPro;

public class ActionPointCostUI : MonoBehaviour
{
    public Camera mainCamera;
    public Transform player;
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    // Referência para o gerenciador de combate
    public CombatStateManager combatStateManager;

    void Update()
    {
        // Verifica se está em combate
        if (!combatStateManager.IsPlayerInCombat())
        {
            // Não está em combate -> esconde tooltip e sai do Update
            if (tooltipPanel.activeSelf)
                tooltipPanel.SetActive(false);
            return;
        }

        // Está em combate -> ativa tooltip se não estiver ativo
        if (!tooltipPanel.activeSelf)
            tooltipPanel.SetActive(true);

        // Ray do mouse
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        Vector3 targetPoint = Vector3.zero;
        bool foundGround = false;

        // Procura pelo chão (Tag "Ground") entre os hits
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Ground"))
            {
                targetPoint = hit.point;
                foundGround = true;
                break;
            }
        }

        // Se encontrou o chão
        if (foundGround)
        {
            // Calcula a distância
            float distance = Vector3.Distance(player.position, targetPoint);

            // Calcula PA (1 metro = 1 PA)
            int pa = Mathf.FloorToInt(distance);

            // Atualiza o texto
            tooltipText.text = pa.ToString();

            // Faz o painel seguir o mouse
            tooltipPanel.transform.position = Input.mousePosition;
        }
        else
        {
            // Mostra PA como "--"
            tooltipText.text = "--";
            tooltipPanel.transform.position = Input.mousePosition;
        }
    }
}
