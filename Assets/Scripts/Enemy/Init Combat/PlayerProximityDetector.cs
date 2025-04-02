using UnityEngine;

public class PlayerProximityDetector : MonoBehaviour
{
    public float detectionDistance = 10f;
    private bool hasTriggered = false;
    private bool isPlayerInRange = false;
    private Coroutine proximityCoroutine = null;

    public ChangeMovimentController changeMovimentController;
    public EnemyGroupController enemyGroupController;
    public EnemyController enemyController;

    private void Start()
    {
        if (enemyGroupController != null)
        {
            enemyGroupController.RegisterEnemy(this);
        }

        if (enemyController != null)
        {
            enemyController.SetActive(false);
        }
    }

    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && !hasTriggered && !enemyGroupController.groupActivated)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            bool inRange = distance <= detectionDistance;

            if (inRange && !isPlayerInRange)
            {
                isPlayerInRange = true;
                proximityCoroutine = StartCoroutine(WaitAndActivateGroup(3f));
            }
            else if (!inRange && isPlayerInRange)
            {
                isPlayerInRange = false;

                if (proximityCoroutine != null)
                {
                    StopCoroutine(proximityCoroutine);
                    proximityCoroutine = null;
                }
            }
        }
    }

    private System.Collections.IEnumerator WaitAndActivateGroup(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isPlayerInRange && !hasTriggered && !enemyGroupController.groupActivated)
        {
            hasTriggered = true;
            enemyGroupController.ActivateGroup(); // Ativa o grupo (pode ou não chamar ToggleGrid)

            if (enemyController != null)
            {
                enemyController.SetActive(true);
            }
        }
    }

    private void OnDestroy()
    {
        if (enemyGroupController != null)
        {
            enemyGroupController.EnemyDestroyed(this);
        }
    }
}