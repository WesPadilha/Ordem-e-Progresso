using UnityEngine;
using TMPro;

public class ActionPointCostUI : MonoBehaviour
{
    public Camera mainCamera;
    public Transform player;
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    public CombatStateManager combatStateManager;

    void Update()
    {
        // Verifica se está em combate
        if (!combatStateManager.IsPlayerInCombat())
        {
            if (tooltipPanel.activeSelf)
                tooltipPanel.SetActive(false);
            return;
        }

        if (!tooltipPanel.activeSelf)
            tooltipPanel.SetActive(true);

        // Ray do mouse
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        Vector3 targetPoint = Vector3.zero;
        bool foundEnemy = false;
        bool foundGround = false;

        // Verifica se está no modo de mira
        ShootButtonController shootButton = FindObjectOfType<ShootButtonController>();
        bool isAiming = shootButton != null && shootButton.IsAiming();

        if (isAiming)
        {
            // Procura inimigo primeiro
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    targetPoint = hit.point;
                    foundEnemy = true;

                    WeaponController weapon = FindObjectOfType<WeaponController>();
                    if (weapon != null)
                    {
                        // Mostra custo fixo de PA da arma
                        tooltipText.text = weapon.GetActionPointCost().ToString();
                        tooltipPanel.transform.position = Input.mousePosition;
                    }
                    break;
                }
            }
        }

        if (!foundEnemy)
        {
            // Procura o chão
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    targetPoint = hit.point;
                    foundGround = true;
                    break;
                }
            }

            if (foundGround)
            {
                // Calcula distância até o ponto no chão
                float distance = Vector3.Distance(player.position, targetPoint);
                int pa = Mathf.FloorToInt(distance);

                tooltipText.text = pa.ToString();
                tooltipPanel.transform.position = Input.mousePosition;
            }
            else
            {
                // Se não encontrou nada
                tooltipText.text = "--";
                tooltipPanel.transform.position = Input.mousePosition;
            }
        }
    }
}
