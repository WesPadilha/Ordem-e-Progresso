using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DetectionImage : MonoBehaviour
{
    [Header("References")]
    public Image targetImage; // Reference to the UI Image
    public float detectionRadius = 10f; // Detection radius (should match EnemyGroupController)
    
    [Header("Fill Settings")]
    public Color startColor = Color.white;
    public Color fillColor = Color.red;
    public float fillDuration = 3f; // Time to fill the image
    
    private EnemyGroupController enemyGroupController;
    private Transform player;
    private float currentFillAmount = 0f;
    private Coroutine fillCoroutine;
    private bool isPlayerInRange = false;

    void Start()
    {
        enemyGroupController = GetComponentInParent<EnemyGroupController>();
        if (enemyGroupController == null)
        {
            Debug.LogError("EnemyGroupController not found in parent hierarchy");
            return;
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found in scene");
            return;
        }

        if (targetImage == null)
        {
            Debug.LogError("Target Image not assigned");
            return;
        }

        InitializeImage();
    }

    void Update()
    {
        if (enemyGroupController == null || player == null || targetImage == null || enemyGroupController.IsGroupChasing())
            return;

        CheckPlayerDistance();
    }

    private void InitializeImage()
    {
        targetImage.color = startColor;
        targetImage.fillAmount = 0f;
        targetImage.gameObject.SetActive(false);
    }

    private void CheckPlayerDistance()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        bool wasPlayerInRange = isPlayerInRange;
        isPlayerInRange = distance <= detectionRadius;

        // State changed
        if (isPlayerInRange != wasPlayerInRange)
        {
            if (isPlayerInRange)
            {
                StartFill();
            }
            else
            {
                StopFill();
            }
        }
    }

    private void StartFill()
    {
        if (fillCoroutine != null)
            StopCoroutine(fillCoroutine);

        targetImage.gameObject.SetActive(true);
        fillCoroutine = StartCoroutine(FillImageCoroutine());
    }

    private void StopFill()
    {
        if (fillCoroutine != null)
            StopCoroutine(fillCoroutine);

        fillCoroutine = StartCoroutine(ReverseFillCoroutine());
    }

    private IEnumerator FillImageCoroutine()
    {
        float startTime = Time.time;
        float startFill = currentFillAmount;
        float endFill = 1f;

        while (currentFillAmount < endFill)
        {
            float progress = (Time.time - startTime) / fillDuration;
            currentFillAmount = Mathf.Lerp(startFill, endFill, progress);
            targetImage.fillAmount = currentFillAmount;
            targetImage.color = Color.Lerp(startColor, fillColor, currentFillAmount);
            yield return null;
        }

        // When fill completes, start the chase
        if (!enemyGroupController.IsGroupChasing())
        {
            enemyGroupController.StartGroupChase();
        }

        // Hide the image after chase starts
        yield return new WaitForSeconds(0.5f);
        targetImage.gameObject.SetActive(false);
    }

    private IEnumerator ReverseFillCoroutine()
    {
        float startTime = Time.time;
        float startFill = currentFillAmount;
        float endFill = 0f;

        while (currentFillAmount > endFill)
        {
            float progress = (Time.time - startTime) / fillDuration;
            currentFillAmount = Mathf.Lerp(startFill, endFill, progress);
            targetImage.fillAmount = currentFillAmount;
            targetImage.color = Color.Lerp(startColor, fillColor, currentFillAmount);
            yield return null;
        }

        targetImage.gameObject.SetActive(false);
    }
}