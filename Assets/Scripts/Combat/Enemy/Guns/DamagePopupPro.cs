using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class DamagePopupPro : MonoBehaviour
{
    private Transform cameraTransform;
    private TMP_Text damageText;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private float fadeSpeed = 3f;
    [SerializeField] private float moveSpeedMultiplier = 2f;

    public static DamagePopupPro Create(Vector3 position, int damageAmount, bool isCritical, Transform parentCanvas = null)
    {
        GameObject popupGO = new GameObject("DamagePopup");

        // Se tiver um Canvas pai, adiciona o popup nele, senão cria Canvas próprio (não recomendado)
        if (parentCanvas != null)
        {
            popupGO.transform.SetParent(parentCanvas, false);
        }
        else
        {
            Canvas canvas = popupGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 999;
            popupGO.AddComponent<CanvasScaler>();
            popupGO.AddComponent<GraphicRaycaster>();
        }

        RectTransform rt = popupGO.GetComponent<RectTransform>();
        if (rt == null) rt = popupGO.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(1, 1);
        rt.localScale = Vector3.one * 0.01f;

        DamagePopupPro popup = popupGO.AddComponent<DamagePopupPro>();
        popup.cameraTransform = Camera.main.transform;

        popupGO.transform.position = position + new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(0.5f, 1.5f),
            0
        );

        popup.Setup(damageAmount, isCritical);
        return popup;
    }

    private void Awake()
    {
        GameObject textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(transform, false);

        damageText = textGO.AddComponent<TextMeshProUGUI>();
        damageText.alignment = TextAlignmentOptions.Center;
        damageText.fontStyle = FontStyles.Bold;
        damageText.raycastTarget = false;

        moveVector = new Vector3(Random.Range(-0.5f, 0.5f), 1, 0) * moveSpeedMultiplier;
    }

    public void Setup(int damageAmount, bool isCritical)
    {
        damageText.text = damageAmount.ToString();

        if (isCritical)
        {
            damageText.color = Color.red;
            damageText.fontSize = 150;
            moveVector *= 1.5f;

            damageText.enableVertexGradient = true;
            damageText.colorGradient = new VertexGradient(
                Color.red,
                new Color(1f, 0.5f, 0f),
                Color.yellow,
                new Color(1f, 0.8f, 0f)
            );
        }
        else
        {
            damageText.color = new Color(1f, 0.7f, 0.3f);
            damageText.fontSize = 100;
            damageText.enableVertexGradient = false;
        }

        damageText.fontMaterial.EnableKeyword("OUTLINE_ON");
        damageText.outlineWidth = 0.2f;
        damageText.outlineColor = Color.black;

        damageText.UpdateMeshPadding();
        damageText.SetVerticesDirty();

        textColor = damageText.color;
        disappearTimer = lifeTime;
    }

    private void LateUpdate()
    {
        if (cameraTransform != null)
        {
            transform.LookAt(transform.position + cameraTransform.forward);
        }

        transform.position += moveVector * Time.deltaTime;
        moveVector = Vector3.Lerp(moveVector, Vector3.zero, Time.deltaTime * 1f);

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            textColor.a -= fadeSpeed * Time.deltaTime;
            damageText.color = textColor;

            if (textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
