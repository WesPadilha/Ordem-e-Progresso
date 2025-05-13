using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLife : MonoBehaviour
{
    public CharacterData characterData;
    public Slider lifeSlider;
    public TMP_Text lifeText;
    public bool IsInvulnerable { get; set; } = false;

    void Start()
    {
        if (characterData != null)
        {
            characterData.Initialize(); // Configura vida máxima no início
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
    }

    private void OnCharacterLifeChanged()
    {
        UpdateLifeUI();
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

        characterData.SetCurrentLife(characterData.currentLife - damage);
        
        // Adicione feedback visual/auditivo se desejar
        Debug.Log("Player tomou dano: " + damage);
    }

    // No script PlayerLife, atualize o método Heal:
    public void Heal(int amount)
    {
        if (characterData == null) return;

        // Calcula a cura respeitando o máximo de vida
        int newLife = Mathf.Min(characterData.currentLife + amount, characterData.maxLife);
        int actualHeal = newLife - characterData.currentLife;
        
        characterData.SetCurrentLife(newLife);
        
        Debug.Log($"Curou {actualHeal} pontos de vida (Total: {characterData.currentLife}/{characterData.maxLife})");
    }

    public void RestoreFullLife()
    {
        if (characterData == null) return;

        characterData.SetCurrentLife(characterData.maxLife);
    }
}