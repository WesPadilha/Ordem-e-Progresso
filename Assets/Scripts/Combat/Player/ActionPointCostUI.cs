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
        // Primeiro verifica se está em combate
        bool inCombat = combatStateManager != null && combatStateManager.IsPlayerInCombat();
        
        // Se não está em combate, desativa o painel e sai
        if (!inCombat)
        {
            if (tooltipPanel.activeSelf)
                tooltipPanel.SetActive(false);
            return;
        }

        // Ativa o painel se estiver em combate e não estiver ativo
        if (!tooltipPanel.activeSelf)
            tooltipPanel.SetActive(true);

        // Lógica para quando está em combate
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        Vector3 targetPoint = Vector3.zero;
        bool foundEnemy = false;
        bool foundGround = false;

        ShootButtonController shootButton = FindObjectOfType<ShootButtonController>();
        bool isAiming = shootButton != null && shootButton.IsAiming();

        if (isAiming)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    targetPoint = hit.point;
                    foundEnemy = true;

                    WeaponController weapon = FindObjectOfType<WeaponController>();
                    if (weapon != null)
                    {
                        tooltipText.text = weapon.GetActionPointCost().ToString();
                        tooltipPanel.transform.position = Input.mousePosition;
                    }
                    break;
                }
            }
        }

        if (!foundEnemy)
        {
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
                float distance = Vector3.Distance(player.position, targetPoint);
                int pa = Mathf.FloorToInt(distance);
                tooltipText.text = pa.ToString();
                tooltipPanel.transform.position = Input.mousePosition;
            }
            else
            {
                tooltipText.text = "--";
                tooltipPanel.transform.position = Input.mousePosition;
            }
        }
    }
}