using System.Collections;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public GameObject storageUI;
    public ScreenController screenController;
    public Transform player;
    public Transform storage;
    public Camera storageCamera;
    public float interactionDistance = 3f;
    public float fixedDistance = 12f;
    public float movementSpeed = 5f;
    public GameObject hud;
    public CharacterData characterData;
    public int requiredArrombamento = 50;
    public ViewStorage viewStorage;

    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private bool hasGivenXP = false;

    public Animator storageAnimator; // ⬅ Referência ao Animator
    private readonly string openAnim = "Abrir";
    private readonly string closeAnim = "Fechar";

    private void OnMouseOver()
    {
        if (Pause.GameIsPaused || Vector3.Distance(player.position, transform.position) > interactionDistance || screenController.IsAnyUIActive())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            OpenStorage();
        }
    }

    public void OpenStorage()
    {
        if (characterData.arrombamento < requiredArrombamento)
        {
            Debug.Log("Você precisa de pelo menos " + requiredArrombamento + " de arrombamento para abrir este armazenamento.");

            if (viewStorage != null)
            {
                viewStorage.ShowInsufficientMessage(requiredArrombamento);
            }

            return;
        }

        if (!hasGivenXP && requiredArrombamento > 0)
        {
            ExperienceManager.Instance.AddExperience(25);
            hasGivenXP = true;
        }

        // Ativa parâmetro Abrir e desativa Fechar
        if (storageAnimator != null)
        {
            // Use as variáveis que você já declarou:
            storageAnimator.SetBool(openAnim, true);
            storageAnimator.SetBool(closeAnim, false);

        }

        storageUI.SetActive(true);
        screenController.SetStorageState(true);

        originalCameraPosition = storageCamera.transform.position;
        originalCameraRotation = storageCamera.transform.rotation;

        StartCoroutine(MoveCameraToStoragePosition());
        hud.SetActive(false);
    }

    public void CloseStorage()
    {
        // Ativa parâmetro Fechar e desativa Abrir
        if (storageAnimator != null)
        {
            storageAnimator.SetBool(closeAnim, true);
            storageAnimator.SetBool(openAnim, false);

        }

        storageUI.SetActive(false);
        screenController.SetStorageState(false);
        hud.SetActive(true);
    }

    private IEnumerator MoveCameraToStoragePosition()
    {
        float currentAngleY = storageCamera.transform.rotation.eulerAngles.y;
        Vector3 direction = new Vector3(Mathf.Sin(currentAngleY * Mathf.Deg2Rad), 0, Mathf.Cos(currentAngleY * Mathf.Deg2Rad));
        Vector3 targetPosition = storage.position - direction * fixedDistance;
        targetPosition.y = storageCamera.transform.position.y;

        while (Vector3.Distance(storageCamera.transform.position, targetPosition) > 0.1f)
        {
            storageCamera.transform.position = Vector3.Lerp(storageCamera.transform.position, targetPosition, movementSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && storageUI.activeSelf)
        {
            CloseStorage();
        }
    }
}
