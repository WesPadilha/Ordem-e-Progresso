using System.Collections;
using UnityEngine;

public class NPCOrientationController : MonoBehaviour
{
    public Transform player; // Referência ao jogador
    private Quaternion originalRotation; // Armazena a rotação inicial do NPC
    private Coroutine currentRotationCoroutine = null; // Referência para a coroutine de rotação ativa
    public float rotationSpeed = 5f; // Velocidade da rotação

    void Start()
    {
        // Salva a rotação inicial do NPC
        originalRotation = transform.rotation;
    }

    public void StartConversation()
    {
        // Interrompe qualquer rotação ativa
        if (currentRotationCoroutine != null)
        {
            StopCoroutine(currentRotationCoroutine);
        }

        // Inicia a rotação para o jogador
        currentRotationCoroutine = StartCoroutine(RotateTowards(player));
    }

    public void EndConversation()
    {
        // Interrompe qualquer rotação ativa
        if (currentRotationCoroutine != null)
        {
            StopCoroutine(currentRotationCoroutine);
        }

        // Retorna à rotação original
        currentRotationCoroutine = StartCoroutine(RotateTowardsOriginal());
    }

    private IEnumerator RotateTowards(Transform target)
    {
        while (true)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            if (direction.magnitude < 0.01f)
                break;

            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Condição de parada
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                break;
            }

            yield return null;
        }

        currentRotationCoroutine = null;
    }

    private IEnumerator RotateTowardsOriginal()
    {
        while (Quaternion.Angle(transform.rotation, originalRotation) > 0.1f)
        {
            // Interpola suavemente entre a rotação atual e a rotação original
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }

        // Garante precisão ao final da rotação
        transform.rotation = originalRotation;

        // Libera a referência à coroutine
        currentRotationCoroutine = null;
    }
}
