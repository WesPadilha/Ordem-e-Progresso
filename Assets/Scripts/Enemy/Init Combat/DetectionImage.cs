using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectionImage : MonoBehaviour
{
    [Header("Referências")]
    public Image targetImage; // Referência para a imagem
    public Transform player;  // Referência para o jogador
    public EnemyGroupController enemyGroupController; // Referência ao controlador de grupo de inimigos

    [Header("Cores")]
    public Color startColor = Color.white; // Cor inicial
    public Color fillColor = Color.red;   // Cor que irá preencher

    [Header("Configurações")]
    public float duration = 3f; // Duração do preenchimento em segundos
    public float detectionRadius = 10f; // Raio de detecção (10 metros)

    private float elapsedTime;
    private float currentFillAmount = 0f; // Variável para controlar o progresso de preenchimento
    private bool isInsideDetectionArea = false;
    private bool hasTriggered = false; // Para garantir que a imagem só seja acionada uma vez
    private Coroutine currentCoroutine; // Para gerenciar a execução da Coroutine

    void Start()
    {
        if (targetImage == null)
        {
            Debug.LogError("A imagem alvo não foi atribuída.");
            return;
        }

        if (player == null)
        {
            Debug.LogError("A referência para o jogador não foi atribuída.");
            return;
        }

        // Define a cor inicial e configura o preenchimento inicial
        targetImage.color = startColor;
        targetImage.fillAmount = currentFillAmount; // Começa com o preenchimento inicial (0)
        targetImage.gameObject.SetActive(false); // Começa invisível
    }

    private void Update()
    {
        if (hasTriggered || enemyGroupController.groupActivated) return; // Não faz nada se o grupo já foi ativado

        bool isAnyEnemyInRange = false;

        foreach (var enemy in enemyGroupController.enemiesInGroup)
        {
            if (enemy != null && !enemyGroupController.groupActivated)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                if (distanceToPlayer <= detectionRadius)
                {
                    isAnyEnemyInRange = true;
                    break;
                }
            }
        }

        if (isAnyEnemyInRange)
        {
            if (!isInsideDetectionArea)
            {
                isInsideDetectionArea = true;
                if (currentCoroutine != null) StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(FillImage());
            }
        }
        else
        {
            if (isInsideDetectionArea)
            {
                isInsideDetectionArea = false;
                if (currentCoroutine != null) StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(ReverseFillImage());
            }
        }
    }

    // Preenche a imagem
    private IEnumerator FillImage()
    {
        targetImage.gameObject.SetActive(true); // Torna a imagem visível
        float startFillAmount = currentFillAmount; // Salva o valor inicial do preenchimento
        elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            currentFillAmount = Mathf.Clamp01(startFillAmount + (elapsedTime / duration)); // Calcula o novo valor de fillAmount
            targetImage.fillAmount = currentFillAmount;

            // Mantém o efeito de preenchimento enquanto sobe
            targetImage.color = Color.Lerp(startColor, fillColor, currentFillAmount);
            yield return null;
        }

        currentFillAmount = 1f; // Garantir que o valor final seja 100%
        hasTriggered = true; // Garante que a imagem não será acionada novamente
        yield return new WaitForSeconds(1f); // Espera um pouco antes de desativar a imagem
        targetImage.gameObject.SetActive(false); // Desativa a imagem após 3 segundos
    }

    // Reverte o preenchimento da imagem
    private IEnumerator ReverseFillImage()
    {
        float startFillAmount = currentFillAmount; // Salva o valor atual de fillAmount
        elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            currentFillAmount = Mathf.Clamp01(startFillAmount - (elapsedTime / duration)); // Reverte o valor de fillAmount
            targetImage.fillAmount = currentFillAmount;

            // Mantém o efeito de retorno à cor inicial
            targetImage.color = Color.Lerp(startColor, fillColor, currentFillAmount);
            yield return null;
        }

        targetImage.gameObject.SetActive(false); // Desativa a imagem após o retorno completo
        currentFillAmount = 0f; // Garante que o valor final seja 0%
    }
}
