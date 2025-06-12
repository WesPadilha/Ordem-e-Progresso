using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WeaponRangeRing : MonoBehaviour
{
    public int segments = 60;
    public float width = 0.05f;
    public Color ringColor = Color.yellow;

    private LineRenderer lineRenderer;
    private float currentRange = 1f;
    private Transform target;
    private Vector3 offset = new Vector3(0, 0.1f, 0);
    private bool isVisible = false;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = segments + 1;
        lineRenderer.widthMultiplier = width;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = ringColor;
        lineRenderer.endColor = ringColor;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (isVisible && target != null)
        {
            transform.position = target.position + offset;
        }
    }

    public void SetTargetAndRange(Transform targetTransform, float range)
    {
        target = targetTransform;
        currentRange = range;
        CreateRing(range);
        Show();
    }

    private void CreateRing(float radius)
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

    public void Show()
    {
        isVisible = true;
        lineRenderer.enabled = true;
    }

    public void Hide()
    {
        isVisible = false;
        lineRenderer.enabled = false;
    }
}
