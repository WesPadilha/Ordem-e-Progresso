using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TurnIndicatorRing : MonoBehaviour
{
    [Header("Settings")]
    public float radius = 1.5f;
    public float width = 0.2f;
    public int segments = 36;

    [Header("Colors")]
    public Color playerColor = Color.blue;
    public Color enemyColor = Color.red;
    public Color defaultColor = new Color(1f, 0.5f, 0f, 1f); // Laranja forte sem transparência

    private LineRenderer lineRenderer;
    private TurnManager turnManager;
    private Transform currentTarget;
    private bool isActive = false;
    private Color currentBaseColor;
    private Vector3 offset = new Vector3(0f, 0.1f, 0f);

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Material com shader que exibe bem a cor
        Material lineMaterial = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material = lineMaterial;
        lineRenderer.material.color = Color.white; // Garante que a cor do LineRenderer seja controlada via código

        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = segments + 1;
        lineRenderer.widthMultiplier = width;
        lineRenderer.enabled = false;

        turnManager = FindObjectOfType<TurnManager>();
        if (turnManager == null)
        {
            Debug.LogWarning("TurnManager não encontrado na cena.");
            enabled = false;
            return;
        }

        CreateRing(); // Gera o anel uma única vez
        SetDefaultColor();
    }

    void OnEnable()
    {
        turnManager.OnTurnChanged += HandleTurnChange;
    }

    void OnDisable()
    {
        if (turnManager != null)
            turnManager.OnTurnChanged -= HandleTurnChange;
    }

    void Update()
    {
        if (!isActive || currentTarget == null) return;

        // Seguir o alvo com leve deslocamento vertical
        transform.position = currentTarget.position + offset;

        // Mantém cor sólida (sem alteração por tempo)
        lineRenderer.startColor = currentBaseColor;
        lineRenderer.endColor = currentBaseColor;
    }

    private void HandleTurnChange(TurnManager.TurnPhase phase, GameObject activeCharacter)
    {
        Debug.Log($"[TurnIndicatorRing] Turn Changed: {phase}, Character: {activeCharacter?.name}");

        if (activeCharacter == null)
        {
            SetDefaultColor();
            return;
        }

        currentTarget = activeCharacter.transform;
        isActive = true;
        lineRenderer.enabled = true;

        // Define cor sólida para o turno atual
        currentBaseColor = (phase == TurnManager.TurnPhase.Player ? playerColor : enemyColor);

        lineRenderer.startColor = currentBaseColor;
        lineRenderer.endColor = currentBaseColor;
    }

    private void CreateRing()
    {
        float angleStep = 360f / segments;
        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * angleStep * i;
            float x = Mathf.Sin(angle) * radius;
            float z = Mathf.Cos(angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
        }
    }

    private void SetDefaultColor()
    {
        isActive = false;
        currentTarget = null;
        currentBaseColor = defaultColor;

        lineRenderer.startColor = currentBaseColor;
        lineRenderer.endColor = currentBaseColor;
        lineRenderer.enabled = true; // Anel visível fora de combate
    }
}
