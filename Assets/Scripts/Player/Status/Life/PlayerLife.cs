using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLife : MonoBehaviour
{
    public CharacterData characterData;
    public Slider lifeSlider;
    public TMP_Text lifeText;

    private int currentLife;

    void Start()
    {
        if (characterData != null)
        {
            currentLife = characterData.maxLife;
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
        currentLife = characterData.maxLife;
        UpdateLifeUI();
    }

    public void UpdateLifeUI()
    {
        if (characterData == null) return;

        if (lifeSlider != null)
        {
            lifeSlider.maxValue = characterData.maxLife;
            lifeSlider.value = currentLife;
        }

        if (lifeText != null)
        {
            lifeText.text = $"{currentLife}/{characterData.maxLife}";
        }
    }

    public void TakeDamage(int damage)
    {
        if (characterData == null) return;

        currentLife -= damage;
        currentLife = Mathf.Clamp(currentLife, 0, characterData.maxLife);
        UpdateLifeUI();
    }

    public void Heal(int amount)
    {
        if (characterData == null) return;

        currentLife += amount;
        currentLife = Mathf.Clamp(currentLife, 0, characterData.maxLife);
        UpdateLifeUI();
    }

    public void RestoreFullLife()
    {
        if (characterData == null) return;

        currentLife = characterData.maxLife;
        UpdateLifeUI();
    }
}