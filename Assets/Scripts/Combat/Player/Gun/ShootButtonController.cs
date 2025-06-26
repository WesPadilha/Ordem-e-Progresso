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
    private CombatStatusChecker combatStatusChecker;
    private WeaponRangeRing weaponRangeRing;


    void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player");
        combatStatusChecker = FindObjectOfType<CombatStatusChecker>();
        weaponRangeRing = FindObjectOfType<WeaponRangeRing>();

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
                    aimingMode = false;

                    if (currentShootCoroutine != null)
                    {
                        StopCoroutine(currentShootCoroutine);
                    }

                    Vector3 targetCenter = hit.collider.bounds.center + Vector3.up * 0.1f;
                    currentShootCoroutine = StartCoroutine(RotateAndShoot(targetCenter, hit.collider.gameObject));
                }
                else
                {
                    CancelAimingMode(); // Novo método para encapsular cancelamento
                }
            }
            else
            {
                CancelAimingMode(); // Também cancela se não atingir nada
            }
        }
    }

    public void OnButtonClick()
    {
        // Verifica se o player ainda existe
        if (player == null)
        {
            Debug.LogWarning("Player não encontrado.");
            return;
        }

        // Verifica se há uma arma equipada
        WeaponController weapon = FindObjectOfType<WeaponController>();
        if (weapon == null)
        {
            Debug.Log("Nenhuma arma equipada.");
            return;
        }

        // Se estiver em combate, verifica pontos de ação
        if (combatStatusChecker != null && combatStatusChecker.IsInCombat())
        {
            MovimentCombat playerMovement = player.GetComponent<MovimentCombat>();
            if (playerMovement == null)
            {
                Debug.LogWarning("Componente MovimentCombat não encontrado no player.");
                return;
            }

            if (playerMovement.GetCurrentActionPoints() < weapon.GetActionPointCost())
            {
                Debug.Log("Pontos de ação insuficientes para atirar.");
                return;
            }
        }

        // Cancela qualquer ação de disparo em andamento
        if (currentShootCoroutine != null)
        {
            StopCoroutine(currentShootCoroutine);
            currentShootCoroutine = null;
        }

        aimingMode = true;
        Debug.Log("Modo de mira ativado. Clique em um inimigo para atirar.");

        if (weaponRangeRing != null)
        {
            weaponRangeRing.SetTargetAndRange(player.transform, weaponTypeToRange(weapon.weaponType));
        }
    }

    private float weaponTypeToRange(WeaponController.WeaponType type)
    {
        return type == WeaponController.WeaponType.LongRange ? 15f : 10f;
    }

    public bool IsAiming()
    {
        return aimingMode;
    }
    
    private void CancelAimingMode()
    {
        aimingMode = false;

        if (weaponRangeRing != null)
        {
            weaponRangeRing.Hide();
        }

        Debug.Log("Modo de mira cancelado.");
    }


    private IEnumerator RotateAndShoot(Vector3 targetPoint, GameObject enemyHit = null)
    {
        if (player == null)
            yield break;

        WeaponController weapon = FindObjectOfType<WeaponController>();
        if (weapon == null)
        {
            Debug.Log("Arma não encontrada durante o disparo.");
            aimingMode = false;
            yield break;
        }

        // Verifica o alcance antes de qualquer ação, mesmo fora de combate
        if (!weapon.CanShoot(targetPoint))
        {
            Debug.Log("Alvo fora do alcance da arma.");
            aimingMode = false;
            yield break;
        }

        // Verifica se a arma tem munição, qualquer coisa comenta esta linha
        if (weapon.weaponLoader == null || weapon.weaponLoader.ammoCurrent <= 0)
        {
            Debug.Log("Sem munição. Disparo cancelado.");
            aimingMode = false;
            yield break;
        }

        MovimentCombat playerMovement = player.GetComponent<MovimentCombat>();
        if (playerMovement == null)
            yield break;

        // Para garantir que o player esteja parado
        playerMovement.StopMovement();

        // Espera um frame para garantir que o movimento foi parado
        yield return null;

        // Se estiver em combate, verifica pontos de ação
        if (combatStatusChecker != null && combatStatusChecker.IsInCombat())
        {
            int paCost = weapon.GetActionPointCost();
            if (playerMovement.GetCurrentActionPoints() < paCost)
            {
                Debug.Log("Pontos de ação insuficientes para atirar.");
                yield break;
            }
        }

        // Calcula direção para o alvo (ignorando diferença de altura)
        Vector3 direction = (targetPoint - player.transform.position).normalized;
        direction.y = 0f;

        // Rotaciona o jogador para o alvo
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float rotationSpeed = 10f;
        float rotationThreshold = 1f;

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
            yield return null;
        }

        // Se estiver em combate, gasta pontos de ação
        if (combatStatusChecker != null && combatStatusChecker.IsInCombat())
        {
            playerMovement.SpendActionPoints(weapon.GetActionPointCost());
        }

        yield return new WaitForSeconds(.08f);

        // Dispara
        weapon.Shoot(targetPoint);

        // Se acertou um inimigo e não está em combate, inicia o combate
        if (enemyHit != null && (combatStatusChecker == null || !combatStatusChecker.IsInCombat()))
        {
            EnemyChase enemy = enemyHit.GetComponent<EnemyChase>();
            if (enemy != null)
            {
                EnemyGroupController group = enemy.GetComponentInParent<EnemyGroupController>();
                if (group != null)
                {
                    group.StartGroupChase();
                }
            }
        }

        // Espera um tempo mínimo para a animação
        yield return new WaitForSeconds(0.5f);

        // Desativa animação de ataque
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("Attack", false);
        }

        aimingMode = false;
        currentShootCoroutine = null;

        if (weaponRangeRing != null)
        {
            weaponRangeRing.Hide();
        }
    }
}