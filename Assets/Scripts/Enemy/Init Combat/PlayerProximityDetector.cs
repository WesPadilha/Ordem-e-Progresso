using UnityEngine;

public class PlayerProximityDetector : MonoBehaviour
{
    public float detectionDistance = 10f;
    private bool hasTriggered = false;
    private bool isPlayerInRange = false;
    private Coroutine proximityCoroutine = null;

    public ChangeMovimentController changeMovimentController;
    public EnemyGroupController enemyGroupController;

    private void Start()
    {
        if (enemyGroupController != null)
        {
            enemyGroupController.RegisterEnemy(this);
        }
    }

    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && !hasTriggered)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            bool inRange = distance <= detectionDistance;

            if (inRange && !isPlayerInRange)
            {
                // Jogador entrou no alcance
                isPlayerInRange = true;
                proximityCoroutine = StartCoroutine(WaitAndTriggerGrid(3f));
            }
            else if (!inRange && isPlayerInRange)
            {
                // Jogador saiu do alcance
                isPlayerInRange = false;

                if (proximityCoroutine != null)
                {
                    StopCoroutine(proximityCoroutine);
                    proximityCoroutine = null;
                }
            }
        }
    }

    private System.Collections.IEnumerator WaitAndTriggerGrid(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isPlayerInRange && !hasTriggered) // Verifica novamente se o jogador ainda está no alcance
        {
            hasTriggered = true;

            if (changeMovimentController != null)
            {
                changeMovimentController.ToggleGrid();
            }

            if (enemyGroupController != null)
            {
                enemyGroupController.ActivateAllEnemies();
            }
        }
    }

    private void OnDestroy()
    {
        // Notifica o controlador de grupo quando este inimigo é destruído
        if (enemyGroupController != null)
        {
            enemyGroupController.EnemyDestroyed(this);
        }
    }

    public void SetHasTriggered(bool value)
    {
        hasTriggered = value;
    }
}
