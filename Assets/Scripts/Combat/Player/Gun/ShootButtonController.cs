using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ShootButtonController : MonoBehaviour
{
    private Camera mainCamera;
    private bool aimingMode = false;
    private GameObject player;
    private Animator playerAnimator;
    private Coroutine currentShootCoroutine;

    void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerAnimator = player.GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (aimingMode && Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    aimingMode = false; // Desativa modo de mira imediatamente
                    
                    // Cancela qualquer corotina de disparo anterior
                    if (currentShootCoroutine != null)
                    {
                        StopCoroutine(currentShootCoroutine);
                    }
                    
                    currentShootCoroutine = StartCoroutine(RotateAndShoot(hit.point));
                }
            }
        }
    }

    public void OnButtonClick()
    {
        // Cancela qualquer ação de disparo em andamento
        if (currentShootCoroutine != null)
        {
            StopCoroutine(currentShootCoroutine);
            currentShootCoroutine = null;
        }
        
        aimingMode = true;
        Debug.Log("Modo de mira ativado. Clique em um inimigo para atirar.");
    }

    public bool IsAiming()
    {
        return aimingMode;
    }

    private IEnumerator RotateAndShoot(Vector3 targetPoint)
    {
        if (player == null)
            yield break;

        WeaponController weapon = FindObjectOfType<WeaponController>();
        if (weapon == null)
            yield break;

        MovimentCombat playerMovement = player.GetComponent<MovimentCombat>();
        if (playerMovement == null)
            yield break;

        // Para garantir que o player esteja parado
        playerMovement.StopMovement();

        // Espera um frame para garantir que o movimento foi parado
        yield return null;

        int paCost = weapon.GetActionPointCost();
        if (playerMovement.GetCurrentActionPoints() < paCost)
        {
            Debug.Log("Pontos de ação insuficientes para atirar.");
            yield break;
        }

        if (!weapon.CanShoot(targetPoint))
        {
            Debug.Log("Alvo fora do alcance da arma. Não será gasto PA nem disparado.");
            yield break;
        }

        // Calcula direção para o alvo (ignorando diferença de altura)
        Vector3 direction = (targetPoint - player.transform.position).normalized;
        direction.y = 0f;

        // Rotaciona o jogador para o alvo
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float rotationSpeed = 10f; // Velocidade mais rápida para rotação
        float rotationThreshold = 1f; // Limiar de diferença para considerar rotação completa

        while (Quaternion.Angle(player.transform.rotation, targetRotation) > rotationThreshold)
        {
            player.transform.rotation = Quaternion.Slerp(
                player.transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime);
            yield return null;
        }

        // Garante rotação exata no final
        player.transform.rotation = targetRotation;

        // Ativa animação de ataque
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("Attack", true);
            // Espera um frame para garantir que a animação foi ativada
            yield return null;
        }

        // Gasta pontos de ação e dispara
        playerMovement.SpendActionPoints(paCost);
        weapon.Shoot(targetPoint);

        // Espera um tempo mínimo para a animação
        yield return new WaitForSeconds(0.5f);

        // Desativa animação de ataque
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("Attack", false);
        }

        currentShootCoroutine = null;
    }
}