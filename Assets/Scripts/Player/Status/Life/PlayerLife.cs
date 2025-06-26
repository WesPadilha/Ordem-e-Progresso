using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerLife : MonoBehaviour
{
    public CharacterData characterData;
    public Slider lifeSlider;
    public TMP_Text lifeText;
    public AvoidDamageUI avoidDamageUI; 
    public bool IsInvulnerable { get; set; } = false;
    
    [Header("Game Over Settings")]
    public float gameOverDelay = 1.5f; // Tempo de delay antes de carregar a cena
    private bool isGameOverTriggered = false;
    private Coroutine gameOverCoroutine;

    void Start()
    {
        if (characterData != null)
        {
            characterData.Initialize();
            characterData.OnLifeChanged += OnCharacterLifeChanged;
            UpdateLifeUI();
        }
    }

    void OnDestroy()
    {
        if (characterData != null)
        {
            characterData.OnLifeChanged -= OnCharacterLifeChanged;
        }
        
        // Para a coroutine se o objeto for destruído
        if (gameOverCoroutine != null)
        {
            StopCoroutine(gameOverCoroutine);
        }
    }

    private void OnCharacterLifeChanged()
    {
        UpdateLifeUI();
        
        // Verifica se a vida chegou a zero e o game over ainda não foi acionado
        if (characterData.currentLife <= 0 && !isGameOverTriggered)
        {
            isGameOverTriggered = true;
            gameOverCoroutine = StartCoroutine(GameOverWithDelay());
        }
    }

    private IEnumerator GameOverWithDelay()
    {
        // Espera o tempo de delay antes de carregar a cena
        yield return new WaitForSeconds(gameOverDelay);
        
        // Verifica novamente se a vida ainda está zerada (para evitar casos de cura durante o delay)
        if (characterData.currentLife <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            isGameOverTriggered = false;
        }
    }

    public void UpdateLifeUI()
    {
        if (characterData == null) return;

        if (lifeSlider != null)
        {
            lifeSlider.maxValue = characterData.maxLife;
            lifeSlider.value = characterData.currentLife;
        }

        if (lifeText != null)
        {
            lifeText.text = $"{characterData.currentLife}/{characterData.maxLife}";
        }
    }

    public void TakeDamage(int damage)
    {
        if (characterData == null || IsInvulnerable) return;

        // Calcula chance de evitar dano baseado na sorte (luck)
        float luckAvoidanceChance = Mathf.Clamp(characterData.luck * 2f, 0f, 100f) / 100f;
        
        // Testa se o jogador teve sorte de evitar o dano completamente
        if (Random.Range(0f, 1f) < luckAvoidanceChance)
        {
            Debug.Log($"Sorte evitou todo o dano! (Chance: {luckAvoidanceChance * 100}%)");
            
            if (avoidDamageUI != null)
                avoidDamageUI.ShowAvoidMessage("Sorte! Você não tomou dano!");

            return;
        }

        // Converte defense em porcentagem e calcula redução
        float defenseReduction = Mathf.Clamp(characterData.defense, 0, 100) / 100f;
        int reducedDamage = Mathf.CeilToInt(damage * (1f - defenseReduction));

        // Garante dano mínimo de 1 se dano original > 0
        if (damage > 0 && reducedDamage < 1)
        {
            reducedDamage = 1;
        }

        characterData.SetCurrentLife(characterData.currentLife - reducedDamage);

        Debug.Log($"Player tomou {reducedDamage} de dano (Original: {damage}, Defesa: {characterData.defense}%)");
    }

    public void Heal(int amount)
    {
        if (characterData == null) return;

        int newLife = Mathf.Min(characterData.currentLife + amount, characterData.maxLife);
        int actualHeal = newLife - characterData.currentLife;

        characterData.SetCurrentLife(newLife);

        Debug.Log($"Curou {actualHeal} pontos de vida (Total: {characterData.currentLife}/{characterData.maxLife})");
        
        // Se curou durante o delay do game over, cancela o game over
        if (newLife > 0 && isGameOverTriggered)
        {
            if (gameOverCoroutine != null)
            {
                StopCoroutine(gameOverCoroutine);
            }
            isGameOverTriggered = false;
        }
    }

    public void RestoreFullLife()
    {
        if (characterData == null) return;

        characterData.SetCurrentLife(characterData.maxLife);
        
        // Se restaurou vida durante o delay do game over, cancela o game over
        if (isGameOverTriggered)
        {
            if (gameOverCoroutine != null)
            {
                StopCoroutine(gameOverCoroutine);
            }
            isGameOverTriggered = false;
        }
    }
}