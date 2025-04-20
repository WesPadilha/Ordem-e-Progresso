using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLife : MonoBehaviour
{
    public CharacterData characterData; // Referência ao ScriptableObject com os dados
    public Slider lifeSlider; // Referência ao Slider de vida
    public TMP_Text lifeText; // Referência ao Texto de vida

    private int currentLife;

    // Start is called before the first frame update
    void Start()
    {
        if (characterData != null)
        {
            currentLife = characterData.maxLife; // Inicializa com vida máxima
        }
        UpdateLifeUI();
    }

    // Método para atualizar a UI de vida
    public void UpdateLifeUI()
    {
        if (characterData != null)
        {
            // Atualiza o slider
            if (lifeSlider != null)
            {
                lifeSlider.maxValue = characterData.maxLife;
                lifeSlider.value = currentLife;
            }

            // Atualiza o texto no formato "current/max"
            if (lifeText != null)
            {
                lifeText.text = $"{currentLife}/{characterData.maxLife}";
            }
        }
    }

    // Método para causar dano ao jogador
    public void TakeDamage(int damage)
    {
        if (characterData != null)
        {
            currentLife -= damage;
            currentLife = Mathf.Clamp(currentLife, 0, characterData.maxLife);
            UpdateLifeUI();
        }
    }

    // Método para curar o jogador
    public void Heal(int amount)
    {
        if (characterData != null)
        {
            currentLife += amount;
            currentLife = Mathf.Clamp(currentLife, 0, characterData.maxLife);
            UpdateLifeUI();
        }
    }

    // Método para restaurar vida completamente
    public void RestoreFullLife()
    {
        if (characterData != null)
        {
            currentLife = characterData.maxLife;
            UpdateLifeUI();
        }
    }
}