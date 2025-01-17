using UnityEngine;

public class PlayerProximityDetector : MonoBehaviour
{
    public float detectionDistance = 10f;
    private bool hasTriggered = false;
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

            if (distance <= detectionDistance)
            {
                hasTriggered = true;
                if (changeMovimentController != null) // Verifique se é nulo antes de chamar
                {
                    changeMovimentController.ToggleGrid();
                }
                if (enemyGroupController != null) // Verifique se é nulo antes de chamar
                {
                    enemyGroupController.ActivateAllEnemies();
                }
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
