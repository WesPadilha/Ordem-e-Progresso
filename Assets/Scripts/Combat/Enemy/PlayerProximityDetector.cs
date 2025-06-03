using UnityEngine;

public class PlayerProximityDetector : MonoBehaviour
{
    public float detectionRadius = 10f;
    public float timeToStartChasing = 3f;

    private GameObject player;
    private EnemyGroupController groupController;
    private float timePlayerInside = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        groupController = GetComponentInParent<EnemyGroupController>();

        if (player == null)
            Debug.LogWarning("Player não encontrado na cena com tag 'Player'");

        if (groupController == null)
            Debug.LogWarning("EnemyGroupController não encontrado no objeto pai");
    }

    void Update()
    {
        if (player == null || groupController == null)
            return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= detectionRadius)
        {
            timePlayerInside += Time.deltaTime;

            if (timePlayerInside >= timeToStartChasing && !groupController.IsGroupChasing())
            {
                groupController.StartGroupChase();
            }
        }
        else
        {
            timePlayerInside = 0f;
        }
    }
}