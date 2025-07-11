using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPPlayer : MonoBehaviour
{
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TMP_Text xpText;
    [SerializeField] public CharacterData characterData;

    private void Start()
    {
        // Garante que o UI seja atualizado no início
        UpdateXPUI();
    }
    
    private void OnEnable()
    {
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.OnExperienceChange += HandleExperienceChange;
        }
        UpdateXPUI();
    }

    private void OnDisable()
    {
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.OnExperienceChange -= HandleExperienceChange;
        }
    }

    private void HandleExperienceChange(int amount)
    {
        characterData.AddExperience(amount);
        UpdateXPUI();
    }

    private void UpdateXPUI()
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = characterData.maxExperience;
            xpSlider.value = characterData.currentExperience;
        }

        if (xpText != null)
        {
            xpText.text = $"Level {characterData.level} - {characterData.currentExperience}/{characterData.maxExperience}";
        }
    }
}